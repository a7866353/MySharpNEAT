using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MyProject01.DAO;
using MongoDB.Bson.Serialization.Attributes;

namespace MyProject01.DAO
{
    class RateDataDateComparer : IComparer<RateData>
    {
        private bool _isIncr = false;

        public RateDataDateComparer(bool isIncr)
        {
            _isIncr = isIncr;
        }
        public int Compare(RateData x, RateData y)
        {
            if (_isIncr)
            {
                if (x.time > y.time)
                    return 1;
                else if (x.time == y.time)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (x.time < y.time)
                    return 1;
                else if (x.time == y.time)
                    return 0;
                else
                    return -1;
            }

        }
    }
    public abstract class BasicDataObject
    {
        public ObjectId _id;
    }
    public class RateData : BasicDataObject
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime time;
        public double open;
        public double high;
        public double low;
        public double close;
        public long tick_volume;
        public long real_volume;
        public int spread;
 
    }
    public class RateDataControlDAO
    {
        static private string _controlCollectionName = "_RateDataControl";
        static public RateDataControlDAO[] GetList()
        {
            RateDataControlDAO[] retDaoArr = null;
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();
            MongoDatabase db = connector.Connect();

            MongoCollection<RateDataControlDAO> collection = db.GetCollection<RateDataControlDAO>(_controlCollectionName);
            var curst = collection.FindAll();
            long resultCount = curst.Count();
            if (resultCount != 0)
            {
                retDaoArr = new RateDataControlDAO[resultCount];
                int index = 0;
                foreach (RateDataControlDAO obj in curst)
                {
                    retDaoArr[index++] = obj;
                }
            }
            connector.Close();
            return retDaoArr;
        }

        static public bool IsExist(string name)
        {
            bool isExist = false;
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();
            MongoDatabase db = connector.Connect();
            MongoCollection<RateDataControlDAO> collection = db.GetCollection<RateDataControlDAO>(_controlCollectionName);
            var query = new QueryDocument { { "Name", name } };
            var curst = collection.Find(query);
            if (curst.Count() == 0)
            {
                isExist = false;
            }
            else
            {
                isExist = true;
            }
            connector.Close();

            return isExist;
        }

        static public RateDataControlDAO GetByName(string name)
        {
            RateDataControlDAO res;
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();
            MongoDatabase db = connector.Connect();
            MongoCollection<RateDataControlDAO> collection = db.GetCollection<RateDataControlDAO>(_controlCollectionName);
            var query = new QueryDocument { { "Name", name } };
            var curst = collection.Find(query);
            if (curst.Count() == 0)
            {
                res = null;
            }
            else if (curst.Count() == 1)
            {
                res = curst.FirstOrDefault();
            }
            else
            {
                res = curst.FirstOrDefault();
                throw (new Exception("Get too many!"));
            }
            connector.Close();
            return res;
        }

        static public RateDataControlDAO Create(string name, string symbolName, int timeFrame, DateTime startTime)
        {
            RateDataControlDAO dao = new RateDataControlDAO(name, symbolName, timeFrame, startTime);
            return dao;
        }

        public ObjectId _id;
        public string Name { set; get; }
        public string SymbolName { set; get; }
        public int TimeFrame { set; get; }
        public string CollectiongName { set; get; }
        public int Count { set; get; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartTime { set; get; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastItemTime { set; get; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastGetTime { set; get; }

        private RateDataControlDAO(string name, string symbolName, int timeFrame, DateTime startTime)
        {
            this.Name = name;
            this.SymbolName = symbolName;
            this.TimeFrame = timeFrame;
            this.StartTime =  startTime;
            this.LastItemTime = this.LastGetTime = startTime;
            this.Count = 0;

            this.CollectiongName = name;

            Remove();
            this.Save();
            SetIndex();

        }

        private void SetIndex()
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();
            MongoDatabase db = connector.Connect();
            MongoCollection<RateData> collection = db.GetCollection<RateData>(CollectiongName);
            // collection.CreateIndex("time", IndexOptions.SetUnique);


            collection.CreateIndex(IndexKeys.Descending("time"), IndexOptions.SetUnique(true));
            connector.Close();


        }
        public void Save()
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();
            MongoDatabase db = connector.Connect();
            MongoCollection<RateDataControlDAO> collection = db.GetCollection<RateDataControlDAO>(_controlCollectionName);
            collection.Save(this);
            connector.Close();

        }
        public void Update()
        {
            RateDataControlDAO dao = RateDataControlDAO.GetByName(this.Name);
            if (dao == null)
                return;
            this.Count = dao.Count;
            this.LastItemTime = dao.LastItemTime;
            this.LastGetTime = dao.LastGetTime;
        }

        public void Remove()
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();
            MongoDatabase db = connector.Connect();
            MongoCollection<RateDataControlDAO> collection = db.GetCollection<RateDataControlDAO>(_controlCollectionName);

            var query = new QueryDocument { { "Name", Name } };
            collection.Remove(query);

            db.DropCollection(CollectiongName);

            connector.Close();
        }

        public void Add(RateData data)
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();
            MongoDatabase db = connector.Connect();
            MongoCollection<RateData> collection = db.GetCollection<RateData>(CollectiongName);
            collection.Insert(data);
            connector.Close();

            this.Count += 1;
            if (data.time > this.LastItemTime)
            {
                this.LastItemTime = data.time;
            }
            Save();
        }

        public void Add(RateData[] datas)
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();

            MongoDatabase db = connector.Connect();
            MongoCollection<RateData> collection = db.GetCollection<RateData>(CollectiongName);
            collection.InsertBatch(datas);

            connector.Close();

            this.Count += datas.Length;
            if (datas[datas.Length - 1].time > this.LastItemTime)
            {
                this.LastItemTime = datas[datas.Length - 1].time;
            }

            Save();

        }

        public RateData[] Get(DateTime startTime, DateTime endTime)
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();

            MongoDatabase db = connector.Connect();
            MongoCollection<RateData> collection = db.GetCollection<RateData>(CollectiongName);
            QueryDocument query = new QueryDocument();
            BsonDocument b = new BsonDocument();
            b.Add("$gt", startTime);
            b.Add("$lt", endTime);
            query.Add("time", b);

            SortByDocument s = new SortByDocument();
            s.Add("time", 1);

            var curst = collection.Find(query).SetSortOrder(s);
            RateData[] dataArr = curst.ToArray<RateData>();

            return dataArr;
        }

        public RateData[] Get(DateTime startTime, int count)
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();

            MongoDatabase db = connector.Connect();
            MongoCollection<RateData> collection = db.GetCollection<RateData>(CollectiongName);
            QueryDocument query = new QueryDocument();
            BsonDocument b = new BsonDocument();
            b.Add("$lt", startTime);
            query.Add("time", b);

            SortByDocument s = new SortByDocument();
            s.Add("time", 1);

            var curst = collection.Find(query).SetSortOrder(s).SetLimit(count);
            RateData[] dataArr = curst.ToArray<RateData>();

            return dataArr;
        }
        public RateData[] GetByEndTime(DateTime endTime, int count)
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();

            MongoDatabase db = connector.Connect();
            MongoCollection<RateData> collection = db.GetCollection<RateData>(CollectiongName);
            QueryDocument query = new QueryDocument();
            BsonDocument b = new BsonDocument();
            b.Add("$lte", endTime);
            query.Add("time", b);

            SortByDocument sDown = new SortByDocument();
            sDown.Add("time", -1);

            SortByDocument sUp = new SortByDocument();
            sUp.Add("time", 1);

            MongoCursor<RateData> curst;
            curst = collection.Find(query).SetSortOrder(sDown).SetLimit(count);
            List<RateData> dataList = curst.ToList<RateData>();
            dataList.Sort(new RateDataDateComparer(true));

            return dataList.ToArray();
        }
        public RateData[] Get(int startIndex, int count)
        {
            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();

            MongoDatabase db = connector.Connect();
            MongoCollection<RateData> collection = db.GetCollection<RateData>(CollectiongName);

            SortByDocument s = new SortByDocument();
            s.Add("time", 1);

            var curst = collection.FindAll().SetSortOrder(s).SetSkip(startIndex).SetLimit(count);
            RateData[] dataArr = curst.ToArray<RateData>();

            return dataArr;
        }

        


    }
}
