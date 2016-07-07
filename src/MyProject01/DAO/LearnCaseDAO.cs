using Encog.Neural.NEAT;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.DAO
{
    class LearnEpisodeDAO
    {
        private TestCaseDatabaseConnector _connector;
        private LearnCaseDAO _caseDAO;
        private MongoCollection<LearnEpisodeDAO> _collection;

        /*****************
         * Database values
         *****************/
        public ObjectId _id;
        public ObjectId CaseID { set; get; }
        public DateTime CreateTime { set; get; }
        public int TrainedDealCount { set; get; }
        public int UntrainedDealCount { set; get; }
        public double TrainedDataEarnRate { set; get; }
        public double UnTrainedDataEarnRate { set; get; }
        public long Step { set; get; }

        // ================
        // Network
        public int HidenNodeCount { set; get; }

        // ====================
        // Functions
        public LearnEpisodeDAO(LearnCaseDAO caseDao)
        {
            _caseDAO = caseDao;
            CaseID = caseDao._id;
            CreateTime = DateTime.Now;
            _connector = new TestCaseDatabaseConnector();
            MongoDatabase db = _connector.Connect();
            _collection = db.GetCollection<LearnEpisodeDAO>(LearnCaseDAO.EpisodeCollectionName);
        }

        public void Save()
        {
            _collection.Save(this);
        }

        public void Remove()
        {
            MongoDBUtility.RemoveFromFS(_connector, GetDealLogName());

            var epQuery = new QueryDocument { { "_id", _id } };
            _collection.Remove(epQuery);
        }
        public void SaveDealLogs(DealLogList dealLogList)
        {
            MongoDBUtility.SaveToFS<DealLogList>(_connector, dealLogList, GetDealLogName());
        }

        public DealLogList GetDealLogs()
        {
            return MongoDBUtility.GetFromFS<DealLogList>(_connector, GetDealLogName());
        }

        private string GetDealLogName()
        {
            return _caseDAO.CaseName + Step.ToString();
        }
    }
    class LearnCaseDAO
    {
        #region Static Region
        static protected TestCaseDatabaseConnector _connector;
        static LearnCaseDAO()
        {
            _connector = new TestCaseDatabaseConnector();
        }

        static public string CollectionName
        {
            get { return "LearnCases"; }
        }
        static public string EpisodeCollectionName
        {
            get { return "LearnEpisode"; }
        }



        static public LearnCaseDAO GetDAO(string caseName, bool isNew = false)
        {
            if (string.IsNullOrWhiteSpace(caseName) == true)
            {
                return null;
            }

            var query = new QueryDocument { { "CaseName", caseName } };
            LearnCaseDAO[] retArr = MongoDBUtility.GetDAO<LearnCaseDAO>(
                _connector, CollectionName, query);

            if(isNew == true)
            {
                if(retArr != null)
                {
                    foreach( LearnCaseDAO dao in retArr)
                    {
                        dao.Remove();
                    }
                }

                LearnCaseDAO retDao = new LearnCaseDAO(caseName);
                    retDao.Save();
                return retDao;
            }

            if (retArr == null || retArr.Length == 0)
                return null;
            else
                return retArr[0];
        }
        static public LearnCaseDAO GetDAO(ObjectId id)
        {
            var query = new QueryDocument { { "_id", id } };
            LearnCaseDAO[] retArr = MongoDBUtility.GetDAO<LearnCaseDAO>(
                _connector, CollectionName, query);

            if (retArr == null || retArr.Length == 0)
                return null;
            else
                return retArr[0];
        }

        static public LearnCaseDAO[] GetAllDAOs()
        {
            LearnCaseDAO[] retDaoArr = MongoDBUtility.GetAllDAOs<LearnCaseDAO>(
                _connector, CollectionName);
            return retDaoArr;
        }
        static private MongoCollection<LearnCaseDAO> Collection
        {
            get
            {
                MongoDatabase db = _connector.Connect();
                MongoCollection<LearnCaseDAO> collection = db.GetCollection<LearnCaseDAO>(CollectionName);
                return collection;
            }
        }
        static private MongoCollection<LearnEpisodeDAO> EpisodeCollection
        {
            get
            {
                MongoDatabase db = _connector.Connect();
                MongoCollection<LearnEpisodeDAO> collection = db.GetCollection<LearnEpisodeDAO>(EpisodeCollectionName);
                return collection;
            }
        }

        #endregion


        // ===================================
        // Protect region
        #region protect region
        #endregion

        // ===========================
        // public region
        #region public region
        private LearnCaseDAO(string caseName)
        {
            CaseName = caseName;
            CreateTime = DateTime.Now;
        }
        public void Save()
        {
            Collection.Save(this);
        }

        public LearnEpisodeDAO[] GetAllEpisodes()
        {
            LearnEpisodeDAO[] daoArr = null;
            var query = new QueryDocument { { "CaseID", _id } };
            var curst = EpisodeCollection.Find(query);
            long count = curst.Count();
            if (count != 0)
            {
                daoArr = new LearnEpisodeDAO[count];
                int i = 0;
                foreach (LearnEpisodeDAO dao in curst)
                {
                    daoArr[i++] = dao;
                }
            }
            return daoArr;
        }
        public void Remove()
        {
            var query = new QueryDocument { { "CaseName", CaseName } };
            var curst = Collection.Find(query);
            foreach (LearnCaseDAO caseDao in curst)
            {
                // Remove referd episodes
                LearnEpisodeDAO[] epDaoArr = caseDao.GetAllEpisodes();
                if (epDaoArr == null)
                    continue;

                foreach (LearnEpisodeDAO epDao in epDaoArr)
                {
                    epDao.Remove();
                }
            }
            Collection.Remove(query);
        }
        #endregion

        #region Private Data section


        #endregion

        // =============================
        // Data for saving into database.
        #region Pulbic Data section

        public ObjectId _id;
        public string CaseName { set; get; }
        public DateTime CreateTime { set; get; }
        public string TestDescription { set; get; }
        public int DataBlockCount { set; get; }
        public int TestDataStartIndex { set; get; }
        public int TotalDataCount { set; get; }
        public long Step { set; get; }
        public double LastTrainedDataEarnRate { set; get; }
        public double LastTestDataEarnRate { set; get; }

        public int PopulationNumber { set; get; }

        public byte[] NetworkData { set; get; }

        public LearnEpisodeDAO CreateEpisode()
        {
            return new LearnEpisodeDAO(this);
        }
        public NEATPopulation GetPopulation()
        {
            NEATPopulation pop = MongoDBUtility.GetFromFS<NEATPopulation>(_connector, CaseName);
            return pop;

        }
        public void UpdatePopulation(NEATPopulation pop)
        {
            MongoDBUtility.SaveToFS<NEATPopulation>(_connector, pop, CaseName);
        }

        #endregion

    }
}
