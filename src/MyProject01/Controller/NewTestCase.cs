using MyProject01.Agent;
using MyProject01.Controller.Jobs;
using MyProject01.DAO;
using MyProject01.NeuroNetwork;
using MyProject01.Util;
using MyProject01.Util.DllTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    class NewTestDataPacket
    {
        static public BasicTestDataLoader GetCurrentM1_1Month()
        {
            DateTime EndDateTime = DateTime.Now;
            DateTime StartDateTime = EndDateTime.AddMonths(-1);
            BasicTestDataLoader loader =
                // new TestDataDateRangeLoader("USDJPY_30", DataTimeType.M30, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = false };
                new TestDataDateRangeLoader("USDJPY_1", DataTimeType.M1, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = false };
            // loader.Load();
            return loader;
        }

        static public BasicTestDataLoader GetCurrentM30_1Month()
        {
            DateTime EndDateTime = DateTime.Now;
            DateTime StartDateTime = EndDateTime.AddMonths(-1);
            BasicTestDataLoader loader =
                // new TestDataDateRangeLoader("USDJPY_30", DataTimeType.M30, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = false };
                new TestDataDateRangeLoader("USDJPY_1", DataTimeType.M30, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = true };
           // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetCurrentM30_3Month()
        {
            DateTime EndDateTime = DateTime.Now;
            DateTime StartDateTime = EndDateTime.AddMonths(-3);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_30", DataTimeType.M30, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = false };
                // new TestDataDateRangeLoader("USDJPY_1", DataTimeType.M30, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = true };

            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetRecentM5_1Month()
        {
            DateTime StartDateTime = new DateTime(2016, 2, 13);
            DateTime EndDateTime = new DateTime(2016, 3, 13);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_1", DataTimeType.M5, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = true };
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetRecentM5_3Month()
        {
            DateTime StartDateTime = new DateTime(2015, 12, 13);
            DateTime EndDateTime = new DateTime(2016, 3, 13); 
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_5", DataTimeType.M5, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = false };
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetRecentM5_2Year()
        {
            DateTime EndDateTime = new DateTime(2016, 3, 13);
            DateTime StartDateTime = EndDateTime.AddYears(-2);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_5", DataTimeType.M5, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = false };
            // loader.Load();
            return loader;
        }

        static public BasicTestDataLoader GetRecnetM30_1Month()
        {
            DateTime StartDateTime = new DateTime(2016, 2, 13);
            DateTime EndDateTime = new DateTime(2016, 3, 13);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_1", DataTimeType.M30, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetRecnetM30_1Month_NonCov()
        {
            DateTime StartDateTime = new DateTime(2016, 2, 13);
            DateTime EndDateTime = new DateTime(2016, 3, 13);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_30", DataTimeType.M30, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = false };
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetRecnetM30_3Month()
        {
            DateTime StartDateTime = new DateTime(2015, 12, 13);
            DateTime EndDateTime = new DateTime(2016, 3, 13);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_1", DataTimeType.M30, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }

        static public BasicTestDataLoader GetRecnetM30_2Year()
        {
            DateTime EndDateTime = new DateTime(2016, 3, 13);
            DateTime StartDateTime = EndDateTime.AddYears(-2);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_1", DataTimeType.M30, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetRecnetM30_10Year()
        {
            DateTime EndDateTime = new DateTime(2016, 3, 13);
            DateTime StartDateTime = EndDateTime.AddYears(-10);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY_30", DataTimeType.M30, StartDateTime, EndDateTime, 50000) { NeedTimeFrameConver = false };
            // loader.Load();
            return loader;
        }

        static public BasicTestDataLoader GetM5_1Year()
        {
            DateTime StartDateTime = new DateTime(2013, 10, 31);
            DateTime EndDateTime = new DateTime(2014, 10, 31);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY", DataTimeType.M5, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetM30OneMonth()
        {
            DateTime StartDateTime = new DateTime(2013, 11, 1);
            DateTime EndDateTime = new DateTime(2013, 12, 31);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY", DataTimeType.M30, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetM30OneYear()
        {
            DateTime StartDateTime = new DateTime(2013, 10, 31);
            DateTime EndDateTime = new DateTime(2014, 10, 31);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY", DataTimeType.M30, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader GetD1OneMounth()
        {
            DateTime StartDateTime = new DateTime(2013, 11, 1);
            DateTime EndDateTime = new DateTime(2013, 12, 31);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY", DataTimeType.D1, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader Get1DayOneYear()
        {
            DateTime StartDateTime = new DateTime(2011, 10, 31);
            DateTime EndDateTime = new DateTime(2014, 10, 31);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY", DataTimeType.D1, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }
        static public BasicTestDataLoader Get1Day10Year()
        {
            DateTime StartDateTime = new DateTime(2004, 10, 31);
            DateTime EndDateTime = new DateTime(2014, 10, 31);
            BasicTestDataLoader loader =
                new TestDataDateRangeLoader("USDJPY", DataTimeType.D1, StartDateTime, EndDateTime, 50000);
            // loader.Load();
            return loader;
        }
    }
    class NewUpdateControllerJob : ICheckJob
    {
#if false
        private ControllerPacker _packer;
        private string _caseName;
        public NewUpdateControllerJob(string caseName, ControllerPacker packer)
        {
            _caseName = caseName;
            _packer = packer;
        }


        public bool Do(TrainerContex context)
        {
            if (context.IsChanged == false)
                return true;

            // ControllerDAOV2 dao = ControllerDAOV2.GetDAOByName("Controller"+DateTime.Now);
            ControllerDAOV2 dao = new ControllerDAOV2();
            dao.Name = "Controller" + DateTime.Now;
            dao.CaseName = _caseName;
            dao.StepNum = (int)context.Epoch;
            dao.UpdateTime = DateTime.Now;
            _packer.NeuroNetwork = context.BestNetwork;
            dao.ControllerData = _packer.GetData();
            dao.Save();
            context.ControllerName = dao.Name;

            return true;
        }
#endif
        public bool Do(TrainerContex context)
        {
            throw new NotImplementedException();
        }
    }
    class NewUpdateTestCaseJob : ICheckJob
    {
        public string TestName;
        public string TestDescription;
        public IController Controller;
        public int StartPosition = 50000;
        public int TrainDataLength;
        public int TestDataLength;

        private RateMarketTestDAO _testCaseDAO;

        private int _testStartIndex;
        private long _epoch;
        private LogFormater _log;
        private double _lastFitness;

        private TrainerContex _context;
        public NewUpdateTestCaseJob()
        {
            _log = new LogFormater();
            LogFile.WriteLine(_log.GetTitle());
            _testCaseDAO = null;
            _lastFitness = 0;
        }
        public bool Do(TrainerContex context)
        {
            if (_lastFitness == context.Fitness)
                return true;
            _lastFitness = context.Fitness;

            _context = context;
            if (_testCaseDAO == null)
            {
                _testCaseDAO = RateMarketTestDAO.GetDAO<RateMarketTestDAO>(TestName, true);
                _testCaseDAO.TestDescription = TestDescription;
                // TODO
                _testCaseDAO.TotalDataCount = TrainDataLength + TestDataLength;
                _testCaseDAO.TestDataStartIndex = TrainDataLength;
                _testCaseDAO.Time = _context.StartDate;

            }

            _epoch = context.Epoch;
            _testStartIndex = StartPosition + TrainDataLength;
            TestResult(context.BestNetwork, _testCaseDAO);

            _testCaseDAO.NetworkData = null;
            _testCaseDAO.Step = _epoch;
            _testCaseDAO.Save();

            return true;

        }
        private void TestResult(NEATNetwork network, RateMarketTestDAO dao)
        {
            Controller.UpdateNetwork(network);
            LearnRateMarketAgent agent = new LearnRateMarketAgent(Controller);
            agent.Reset();
            agent.SetRange(StartPosition, Controller.TotalLength);

            RateMarketTestEpisodeDAO epsodeLog = (RateMarketTestEpisodeDAO)dao.CreateEpisode();
            DealLogList logList = new DealLogList();
            int trainDealCount = 0;
            int trainedDataIndex = _testStartIndex;
            double startMoney = agent.InitMoney;
            double trainedMoney = 0;
            double endMoney = 0;
            while (true)
            {
                if (agent.IsEnd == true)
                    break;

                // Get Action Value
                agent.DoAction();

                // Add log
                logList.Add(agent.LastAction, agent.CurrentValue, agent.CurrentRateValue);

                // To large for test
                // epsodeLog.DealLogs.Add(dealLog);
                if (agent.CurrentIndex == trainedDataIndex)
                {
                    trainedMoney = agent.CurrentValue;
                    // trainDealCount = dealCount;
                    trainDealCount = agent.DealCount;
                }
                agent.Next();

            } // end while

            if (agent.CurrentIndex < trainedDataIndex)
            {
                trainedMoney = agent.CurrentValue;
                // trainDealCount = dealCount;
                trainDealCount = agent.DealCount;
                trainedDataIndex = (int)agent.CurrentIndex;
            }

            endMoney = agent.CurrentValue;

            epsodeLog.TrainedDataEarnRate = (trainedMoney / startMoney) * 100;
            epsodeLog.UnTrainedDataEarnRate = (endMoney / trainedMoney) * 100;
            epsodeLog.TrainedDealCount = trainDealCount;
            epsodeLog.UntrainedDealCount = agent.DealCount - trainDealCount;
            // epsodeLog.HidenNodeCount = network.Links.Length;
            epsodeLog.ResultMoney = endMoney;
            epsodeLog.Step = _epoch;
            epsodeLog.ControllerName = _context.ControllerName;
            epsodeLog.Time = _context.CurrentDate;
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

   
    public class AgentFactory
    {
        private ControllerFactory _ctrlFactory;
        private int _startPositon;
        private int _trainDataLength;

        public AgentFactory(ControllerFactory ctrlFactory)
        {
            _ctrlFactory = ctrlFactory;
            _startPositon = _ctrlFactory.BaseController.CurrentPosition;
            _trainDataLength = _ctrlFactory.BaseController.TotalLength;
        }
        public int StartPosition
        {
            set { _startPositon = value; }
        }
        public int TrainDataLength
        {
            set { _trainDataLength = value; }
        }
        public IController BaseController
        {
            get { return _ctrlFactory.BaseController; }
        }
        public double Run(NEATNetwork net)
        {
            double result;
            IController ctrl = _ctrlFactory.Get();

            ctrl.UpdateNetwork(net);
            LearnRateMarketAgent agent = new LearnRateMarketAgent(ctrl);
            agent.SetRange(_startPositon, _startPositon + _trainDataLength);
            while (true)
            {
                if (agent.IsEnd == true)
                    break;

                agent.DoAction();
                agent.Next();
                if (agent.IsEnd == true)
                    break;
            }

            _ctrlFactory.Free(ctrl);

            result = agent.CurrentValue;
            return result;
        }
    }
    abstract class BasicNewTestCase
    {
        private ControllerFactory _ctrlFac;
        private BasicControllerWithCache _testCtrl;
        private BasicTestDataLoader _loader;
        private AgentFactory _agentFac;

        private double _testRate = 0.7;
        private int _startPosition = 50000;
        private int _trainBlockLength = 32;
        private int _trainTryCount = 2;

        private int _trainDataLength;
        private int _testDataLength;
        public void Run()
        {
            // Config Server IP
            DataBaseAddress.SetIP(CommonConfig.ServerIP);

            _trainBlockLength = CommonConfig.TrainingDataBlockLength;
            _trainTryCount = CommonConfig.TrainingTryCount;

            _loader = GetDataLoader();
            _loader.Load();

            _testCtrl = new BasicControllerWithCache(GetSensor(), GetActor()) { StartPosition = _startPosition };
            _testCtrl.DataSourceCtrl = new DataSources.DataSourceCtrl(_loader);

            int totalDataLength = _testCtrl.TotalLength - _startPosition;
            _trainDataLength = (int)(totalDataLength * _testRate);
            _testDataLength = totalDataLength - _trainDataLength;

            // _testCtrl.Normilize(0, 0.1);
            // _testCtrl.Normilize2(0, 0.1);
            // _testCtrl.Normilize3();
            _testCtrl.Normilize4();


            BasicControllerWithCache trainCtrl = (BasicControllerWithCache)_testCtrl.Clone();
            trainCtrl.DataSourceCtrl = new DataSources.DataSourceCtrl(_loader); // TODO
            _ctrlFac = new ControllerFactory(trainCtrl);


            _agentFac = new AgentFactory(_ctrlFac);
            _agentFac.StartPosition = _startPosition;
            _agentFac.TrainDataLength = _trainBlockLength;

            Trainer trainer = new Trainer(_agentFac);

            trainer.CheckCtrl = CreateCheckCtrl();
            trainer.TestName = "";
           
            trainer.RunTestCase();
        }

        private ICheckJob CreateCheckCtrl()
        {
            TrainResultCheckSyncController mainCheckCtrl = new TrainResultCheckSyncController();
            // mainCheckCtrl.Add(new CheckNetworkChangeJob());
            // mainCheckCtrl.Add(new NewUpdateControllerJob(TestCaseName, _testCtrl.GetPacker()));

            // TrainResultCheckAsyncController subCheckCtrl = new TrainResultCheckAsyncController();
            // subCheckCtrl.Add(new UpdateTestCaseJob() 
            IController testCtrl = _ctrlFac.Get();
            testCtrl.DataSourceCtrl = new DataSources.DataSourceCtrl(_loader);
            mainCheckCtrl.Add(new NewUpdateTestCaseJob()
            {
                TestName = TestCaseName + "_" + DateTime.Now.ToString(),

                TestDescription = TestCaseName + "|" +
                    CommonConfig.LoaderParam.ToString() + "|" +
                    "P=" + CommonConfig.PopulationSize + "|" +
                    "Offset=" + CommonConfig.BuyOffset + "," + CommonConfig.SellOffset + "|" +
                    "TrnBlk=" + _trainBlockLength + "," + "TrnCnt=" + _trainTryCount
                    ,

                Controller = testCtrl,
                TrainDataLength = _trainDataLength,
                TestDataLength = _testDataLength,
                StartPosition = _startPosition,
            });

            // mainCheckCtrl.Add(subCheckCtrl);
            mainCheckCtrl.Add(new TrainDataChangeJob(_agentFac, _startPosition, _trainDataLength, _trainBlockLength / 4, _trainTryCount));
            return mainCheckCtrl;

        }

        abstract protected ISensor GetSensor();
        abstract protected IActor GetActor();
        abstract protected BasicTestDataLoader GetDataLoader();
        abstract public string TestCaseName
        {
            get;
        }
    }
    /*
    class NewTestCase : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup();
            senGroup.Add(new RateSensor(16));
            senGroup.Add(new RateSensor(32));
            senGroup.Add(new KDJ_KSensor(32));
            senGroup.Add(new KDJ_DSensor(32));
            senGroup.Add(new KDJ_JSensor(32));
            senGroup.Add(new KDJ_CrossSensor(32));

            return senGroup;
        }

        protected override IActor GetActor()
        {
            BasicActor actor = new BasicActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return  new MTDataLoader("USDJPY", DataTimeType.M5);
        }

        public override string TestCaseName
        {
            get { return "NewTest" + DateTime.Now; }
        }
    }
     */


    class NewTestCase2 : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup();
            senGroup.Add(new RateSensor(64));
            senGroup.Add(new KDJ_KSensor(64));
            senGroup.Add(new KDJ_DSensor(64));
            senGroup.Add(new KDJ_JSensor(64));
            senGroup.Add(new KDJ_CrossSensor(64));

            return senGroup;
        }

        protected override IActor GetActor()
        {
            BasicActor actor = new BasicActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.GetM30OneYear();
        }

        public override string TestCaseName
        {
            get { return "NewTest2" + DateTime.Now; }
        }
    }
    class NewTestCase2Short : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup();
            senGroup.Add(new RateSensor(64));
            senGroup.Add(new KDJ_KSensor(64));
            senGroup.Add(new KDJ_DSensor(64));
            senGroup.Add(new KDJ_JSensor(64));
            senGroup.Add(new KDJ_CrossSensor(64));

            return senGroup;
        }

        protected override IActor GetActor()
        {
            BasicActor actor = new BasicActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.GetM30OneMonth();
        }

        public override string TestCaseName
        {
            get { return "NewTest2Short" + DateTime.Now; }
        }
    }
    class NewTestCase_FWT : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup();
            // senGroup.Add(new RateSensor(64));
            senGroup.Add(new RateFWTSensor(512));
            /*
            senGroup.Add(new KDJ_KSensor(64));
            senGroup.Add(new KDJ_DSensor(64));
            senGroup.Add(new KDJ_JSensor(64));
            senGroup.Add(new KDJ_CrossSensor(64));
            */

            return senGroup;
        }

        protected override IActor GetActor()
        {
            BasicActor actor = new BasicActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.GetM30OneMonth();
        }

        public override string TestCaseName
        {
            get { return "NewTestFWT" + DateTime.Now; }
        }
    }

    class NewTestCase_All : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup();
            senGroup.Add(new RateSensor(64));
            senGroup.Add(new RateFWTSensor(128));
            senGroup.Add(new KDJ_KSensor(64));
            senGroup.Add(new KDJ_DSensor(64));
            senGroup.Add(new KDJ_JSensor(64));
            senGroup.Add(new KDJ_CrossSensor(128));

            return senGroup;
        }

        protected override IActor GetActor()
        {
            BasicActor actor = new BasicActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.GetM30OneMonth();
        }

        public override string TestCaseName
        {
            get { return "NewTestAll" + DateTime.Now; }
        }
    }
    class NewTestCase_All_5Min_Short : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup()
            {
                // new RateSensor(64),
                new RateFWTSensor(256),
                new KDJ_KSensor(64),
                new KDJ_DSensor(64),
                new KDJ_JSensor(64),
                new KDJ_KDCrossSensor(64),
                new KDJ_DJCrossSensor(64),
                new KDJ_KJCrossSensor(64),
                new KDJ_CrossSensor(64)
            };
            return senGroup;
        }

        protected override IActor GetActor()
        {
            BasicActor actor = new BasicActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.GetM30OneMonth();
        }

        public override string TestCaseName
        {
            get { return "NewTestCase_All_30Min_Year" + DateTime.Now; }
        }
    }
    class NewTestCase_All_Switch_5Min_Short : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup()
            {
                // new RateSensor(64),
                // new RateFWTSensor(256),
                new KDJ_KSensor(8),
                new KDJ_DSensor(8),
                new KDJ_JSensor(8),
                new KDJ_KDCrossSensor(8),
                new KDJ_DJCrossSensor(8),
                new KDJ_KJCrossSensor(8),
                new KDJ_CrossSensor(8)
            };
            return senGroup;
        }

        protected override IActor GetActor()
        {
            IActor actor = new StateSwitchActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.GetM30OneMonth();
        }

        public override string TestCaseName
        {
            get { return "NewTestCase_All_Switch_5Min_Short" + DateTime.Now; }
        }
    }
    class NewTestCase_All_Switch_1Day_Short : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup()
            {
                // new RateSensor(64),
                // new RateFWTSensor(256),
                new KDJ_KSensor(8),
                new KDJ_DSensor(8),
                new KDJ_JSensor(8),
                new KDJ_KDCrossSensor(8),
                new KDJ_DJCrossSensor(8),
                new KDJ_KJCrossSensor(8),
                new KDJ_CrossSensor(8)
            };
            return senGroup;
        }

        protected override IActor GetActor()
        {
            IActor actor = new StateSwitchActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.GetD1OneMounth();
        }

        public override string TestCaseName
        {
            get { return "NewTestCase_All_Switch_1Day_Short" + DateTime.Now; }
        }
    }
    class NewTestCase_All_SwitchClose_5Min_Short : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup()
            {
                // new RateSensor(64),
                new RateFWTSensor(256),
                new KDJ_KSensor(64),
                new KDJ_DSensor(64),
                new KDJ_JSensor(64),
                new KDJ_KDCrossSensor(64),
                new KDJ_DJCrossSensor(64),
                new KDJ_KJCrossSensor(64),
                new KDJ_CrossSensor(64)
            };
            return senGroup;
        }

        protected override IActor GetActor()
        {
            IActor actor = new StateSwitchWithCloseActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.GetM30OneMonth();
        }

        public override string TestCaseName
        {
            get { return "NewTestCase_All_SwitchClose_5Min_Short" + DateTime.Now; }
        }
    }
    class NewTestCase_All_1Day_Long : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup();
            senGroup.Add(new RateFWTSensor(128));
            senGroup.Add(new KDJ_KSensor(64));
            senGroup.Add(new KDJ_DSensor(64));
            senGroup.Add(new KDJ_JSensor(64));
            senGroup.Add(new KDJ_CrossSensor(64));

            return senGroup;
        }

        protected override IActor GetActor()
        {
            BasicActor actor = new BasicActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.Get1Day10Year();
        }

        public override string TestCaseName
        {
            get { return "NewTestCase_All_1Day_Long" + DateTime.Now; }
        }
    }
    class NewTestCase_All_1Day_Short : BasicNewTestCase
    {
        protected override ISensor GetSensor()
        {
            SensorGroup senGroup = new SensorGroup();
            senGroup.Add(new RateFWTSensor(128));
            senGroup.Add(new KDJ_KSensor(64));
            senGroup.Add(new KDJ_DSensor(64));
            senGroup.Add(new KDJ_JSensor(64));
            senGroup.Add(new KDJ_CrossSensor(64));

            return senGroup;
        }

        protected override IActor GetActor()
        {
            BasicActor actor = new BasicActor();
            return actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return NewTestDataPacket.Get1DayOneYear();
        }

        public override string TestCaseName
        {
            get { return "NewTestCase_All_1Day_Short"; }
        }
    }


    class NewTestCaseContainer : BasicNewTestCase
    {
        private ISensor _sensor;
        private IActor _actor;
        private BasicTestDataLoader _loader;
        private string _name;

        public NewTestCaseContainer(ISensor sensor, IActor actor, BasicTestDataLoader loader, string name)
        {
            _sensor = sensor;
            _actor = actor;
            _loader = loader;
            _name = name;

        }

        protected override ISensor GetSensor()
        {
            return _sensor;
        }

        protected override IActor GetActor()
        {
            return _actor;
        }

        protected override BasicTestDataLoader GetDataLoader()
        {
            return CommonConfig.LoaderParam.GetLoader();
        }

        public override string TestCaseName
        {
            get
            {
                return _name; 
            }
        }
    }



    class NewTestContainer
    {
        public ISensor Sensor;
        public IActor Actor;
        public BasicTestDataLoader Loader;
        public string Name;
    }


    class NewTestCollecor
    {
        static NewTestContainer[] _testArr;
        static NewTestCollecor()
        {
            _testArr = new NewTestContainer[]
            {
/*
                new NewTestContainer(){ Name="FWT2-1024-4-Recent-M5-1Month", Sensor = new RateFWT2Sensor(1024){ DataCollectLength = 4}, 
                    Actor = new BasicActor()},
                new NewTestContainer(){ Name="FWT2-16384-4-Recent-M5-1Month", Sensor = new RateFWT2Sensor(16384){ DataCollectLength = 4}, 
                    Actor = new BasicActor()},
                new NewTestContainer(){ Name="FWT2-32768-4-Recent-M5-1Month", Sensor = new RateFWT2Sensor(32768){ DataCollectLength = 4}, 
                    Actor = new BasicActor(),   Loader=NewTestDataPacket.GetRecentM5_1Month()},

                new NewTestContainer(){ Name="FWT2-4096-4-Recent-M30-1Month", Sensor = new RateFWT2Sensor(4096){ DataCollectLength = 4}, 
                    Actor = new BasicActor(),   Loader=NewTestDataPacket.GetRecnetM30_1Month()},

                new NewTestContainer(){ Name="FWT2-1024-4-Recent-M30-1Month", Sensor = new RateFWT2Sensor(1024){ DataCollectLength = 4}, 
                    Actor = new BasicActor(),   Loader=NewTestDataPacket.GetRecnetM30_1Month()},
*/

                // Haar
                //------------------------
                new NewTestContainer(){ Name="Haar-64-4", 
                    Sensor = new RateWaveletSensor(64, new HaarWavelet(),4), 
                    Actor = new BasicActor()
                },


                new NewTestContainer(){ Name="Haar-1024-4", 
                    Sensor = new RateWaveletSensor(1024, new HaarWavelet(),4), 
                    Actor = new BasicActor()
                },

                // Daubechies
                //------------------------
                new NewTestContainer(){ Name="Daubechies4-64-4", 
                    Sensor = new RateWaveletSensor(64, new Daubechies4Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Daubechies4-1024-4", 
                    Sensor = new RateWaveletSensor(1024, new Daubechies4Wavelet(),4), 
                    Actor = new BasicActor()
                },

                new NewTestContainer(){ Name="Daubechies8-64-4", 
                    Sensor = new RateWaveletSensor(64, new Daubechies8Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Daubechies8-1024-4", 
                    Sensor = new RateWaveletSensor(1024, new Daubechies8Wavelet(),4), 
                    Actor = new BasicActor()
                },

                new NewTestContainer(){ Name="Daubechies20-64-4", 
                    Sensor = new RateWaveletSensor(64, new Daubechies20Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Daubechies20-1024-4", 
                    Sensor = new RateWaveletSensor(1024, new Daubechies20Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Daubechies20-8192-4", 
                    Sensor = new RateWaveletSensor(8192, new Daubechies20Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Daubechies20-32768-4", 
                    Sensor = new RateWaveletSensor(32768, new Daubechies20Wavelet(),4), 
                    Actor = new BasicActor()
                },


                new NewTestContainer(){ Name="Daubechies20-64-10", 
                    Sensor = new RateWaveletSensor(64, new Daubechies20Wavelet(),10), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Daubechies20-1024-10", 
                    Sensor = new RateWaveletSensor(1024, new Daubechies20Wavelet(),10), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Daubechies20-8192-10", 
                    Sensor = new RateWaveletSensor(8192, new Daubechies20Wavelet(),10), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Daubechies20-32768-10", 
                    Sensor = new RateWaveletSensor(32768, new Daubechies20Wavelet(),10), 
                    Actor = new BasicActor()
                },                
                
                // Legendre
                //------------------------
                new NewTestContainer(){ Name="Legendre6-64-4", 
                    Sensor = new RateWaveletSensor(64, new Legendre6Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Legendre6-1024-4", 
                    Sensor = new RateWaveletSensor(1024, new Legendre6Wavelet(),4), 
                    Actor = new BasicActor()
                },

                new NewTestContainer(){ Name="Legendre6-8192-4", 
                    Sensor = new RateWaveletSensor(8192, new Legendre6Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Legendre6-32768-4", 
                    Sensor = new RateWaveletSensor(32768, new Legendre6Wavelet(),4), 
                    Actor = new BasicActor()
                },


//--------------------------
// CrossTest5
//------------------------
                new NewTestContainer(){ Name="CrossTest5-4,2,9", 
                    Sensor = SensorUtility.GetKDJCrossSensor(4, new int[] {2, 9}, new CrossPartten05()),
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="CrossTest5-4,2,9,31", 
                    Sensor = SensorUtility.GetKDJCrossSensor(4, new int[] {2, 9 ,31}, new CrossPartten05()),
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="CrossTest5-4,2,9,31,95,287", 
                    Sensor = SensorUtility.GetKDJCrossSensor(4, new int[] {2, 6, 9, 31, 95, 287}, new CrossPartten05()),
                    Actor = new BasicActor()
                },

                new NewTestContainer(){ Name="CrossTest5-Switch-4,2,9", 
                    Sensor = SensorUtility.GetKDJCrossSensor(4, new int[] {2, 9}, new CrossPartten05()),
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="CrossTest5-Switch-4,2,9,31", 
                    Sensor = SensorUtility.GetKDJCrossSensor(4, new int[] {2, 9 ,31}, new CrossPartten05()),
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="CrossTest5-Switch-4,2,9,31,95,287", 
                    Sensor = SensorUtility.GetKDJCrossSensor(4, new int[] {2, 6, 9, 31, 95, 287}, new CrossPartten05()),
                    Actor = new StateSwitchActor()
                },

//--------------------------
// Yield
//------------------------
                new NewTestContainer(){ Name="AVE-Yield-Daubechies4-64-4", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(64+1+2), 3)), new Daubechies4Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="Yield-Daubechies4-1024-4", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new RateSensor(1024+1)), new Daubechies4Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="AVE-Yield-Daubechies4-1024-4", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(1024+1+2), 3)), new Daubechies4Wavelet(),4), 
                    Actor = new BasicActor()
                },
                new NewTestContainer(){ Name="AVE-Yield-Daubechies20-8192-10", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(8192+1+2), 3)), new Daubechies20Wavelet(),10), 
                    Actor = new BasicActor()
                },
//--------------------------
// Switch
//------------------------
                new NewTestContainer(){ Name="Switch-Daubechies4-64-4", 
                    Sensor = new RateWaveletSensor(64, new Daubechies4Wavelet(),4), 
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="Switch-Daubechies20-8192-4", 
                    Sensor = new RateWaveletSensor(8192, new Daubechies20Wavelet(),4), 
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="Switch-Daubechies20-32768-10", 
                    Sensor = new RateWaveletSensor(32768, new Daubechies20Wavelet(),10), 
                    Actor = new StateSwitchActor()
                }, 
                new NewTestContainer(){ Name="Switch-Yield-Daubechies4-64-4", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new RateSensor(64+1)), new Daubechies4Wavelet(),4), 
                    Actor = new StateSwitchActor()
                },

                new NewTestContainer(){ Name="Switch-AVE-Yield-Daubechies4-64-4", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(64+1+2), 3)), new Daubechies4Wavelet(),4), 
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="Switch-AVE-Yield-Daubechies20-32768-10", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(32768+1+2), 3)), new Daubechies20Wavelet(),10), 
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="Switch-AVE6-Yield-Daubechies20-32768-10", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(32768+1+5), 6)), new Daubechies20Wavelet(),10), 
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="Switch-AVE9-Yield-Daubechies20-32768-10", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(32768+1+8), 9)), new Daubechies20Wavelet(),10), 
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="Switch-AVE12-Yield-Daubechies20-32768-10", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(32768+1+11), 12)), new Daubechies20Wavelet(),10), 
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="Switch-AVE9-Yield-Daubechies4-64-4", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(64+1+8), 9)), new Daubechies4Wavelet(),4), 
                    Actor = new StateSwitchActor()
                },
                new NewTestContainer(){ Name="Switch-AVE12-Yield-Daubechies4-64-4", 
                    Sensor = new WaveletSensor(new SensorYieldRate(new SensorAveFilter(new RateSensor(64+1+11), 12)), new Daubechies4Wavelet(),4), 
                    Actor = new StateSwitchActor()
                },
//--------------------------
// Switch
//------------------------

                new NewTestContainer(){ Name="Switch-AVE9-Daubechies4-64-4", 
                    Sensor = new WaveletSensor(new SensorAveFilter(new RateSensor(64+8), 9), new Daubechies4Wavelet(),4), 
                    Actor = new StateSwitchActor()
                },


            };
        }
        static public BasicNewTestCase[] GetTest()
        {
            BasicNewTestCase[] testCaseArr = new BasicNewTestCase[_testArr.Length];
            for(int i=0;i<testCaseArr.Length;i++)
            {
                NewTestContainer con = _testArr[i];
                testCaseArr[i] = new NewTestCaseContainer(con.Sensor, con.Actor, con.Loader, con.Name);
            }

            return testCaseArr;
        }
    }

}
