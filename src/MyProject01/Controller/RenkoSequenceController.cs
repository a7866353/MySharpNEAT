using MyProject01.Controller.Jobs;
using MyProject01.DAO;
using MyProject01.DataSources;
using MyProject01.NeuroNetwork;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{

    class RenkoSequenceController : IController
    {
        private IDataSourceCtrl _dataSourceCtrl;
        private INeuroNetwork _neuroNetwork;
        private ISensor _sensor;
        private IActor _actor;
        private RenkoDataCtrl _renkoDataCtrl;
        private int _currentPosition;
        private int _currentRenkoArrPostion;


        public RenkoParms RenkoParma;
        public int StartPosition = 0;
        private int _sequenceCount;
        private DataBlock[] _inDataCache;
        private DataBlock[] _inDataBuffer;


        public RenkoSequenceController(RenkoParms parms, int count)
        {
            this.RenkoParma = parms;
            _sequenceCount = count;
            _inDataBuffer = new DataBlock[count];
        }

        public IDataSourceCtrl DataSourceCtrl
        {
            set
            {
                _dataSourceCtrl = value;
                NormalRenkoCreator creator = new NormalRenkoCreator() { Step = RenkoParma.Step, StepMargin = RenkoParma.StepMargin };
                _renkoDataCtrl = new RenkoDataCtrl(_dataSourceCtrl.SourceLoader, creator);
                _sensor = CreateSensor(_renkoDataCtrl.ValueArr, RenkoParma.DataBlockLen);
                // _actor = new BasicActor();
                _actor = new StateSwitchWithCloseActor();

                _inDataCache = new DataBlock[_sensor.TotalLength];
                for (int i = _sensor.SkipCount; i < _sensor.TotalLength; i++)
                {
                    DataBlock blk = new DataBlock(_sensor.DataBlockLength);
                    _sensor.Copy(i, blk, 0);
                    _inDataCache[i] = blk;
                }

            }
        }

        public void UpdateNetwork(INeuroNetwork network)
        {
            _neuroNetwork = network;
        }

        public int InputVectorLength
        {
            get { return _sensor.DataBlockLength; }
        }

        public int OutputVectorLength
        {
            get { return _actor.DataLength; }
        }

        public int SkipCount
        {
            get { return _renkoDataCtrl[_sensor.SkipCount].Index; }
        }

        public int TotalLength
        {
            get { return _renkoDataCtrl[_renkoDataCtrl.Count - 1].Index + 1; }
        }

        public int CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                int i;
                for (i = 0; i < _renkoDataCtrl.RenkoBlockArr.Length; i++)
                {
                    RenkoBlock blk = _renkoDataCtrl.RenkoBlockArr[i];
                    if (value < blk.Index)
                    {
                        _currentPosition = _renkoDataCtrl[i].Index;
                        break;
                    }
                    else if (value == blk.Index)
                    {
                        _currentPosition = value;
                        break;
                    }
                }

                if (i == _renkoDataCtrl.RenkoBlockArr.Length)
                {
                    _currentPosition = _renkoDataCtrl[i - 1].Index + 1;
                }

                _currentRenkoArrPostion = i;
            }
        }

        public Util.RateSet CurrentRateSet
        {
            get
            {
                return _renkoDataCtrl[CurrentRenkoBlockIndex].RateSet;
            }
        }

        public MarketActions GetAction()
        {
            for(int i=0;i<_sequenceCount;i++)
            {
                _inDataBuffer[i] = _inDataCache[CurrentRenkoBlockIndex - i];
            }
            DataBlock output = _neuroNetwork.Compute(_inDataBuffer);

            MarketActions result = _actor.GetAction(output);
            return result;

        }

        public Util.RateSet GetRateSet(DateTime time)
        {
            throw new NotImplementedException();
        }

        public Util.RateSet GetRateSet(int pos)
        {
            throw new NotImplementedException();
        }

        public IController Clone()
        {
            RenkoSequenceController ctrl = (RenkoSequenceController)MemberwiseClone();
            // ctrl._sensor = _sensor.Clone();
            // ctrl._actor = _actor.Clone();
            // ctrl._normalizerArray = _normalizerArray.Clone() as Normalizer[];

            _currentPosition = Math.Max(_sensor.SkipCount, StartPosition);
            return ctrl;
        }

        private int CurrentRenkoBlockIndex
        {
            get
            {
#if false
                int idx = _currentPosition;
                RenkoBlock blk = _renkoDataCtrl.RenkoBlockArr[i];
                if (idx == blk.Index)
                {
                    return idx;
                }
                for (int i = 0; i < _renkoDataCtrl.RenkoBlockArr.Length; i++)
                {
                    
                    if (_currentPosition <= blk.Index)
                    {
                        return i;
                    }

                }

                return _renkoDataCtrl.RenkoBlockArr.Length - 1;
#else
                return _currentRenkoArrPostion;
#endif
            }
        }

        private ISensor CreateSensor(double[] buffer, int len)
        {
            return new RevertSensor(new StateNormallzeSensor(new BlockSensor(buffer, len + 1)));
        }


    }

    class RenkoSequenceTestCase : ITestCase
    {
        private ControllerFactory _ctrlFac;
        private RenkoSequenceController _testCtrl;
        private BasicTestDataLoader _loader;
        private AgentFactory _agentFac;

        private double _testRate = 0.7;
        private int _startPosition = 50000;
        private int _trainBlockLength = 32;
        private int _trainTryCount = 2;

        private int _trainDataLength;
        private int _testDataLength;

        private RenkoParms _renkoParms;
        private int _sequenceCount;

        public string Name
        {
            get { return "SeqRenkoSwitch" + _renkoParms.ToString() + "Seq:" + _sequenceCount; }
        }

        public string Description
        {
            get { return "SeqRenko Test: " + _renkoParms.ToString(); }
        }
        public RenkoSequenceTestCase(RenkoParms parms, int count)
        {
            _renkoParms = parms;
            _sequenceCount = count;
        }
        public void Run()
        {
            // Config Server IP
            DataBaseAddress.SetIP(CommonConfig.ServerIP);

            _trainBlockLength = CommonConfig.TrainingDataBlockLength;
            _trainTryCount = CommonConfig.TrainingTryCount;
             
            _loader = GetDataLoader();
            _loader.Load();

            _testCtrl = new RenkoSequenceController(_renkoParms, _sequenceCount);
            _testCtrl.DataSourceCtrl = new DataSources.LoaderSourceCtrl(_loader);

            int totalDataLength = _testCtrl.TotalLength - _startPosition;
            _trainDataLength = (int)(totalDataLength * _testRate);
            _testDataLength = totalDataLength - _trainDataLength;


            RenkoSequenceController trainCtrl = (RenkoSequenceController)_testCtrl.Clone();
            trainCtrl.DataSourceCtrl = new DataSources.LoaderSourceCtrl(_loader);
            _ctrlFac = new ControllerFactory(trainCtrl);


            _agentFac = new AgentFactory(_ctrlFac);
            _agentFac.StartPosition = _startPosition;
            // _agentFac.TrainDataLength = _trainBlockLength;
            _agentFac.TrainDataLength = _trainDataLength;

            Trainer trainer = new Trainer(_agentFac);
            // RbfTrainer trainer = new RbfTrainer(_agentFac);

            trainer.CheckCtrl = CreateCheckCtrl();
            trainer.TestName = "";

            trainer.RunTestCase();
        }

        private BasicTestDataLoader GetDataLoader()
        {
            return CommonConfig.LoaderParam.GetLoader();
        }

        private ICheckJob CreateCheckCtrl() 
        {
            TrainResultCheckSyncController mainCheckCtrl = new TrainResultCheckSyncController();
            // mainCheckCtrl.Add(new CheckNetworkChangeJob());
            // mainCheckCtrl.Add(new NewUpdateControllerJob(TestCaseName, _testCtrl.GetPacker()));

            // TrainResultCheckAsyncController subCheckCtrl = new TrainResultCheckAsyncController();
            IController testCtrl = _ctrlFac.Get();
            testCtrl.DataSourceCtrl = new DataSources.LoaderSourceCtrl(_loader);
            mainCheckCtrl.Add(new NewUpdateTestCaseJob()
            {
                TestName = Name + "_" + DateTime.Now.ToString(),

                TestDescription = Name + "|" +
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
            mainCheckCtrl.Add(new TrainDataChangeJob(_agentFac, _startPosition, _trainDataLength, _trainBlockLength / 8, _trainTryCount));
            return mainCheckCtrl;

        }

    }

    class RenkoSequenceTestCaseList
    {
        static public List<ITestCase> GetTest()
        {
            int[] blkLen = new int[] { 64, 32, 16, 8 };
            double[] StepArr = new double[] { 1.0, 0.5, 0.1, 0.05 };
            double[] StepMarginArr = new double[] { 0.2, 0.1, 0.05 };

            List<ITestCase> testCaseList = new List<ITestCase>()
            {
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.1, StepMargin=0.05}, 4),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.1, StepMargin=0.05}, 8),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.1, StepMargin=0.05}, 16),

                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.1, StepMargin=0.05}, 4),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.1, StepMargin=0.05}, 8),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.1, StepMargin=0.05}, 16),

                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.05, StepMargin=0.02}, 4),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.05, StepMargin=0.02}, 8),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.05, StepMargin=0.02}, 16),

                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=4, Step=0.5, StepMargin=0.1}, 4),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=4, Step=0.5, StepMargin=0.1}, 8),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=4, Step=0.5, StepMargin=0.1}, 16),

                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.5, StepMargin=0.1}, 4),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.5, StepMargin=0.1}, 8),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.5, StepMargin=0.1}, 16),

                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.5, StepMargin=0.1}, 4),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.5, StepMargin=0.1}, 8),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.5, StepMargin=0.1}, 16),

                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=32, Step=0.5, StepMargin=0.1}, 4),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=32, Step=0.5, StepMargin=0.1}, 8),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=32, Step=0.5, StepMargin=0.1}, 16),
                new RenkoSequenceTestCase(new RenkoParms(){ DataBlockLen=32, Step=0.5, StepMargin=0.1}, 32),

            };

            return testCaseList;
        }


    }

}
