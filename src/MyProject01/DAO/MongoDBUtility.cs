using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.DAO
{
    class MongoDBUtility
    {
        static public T GetFromFS<T>(TestCaseDatabaseConnector connector, string fileName)
        {
            MongoDatabase db = connector.Connect();

            MongoGridFS fs = new MongoGridFS(db);
            MongoGridFSStream file = fs.OpenRead(fileName);
            if (file.Length == 0)
                return default(T);

            byte[] buffer = new byte[file.Length];
            file.Read(buffer, 0, buffer.Length);
            file.Close();

            // connector.Close();

            MemoryStream stream = new MemoryStream(buffer);
            BinaryFormatter formatter = new BinaryFormatter();
            T resultObj = (T)formatter.Deserialize(stream);
            stream.Close();

            return resultObj;

        }

        static public void SaveToFS<T>(TestCaseDatabaseConnector connector, T obj, string fileName)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            byte[] res = stream.ToArray();
            stream.Close();

            MongoDatabase db = connector.Connect();

            MongoGridFS fs = new MongoGridFS(db);
            MongoGridFSStream file = fs.Create(fileName);
            file.Write(res, 0, res.Length);
            file.Close();

            // connector.Close();
        }

        static public void RemoveFromFS(TestCaseDatabaseConnector connector, string fileName)
        {
            MongoDatabase db = connector.Connect();

            MongoGridFS fs = new MongoGridFS(db);
            fs.Delete(fileName);

            // connector.Close();
        }
        static public T[] GetDAO<T>(TestCaseDatabaseConnector connector, string collectionName,
            QueryDocument query)
        {
            T[] retDao;
            MongoDatabase db = connector.Connect();

            MongoCollection<T> collection = db.GetCollection<T>(collectionName);
            var curst = collection.Find(query);
            if (curst.Count() == 0)
            {
                // not existed.
                retDao = null;
            }
            else
            {
                retDao = new T[curst.Count()];
                int i=0;
                foreach(T dao in curst)
                {
                    retDao[i] = dao;
                    i++;
                }
            }
            return retDao;
        }
        static public T[] GetAllDAOs<T>(TestCaseDatabaseConnector connector, string collectionName) 
        {
            T[] retDaoArr = null;
            MongoDatabase db = connector.Connect();
            MongoCollection<T> collection = db.GetCollection<T>(collectionName);
            var curst = collection.FindAll();
            long resultCount = curst.Count();
            if (resultCount != 0)
            {
                retDaoArr = new T[resultCount];
                int index = 0;
                foreach (T obj in curst)
                {
                    retDaoArr[index++] = obj;
                }
            }
            return retDaoArr;
        }

    }
}
