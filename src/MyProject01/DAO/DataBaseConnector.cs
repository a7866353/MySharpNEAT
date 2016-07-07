using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyProject01.DAO
{
    public class DataBaseAddress
    {
#if true
        static public string ConnectionString = @"mongodb://192.168.1.15";
#else
        static public string ConnectionString = @"mongodb://127.0.0.1";
#endif
        static public void SetIP(string ip)
        {
            ConnectionString = @"mongodb://" + ip;
        }
    }
    class MarketRateDatabaseConnector
    {
        public static string DatabaseName = "MarketRateDB_160312";
        // public static string DatabaseName = "MarketRateDB_160322";
        public static string ConnectionString = DataBaseAddress.ConnectionString;
        // public static string ConnectionString = @"mongodb://127.0.0.1";

        private static MongoServer server = null;

        public MongoDatabase Database
        {
            get { return db; }
        }

        private MongoDatabase db;


        public MongoDatabase Connect()
        {
            if (server == null)
            {
                server = MongoServer.Create(ConnectionString);
                if (null == server)
                {
                    throw (new Exception("Cannot connect to server!"));
                }
            }

            db = server.GetDatabase(DatabaseName); // Create a new Database or get a current Database
            if (null == db)
            {
                throw (new Exception("Cannot connect to server!"));
            }

            return db;
        }

        public void Close()
        {
            // server.Disconnect();
            db = null;
        }
    }
    public class TestCaseDatabaseConnector
    {
        public static string DatabaseName = "NetWorkTestDB";
        // public static string ConnectionString = @"mongodb://127.0.0.1";
        public static string ConnectionString = DataBaseAddress.ConnectionString;

        // public static string ConnectionString = @"mongodb://192.168.1.15";
        // public static string ConnectionString = @"mongodb://192.168.1.11";

        public static Semaphore Lock;

        static TestCaseDatabaseConnector()
        {
            Lock = new Semaphore(1, 1);
        }

        public MongoDatabase Database
        {
            get { return db; }
        }

        private static MongoServer server = null;
        private MongoDatabase db;


        public MongoDatabase Connect()
        {
            // Lock.WaitOne();
            if (server == null)
            {
                MongoClient client = new MongoClient(ConnectionString);
                server = client.GetServer();
                if (null == server)
                {
                    throw (new Exception("Cannot connect to server!"));
                }
            }

            db = server.GetDatabase(DatabaseName); // Create a new Database or get a current Database
            if (null == db)
            {
                throw (new Exception("Cannot connect to server!"));
            }

            return db;
        }

        public void Close()
        {
            // server.Disconnect();
            db = null;
            // Lock.Release();
        }
    }
}
