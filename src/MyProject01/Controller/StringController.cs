using MyProject01.Controller.Jobs;
using MyProject01.DAO;
using MyProject01.DataSources;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    abstract class BasicStringTestCase : ITestCase
    {
        private ControllerFactory _ctrlFac;
        private BasicControllerWithCache _testCtrl;
        private AgentFactory _agentFac;
        private IDataSourceCtrl _dataSrcCtrl;

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


            _trainTryCount = CommonConfig.TrainingTryCount;
            _dataSrcCtrl = new DataSources.CSVSourceCtrl(GetLoaderPath());

            _testCtrl = new BasicControllerWithCache(GetSensor(), GetActor()) { StartPosition = _startPosition };
            _testCtrl.DataSourceCtrl = _dataSrcCtrl;

            int totalDataLength = _testCtrl.TotalLength - _startPosition;
            _trainDataLength = (int)(totalDataLength * _testRate);
            _testDataLength = totalDataLength - _trainDataLength;

            _trainBlockLength = CommonConfig.TrainingDataBlockLength;
            if (_trainBlockLength == 0)
                _trainBlockLength = _trainDataLength;
            else if (_trainBlockLength > _trainDataLength)
                _trainBlockLength = _trainDataLength;

            _testCtrl.Normilize_Array(0, 0.5);
            // _testCtrl.Normilize2(0, 0.1);
            // _testCtrl.Normilize3();
            // _testCtrl.Normilize_None();

            BasicControllerWithCache trainCtrl = (BasicControllerWithCache)_testCtrl.Clone();
            trainCtrl.DataSourceCtrl = _dataSrcCtrl;
            _ctrlFac = new ControllerFactory(trainCtrl);


            _agentFac = new AgentFactory(_ctrlFac);
            _agentFac.StartPosition = _startPosition;
            _agentFac.TrainDataLength = _trainBlockLength;

            Trainer trainer = new Trainer(_agentFac);
            // RbfTrainer trainer = new RbfTrainer(_agentFac);

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
            testCtrl.DataSourceCtrl = _dataSrcCtrl;
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
            mainCheckCtrl.Add(new TrainDataChangeJob(_agentFac, _startPosition, _trainDataLength, _trainBlockLength, _trainTryCount));
            return mainCheckCtrl;

        }

        abstract protected ISensor GetSensor();
        abstract protected IActor GetActor();
        abstract protected string GetLoaderPath();
        abstract public string TestCaseName
        {
            get;
        }

        public string Name
        {
            get { return TestCaseName + ", " + GetLoaderPath(); }
        }

        public string Description
        {
            get { return TestCaseName + ", " + GetLoaderPath(); }
        }
    }
    class StringTestCaseContainer : BasicStringTestCase
    {
        private ISensor _sensor;
        private IActor _actor;
        private string _loaderPath;
        private string _name;

        public StringTestCaseContainer(ISensor sensor, IActor actor, string loaderPath, string name)
        {
            _sensor = sensor;
            _actor = actor;
            _loaderPath = loaderPath;
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
        protected override string GetLoaderPath()
        {
            return _loaderPath;
        }

        public override string TestCaseName
        {
            get
            {
                return _name;
            }
        }

    }


    class StringTestCaseList
    {
        static public List<ITestCase> GetTest()
        {
            int[] blkLen = new int[] { 64, 32, 16, 8 };
            double[] StepArr = new double[] { 1.0, 0.5, 0.1, 0.05 };
            double[] StepMarginArr = new double[] { 0.2, 0.1, 0.05 };

            List<ITestCase> testCaseList = new List<ITestCase>()
            {
                new StringTestCaseContainer
                    (
                        new SensorGroup()
                        { 
                            new RateSensor(3),
                            new RateSensor(3),
                            new RateSensor(3),
                            new RateSensor(3),
                            new RateSensor(3)
                        },
                        new BasicActor(),
                        "output.csv",
                        "TestStringTestCase"
                    )
            };


            return testCaseList;
        }
    }
}
