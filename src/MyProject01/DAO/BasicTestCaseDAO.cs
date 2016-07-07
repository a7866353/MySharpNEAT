using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MyProject01.Agent;
using MyProject01.Test;
using MyProject01.TestCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.DAO
{
    class BasicTestEpisodeDAO
    {
        static public void SaveDao(BasicTestCaseDAO dao)
        {
            if (dao == null)
                return;
            TestCaseDatabaseConnector connector = new TestCaseDatabaseConnector();
            MongoDatabase db = connector.Connect();
            MongoCollection<BasicTestCaseDAO> collection = db.GetCollection<BasicTestCaseDAO>(BasicTestCaseDAO.EpisodeCollectionName);
            collection.Save(dao);
            connector.Close();

        }
        static protected TestCaseDatabaseConnector _connector;
        protected BasicTestCaseDAO _testCaseDAO;

        static BasicTestEpisodeDAO()
        {
            _connector = new TestCaseDatabaseConnector();
        }
      
        /*****************
         * Database values
         *****************/
        public ObjectId _id;

        public ObjectId TestCaseID { set; get; }


        

        public BasicTestEpisodeDAO(BasicTestCaseDAO testCaseDao)
        {
            _testCaseDAO = testCaseDao;
            TestCaseID = testCaseDao._id;
            _connector = new TestCaseDatabaseConnector();
        }

        public void Save()
        {
            MongoDatabase db = _connector.Connect();
            MongoCollection<BasicTestEpisodeDAO> collection = db.GetCollection<BasicTestEpisodeDAO>(BasicTestCaseDAO.EpisodeCollectionName);
            collection.Save(this);
            _connector.Close();
        }

        virtual public void Remove()
        {
            MongoDatabase db = _connector.Connect();

            // delete deal logs

            // delete episode
            MongoCollection<BasicTestCaseDAO> epCollect = db.GetCollection<BasicTestCaseDAO>(BasicTestCaseDAO.EpisodeCollectionName);
            var epQuery = new QueryDocument { { "_id", _id } };
            epCollect.Remove(epQuery);

            _connector.Close();
        }
    }
    abstract class BasicTestCaseDAO
    {
        #region Static Region

        static public readonly string CollectionName = "TestCases";
        static public readonly string EpisodeCollectionName = "TestEpisode";
        static protected TestCaseDatabaseConnector _connector;
        static BasicTestCaseDAO()
        {
            _connector = new TestCaseDatabaseConnector();
        }

        static public T GetDAO<T>(string testCaseName, bool isNew = false) 
            where T : BasicTestCaseDAO, new()
        {
            if (string.IsNullOrWhiteSpace(testCaseName) == true)
            {
                return null;
            }
            if (isNew == true)
            {
                if (RateMarketTestDAO.IsExist(testCaseName) == true)
                    RateMarketTestDAO.Remove(testCaseName);
            }
            T retDao;
            TestCaseDatabaseConnector connector = new TestCaseDatabaseConnector();
            MongoDatabase db = connector.Connect();

            MongoCollection<T> collection = db.GetCollection<T>(CollectionName);
            var query = new QueryDocument { { "TestCaseName", testCaseName } };
            var curst = collection.Find(query);
            if (curst.Count() == 0)
            {
                // not existed.
                retDao = new T();
                retDao.TestCaseName = testCaseName;
                collection.Save(retDao);
            }
            else
            {
                retDao = curst.FirstOrDefault();
            }
            connector.Close();
            return retDao;
        }
        static public T GetDAO<T>(ObjectId id)
            where T : BasicTestCaseDAO, new()
        {
            T retDao;
            TestCaseDatabaseConnector connector = new TestCaseDatabaseConnector();
            MongoDatabase db = connector.Connect();

            MongoCollection<T> collection = db.GetCollection<T>(CollectionName);
            var query = new QueryDocument { { "_id", id } };
            var curst = collection.Find(query);
            if (curst.Count() == 0)
            {
                // not existed.
                retDao = null;
            }
            else
            {
                retDao = curst.FirstOrDefault();
            }
            connector.Close();
            return retDao;
        }

        static public T[] GetAllDAOs<T>()
            where T : BasicTestCaseDAO, new()
        {
           
            T[] retDaoArr = null;
            TestCaseDatabaseConnector connector = new TestCaseDatabaseConnector();
            MongoDatabase db = connector.Connect();

            MongoCollection<T> collection = db.GetCollection<T>(CollectionName);
            var curst = collection.FindAll();
            long resultCount = curst.Count();
            if (resultCount != 0)
            {
                retDaoArr = new T[resultCount];
                int index = 0;
                foreach( T obj in curst)
                {
                    retDaoArr[index++] = obj;
                }
            }
            connector.Close();
            return retDaoArr;
        }

        static public bool IsExist(string testCaseName)
        {
            if (string.IsNullOrWhiteSpace(testCaseName) == true)
            {
                return false;
            }
            bool ret = false;
            TestCaseDatabaseConnector connector = new TestCaseDatabaseConnector();
            MongoDatabase db = connector.Connect();

            MongoCollection<BasicTestCaseDAO> collection = db.GetCollection<BasicTestCaseDAO>(CollectionName);
            var query = new QueryDocument { { "TestCaseName", testCaseName } };
            var curst = collection.Find(query);
            if (curst.Count() == 0)
            {
                ret = false;
            }
            else
            {
                ret = true;
            }
            connector.Close();

            return ret;
        }

        static public void Remove(string testCaseName)
        {
            if (string.IsNullOrWhiteSpace(testCaseName) == true)
            {
                return;
            }
            BasicTestCaseDAO retDao = null;
            TestCaseDatabaseConnector connector = new TestCaseDatabaseConnector();
            MongoDatabase db = connector.Connect();



            MongoCollection<RateMarketTestDAO> collection = db.GetCollection<RateMarketTestDAO>(CollectionName);
            var query = new QueryDocument { { "TestCaseName", testCaseName } };
            var curst = collection.Find(query);
            foreach (RateMarketTestDAO testCaseDAO in curst)
            {
                // Remove referd episodes
                RateMarketTestEpisodeDAO[] epDaoArr = testCaseDAO.GetAllEpisodes<RateMarketTestEpisodeDAO>();
                if (epDaoArr == null)
                    continue;

                foreach (BasicTestEpisodeDAO epDao in epDaoArr)
                {
                    epDao.Remove();
                }
            }
            collection.Remove(query);
            connector.Close();

        }

        #endregion


        // ===================================
        // Protect region
        #region protect region
        #endregion

        // ===========================
        // public region
        #region public region
        public void Save()
        {
            MongoDatabase db = _connector.Connect();
            MongoCollection<BasicTestCaseDAO> collection = db.GetCollection<BasicTestCaseDAO>(CollectionName);
            collection.Save(this);
            _connector.Close();
        }

        abstract public BasicTestEpisodeDAO CreateEpisode();

        public T[] GetAllEpisodes<T>()
        {
            T[] daoArr = null;
            MongoDatabase db = _connector.Connect();
            MongoCollection<T> collection = db.GetCollection<T>(EpisodeCollectionName);
            var query = new QueryDocument { { "TestCaseID", _id } };
            var curst = collection.Find(query);
            long count = curst.Count();
            if ( count != 0 )
            {
                daoArr = new T[count];
                int i = 0;
                foreach (T dao in curst)
                {
                    daoArr[i++] = dao;
                }
            }
            _connector.Close();

            return daoArr;
        }
        public void Remove()
        {
            RateMarketTestDAO.Remove(this.TestCaseName);
        }

        #endregion

        #region Private Data section
        

        #endregion

        // =============================
        // Data for saving into database.
        #region Pulbic Data section

        public ObjectId _id;
        public string TestCaseName { set; get; }
        public string TestDescription { set; get; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { set; get; }

        #endregion

    }
}
