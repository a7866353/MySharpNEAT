using MongoDB.Bson;
using MongoDB.Driver;
using MyProject01.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MyProject01.Util
{
        abstract class BasicDataObject
    {
        public ObjectId _id;
    }

    class MtDateComparer : IComparer<RateData>  
    {
        private bool _isIncr = false;

        public MtDateComparer()
        {
        }
        public int Compare(RateData x, RateData y)
        {
            if (x.time > y.time)
                return 1;
            else if (x.time == y.time)
                return 0;
            else
                return -1;
        }
    }
    class MTDataBuffer : List<RateData>
    {
        static List<MTDataBuffer> _loaderList;
        static Semaphore _getLock;
        static MTDataBuffer()
        {
            _loaderList = new List<MTDataBuffer>();
        }
        static public MTDataBuffer GetLoader(string tickerName, int countLimit = 100000)
        {
            // find saved loader
            MTDataBuffer loader = null;
            foreach (MTDataBuffer l in _loaderList)
            {
                if (l.TickerName == tickerName)
                {
                    loader = l;
                    break;
                }

            }
            if (loader != null)
                return loader;

            // find in db
            loader = new MTDataBuffer(tickerName, countLimit);
            if (loader.Count == 0)
                return null;

            _loaderList.Add(loader);
            return loader;
            
        }

        private string _tickerName;
        public string TickerName
        {
            get { return _tickerName; }
        }
        private MTDataBuffer(string tickerName, int countLimit)
        {
            _tickerName = tickerName;

            MarketRateDatabaseConnector connector = new MarketRateDatabaseConnector();
            MongoDatabase db = connector.Connect();

            string collectionName = _tickerName;
            MongoCollection collection = db.GetCollection(collectionName);

            if (true)
            {
                /*
                MongoCursor cursor = collection.FindAllAs<RateData>();
                cursor.BatchSize = 1000;
                int loadMaxCount = 200000;
                foreach (RateData dataObj in cursor)
                {
                    Add(dataObj);
                    loadMaxCount--;
                    if (loadMaxCount == 0)
                        break;
                }
                */

                /*
                // Add by date limit
                QueryDocument query = new QueryDocument();
                BsonDocument b = new BsonDocument();
                b.Add("$gt", DateTime.Now.AddMonths(-24));
                b.Add("$lt", DateTime.Now);
                query.Add("Date", b);
                var curst = collection.FindAs<RateData>(query);
                */

                var curst = collection.FindAllAs<RateData>();
                curst.BatchSize = 1000;
                curst.SetLimit(countLimit);  // set limit
                foreach (RateData dataObj in curst)
                {
                    Add(dataObj);
                }

            }
            else // for parallel
            {
                var collc = collection.ParallelScanAs<RateData>(new ParallelScanArgs() { NumberOfCursors = 128, BatchSize = 1000 });

                Parallel.ForEach(collc, curs =>
                {
                    while (curs.MoveNext())
                    {
                        lock (this)
                        {
                            Add(curs.Current);
                        }
                    }
                });

            }
            connector.Close();
            SortByDate();
        }

              
        private void SortByDate()
        {
            Sort(new MtDateComparer());
        }
    }
}
