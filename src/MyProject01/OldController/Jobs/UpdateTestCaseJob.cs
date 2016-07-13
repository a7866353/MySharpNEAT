using MyProject01.Agent;
using MyProject01.DAO;
using MyProject01.ExchangeRateTrade;
using MyProject01.Util;
using MyProject01.Util.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller.Jobs
{
    class UpdateTestCaseJob : ICheckJob
    {
        public string TestName;
        public string TestDescription;
        public ITradeDesisoin DecisionCtrl;
        public BasicDataBlock TestDataBlock;

        private RateMarketTestDAO _testCaseDAO;

        private int _trainDataLength;
        private long _epoch;
        private LogFormater _log;

        public UpdateTestCaseJob()
        {
            _log = new LogFormater();
            LogFile.WriteLine(_log.GetTitle());
            _testCaseDAO = null;
        }
        public bool Do(TrainerContex context)
        {
            if (context.IsChanged == false)
                return true;

            if (_testCaseDAO == null)
            {
                _testCaseDAO = RateMarketTestDAO.GetDAO<RateMarketTestDAO>(TestName, true);
                _testCaseDAO.TestDescription = TestDescription;
                _testCaseDAO.TestDataStartIndex = context._trainDataBlock.BlockCount;
                _testCaseDAO.TotalDataCount = TestDataBlock.BlockCount;

            }

            _epoch = context.Epoch;
            _trainDataLength = _testCaseDAO.TestDataStartIndex;
            TestResult(context.BestNetwork, _testCaseDAO);

            _testCaseDAO.NetworkData = null;
            _testCaseDAO.Step = _epoch;
            _testCaseDAO.Save();

            return true;

        }
        private void TestResult(NEATNetwork network, RateMarketTestDAO dao)
        {
            RateMarketAgent agent = new RateMarketAgent(TestDataBlock);
            DecisionCtrl.UpdateNetwork(network);

            TradeController tradeCtrl = new TradeController(agent, DecisionCtrl);
            RateMarketTestEpisodeDAO epsodeLog = (RateMarketTestEpisodeDAO)dao.CreateEpisode();
            DealLogList logList = new DealLogList();
            int trainDealCount = 0;
            int trainedDataIndex = _trainDataLength;
            double startMoney = agent.InitMoney;
            double trainedMoney = 0;
            double endMoney = 0;
            while (true)
            {
                if (agent.CurrentRateValue > 0)
                {
                    // Get Action Value
                    tradeCtrl.DoAction();

                    // Add log
                    logList.Add(agent.LastAction, agent.CurrentValue, agent.CurrentRateValue);

                    // To large for test
                    // epsodeLog.DealLogs.Add(dealLog);
                    if (agent.index == trainedDataIndex)
                    {
                        trainedMoney = agent.CurrentValue;
                        // trainDealCount = dealCount;
                        trainDealCount = agent.DealCount;
                    }

                }
                if (agent.IsEnd == true)
                    break;
            } // end while

            if( agent.index < trainedDataIndex )
            {
                trainedMoney = agent.CurrentValue;
                // trainDealCount = dealCount;
                trainDealCount = agent.DealCount;
                trainedDataIndex = agent.index;
            }

            endMoney = agent.CurrentValue;

            epsodeLog.TrainedDataEarnRate = (trainedMoney / startMoney) * 100;
            epsodeLog.UnTrainedDataEarnRate = (endMoney / trainedMoney) * 100;
            epsodeLog.TrainedDealCount = trainDealCount;
            epsodeLog.UntrainedDealCount = agent.DealCount - trainDealCount;
            // epsodeLog.HidenNodeCount = network.Links.Length;
            epsodeLog.ResultMoney = endMoney;
            epsodeLog.Step = _epoch;
            epsodeLog.Save();
            epsodeLog.SaveDealLogs(logList);

            // update dao
            dao.LastTestDataEarnRate = epsodeLog.UnTrainedDataEarnRate;
            dao.LastTrainedDataEarnRate = epsodeLog.TrainedDataEarnRate;

            // update log
            _log.Set(LogFormater.ValueName.TrainScore, epsodeLog.TrainedDataEarnRate);
            _log.Set(LogFormater.ValueName.UnTrainScore, epsodeLog.UnTrainedDataEarnRate);
            _log.Set(LogFormater.ValueName.Step, _epoch);
            LogFile.WriteLine(_log.GetLog());



        }

    }
}
