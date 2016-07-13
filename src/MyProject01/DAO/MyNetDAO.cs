using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MyProject01.Agent;
using MyProject01.Controller;
using MyProject01.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.DAO
{
     [Serializable]
    class DealLog
    {
        public MarketActions Action { set; get; }
        public double CurrentMoney { set; get; }
        public double Rate;

        public override string ToString()
        {
            return Action + "@" + Rate + ", " + CurrentMoney;
        }
    }

    [Serializable]
    class DealLogList : List<DealLog>
    {
        public void Add(MarketActions action, double currentMoney, double rate)
        {
            Add(new DealLog { Action = action, CurrentMoney = currentMoney, Rate = rate });
        }

    }
    class RateMarketTestEpisodeDAO : BasicTestEpisodeDAO
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { set; get; }
        public double ResultMoney { set; get; }
        public int TrainedDealCount { set; get; }
        public int UntrainedDealCount { set; get; }
        public double TrainedDataEarnRate { set; get; }
        public double UnTrainedDataEarnRate { set; get; }

        public long Step { set; get; }

        // ================
        // Network
        public string ControllerName { set; get; }
        public int HidenNodeCount { set; get; }

        // ====================
        // Functions
        public RateMarketTestEpisodeDAO(RateMarketTestDAO testCaseDAO)
            : base(testCaseDAO)
        {

        }

        public override void Remove()
        {
            MongoDBUtility.RemoveFromFS(new TestCaseDatabaseConnector(), GetDealLogName());
            base.Remove();
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
            if (_testCaseDAO == null )
            {
                _testCaseDAO = RateMarketTestDAO.GetDAO<RateMarketTestDAO>(TestCaseID);
            }
            return _testCaseDAO.TestCaseName + Step.ToString();
        }
    }

    class RateMarketTestDAO : BasicTestCaseDAO
    {
        public int DataBlockCount { set; get; }
        public int TestDataStartIndex { set; get; }
        public int TotalDataCount { set; get; }
        public long Step { set; get; }
        public double LastTrainedDataEarnRate { set; get; }
        public double LastTestDataEarnRate { set; get; }

        public int PopulationNumber { set; get; }

        public byte[] NetworkData { set; get; }

        public override BasicTestEpisodeDAO CreateEpisode()
        {
            return new RateMarketTestEpisodeDAO(this);
        }


    }
}
