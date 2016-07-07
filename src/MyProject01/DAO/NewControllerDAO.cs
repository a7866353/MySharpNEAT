using Encog.Neural.NEAT;
using MongoDB.Bson;
using MongoDB.Driver;
using MyProject01.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.DAO
{
    class NewControllerDAO
    {
        #region Static Region

        static public string CollectionName = "NewControllers";
        static private MongoCollection<NewControllerDAO> _collection;
        static private TestCaseDatabaseConnector _connector;
        static NewControllerDAO()
        {
            _connector = new TestCaseDatabaseConnector();
            MongoDatabase db = _connector.Connect();
            _collection = db.GetCollection<NewControllerDAO>(CollectionName);
        }
        static public NewControllerDAO GetDAO(string name, bool isNew = false)
        {
            if (string.IsNullOrWhiteSpace(name) == true)
            {
                return null;
            }


            var query = new QueryDocument { { "Name", name } };
            NewControllerDAO[] retArr = MongoDBUtility.GetDAO<NewControllerDAO>(
                _connector, CollectionName, query);
            if (isNew == true)
            {
                if (retArr != null)
                {
                    foreach (NewControllerDAO dao in retArr)
                    {
                        dao.Remove();
                    }
                }

                NewControllerDAO retDao = new NewControllerDAO(name);
                retDao.Save();
                return retDao;
            }

            if (retArr == null || retArr.Length == 0)
                return null;
            else
                return retArr[0];
        }
        static public NewControllerDAO[] GetDAOByCase(string caseName)
        {
            var query = new QueryDocument { { "CaseName", caseName } };
            NewControllerDAO[] retArr = MongoDBUtility.GetDAO<NewControllerDAO>(
                _connector, CollectionName, query);

            return retArr;
        }
        static public NewControllerDAO[] GetAllDAOs()
        {

            NewControllerDAO[] retDaoArr = null;
            var curst = _collection.FindAll();
            long resultCount = curst.Count();
            if (resultCount != 0)
            {
                retDaoArr = new NewControllerDAO[resultCount];
                int index = 0;
                foreach (NewControllerDAO obj in curst)
                {
                    retDaoArr[index++] = obj;
                }
            }
            return retDaoArr;
        }
        #endregion


        // ===================================
        // Protect region
        #region protect region
        #endregion

        // ===========================
        // public region
        #region public region
        public NewControllerDAO(string name)
        {
            Name = name;
            CreateTime = DateTime.Now;
        }
        public void Save()
        {
            UpdateTime = DateTime.Now;
            _collection.Save(this);
        }


        public void Remove()
        {
            var query = new QueryDocument { { "Name", Name } };
            _collection.Remove(query);

            MongoDBUtility.RemoveFromFS(_connector, Name);
        }



        public void Set(IController ctrl)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, ctrl);
            ControllerObject = stream.ToArray();
            stream.Close();
        }

        public IController Get()
        {
            MemoryStream stream = new MemoryStream(ControllerObject);
            BinaryFormatter formatter = new BinaryFormatter();
            IController obj = (IController)formatter.Deserialize(stream);

            return obj;
        }

        #endregion
        // =============================
        // Data for saving into database.
        #region Data section

        public ObjectId _id;
        public string Name { set; get; }
        public string CaseName { set; get; }
        public int Step { set; get; }
        public string Symbol { set; get; }
        public int TimeFrame { set; get; }
        public DateTime CreateTime { set; get; }
        public DateTime UpdateTime { set; get; }
        public string Description { set; get; }
        public byte[] ControllerObject { set; get; }

        #endregion

    }
}
