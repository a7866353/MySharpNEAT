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
    class RenkoBlock
    {
        public int Index;
        public RateSet RateSet;
        public double Value;

        public override string ToString()
        {
            return Value.ToString() + "|" + Index.ToString();
        }
    } 

    class NormalRenkoCreator
    {
        public double Step = 0.5;
        public double StepMargin = 0.05;

        private int _index;
        private RateSet[] _rateSets;
        private double _currentRate;
        public RenkoBlock[] Convert(RateSet[] rateSets)
        {
            double result;
            _rateSets = rateSets;
            List<RenkoBlock> blkList = new List<RenkoBlock>();

            _index = 0;
            _currentRate = rateSets[0].Close;
            _currentRate = _currentRate - (_currentRate % Step);

            blkList.Add(CreateBlock());

            for (_index = 0; _index < rateSets.Length; _index++)
            {
                result = Check();
                if (result == _currentRate)
                    continue;

                _currentRate = result;
                blkList.Add(CreateBlock());

#if false
                if(blkList.Count>1)
                {
                    double diff = blkList[blkList.Count-1].RateSet.Close - blkList[blkList.Count-2].RateSet.Close;
                    if (Math.Abs(diff) > Step*2)
                        System.Console.WriteLine("error?");
                }
#endif
                _index--;
            }


            return blkList.ToArray();
        }

        private RenkoBlock CreateBlock()
        {
            RenkoBlock blk = new RenkoBlock();
            blk.RateSet = _rateSets[_index];
            blk.Index = _index;
            blk.Value = _currentRate;
            return blk;
        }
        private double Check()
        {
            double rate = _rateSets[_index].Close;
            if (rate > _currentRate + Step + StepMargin)
                return _currentRate + Step;
            else if (rate < _currentRate - StepMargin)
                return _currentRate - Step;
            else
                return _currentRate;
        }
    }

    class RenkoDataCtrl
    {
        private RenkoBlock[] _renkoBlockArr;
        private double[] _valueArr;

        public int Count
        {
            get { return _renkoBlockArr.Length; }
        }

        public RenkoBlock[] RenkoBlockArr
        {
            get { return _renkoBlockArr; }
        }

        public double[] ValueArr
        {
            get { return _valueArr; }
        }
        public RenkoBlock this[int index]
        {
            get { return _renkoBlockArr[index]; }
        }

        public void Copy(int index, int length, double[] buffer, int offset)
        {
            Array.Copy(_valueArr, index, buffer, offset, length);
        }

        public RenkoDataCtrl(RateSet[] rateSets, NormalRenkoCreator creator)
        {
            _renkoBlockArr = creator.Convert(rateSets);

            _valueArr = new double[Count];
            for(int i=0;i<Count;i++)
            {
                _valueArr[i] = _renkoBlockArr[i].Value;
            }
        }
    }

    #region ISensor

    class BlockSensor : ISensor
    {
        private double[] _buffer;
        private int _blockLen;
        public BlockSensor(double[] buffer, int blockLen)
        {
            _buffer = buffer;
            _blockLen = blockLen;
        }

        public int SkipCount
        {
            get { return _blockLen - 1; }
        }

        public int TotalLength
        {
            get { return _buffer.Length; }
        }

        public int DataBlockLength
        {
            get { return _blockLen; }
        }

        public DataSources.IDataSource DataSource
        {
            get { throw new NotImplementedException(); }
        }

        public DataSources.IDataSourceCtrl DataSourceCtrl
        {
            set { throw new NotImplementedException(); }
        }

        public int Copy(int index, DataBlock buffer, int offset)
        {
            Array.Copy(_buffer, index - SkipCount, buffer.Data, offset, _blockLen);
            return _blockLen;
        }

        public void Init()
        {
            // throw new NotImplementedException();
        }

        public ISensor Clone()
        {
            return new BlockSensor(_buffer, _blockLen);
        }
    }

    class StateNormallzeSensor : ISensor
    {
        private ISensor _source;
        private DataBlock _buffer;
        public StateNormallzeSensor(ISensor sen)
        {
            _source = sen;
            _buffer = new DataBlock(_source.DataBlockLength);
        }

        public int SkipCount
        {
            get { return _source.SkipCount; }
        }

        public int TotalLength
        {
            get { return _source.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _source.DataBlockLength-1; }
        }

        public DataSources.IDataSource DataSource
        {
            get { throw new NotImplementedException(); }
        }

        public DataSources.IDataSourceCtrl DataSourceCtrl
        {
            set { throw new NotImplementedException(); }
        }

        public int Copy(int index, DataBlock buffer, int offset)
        {
            _source.Copy(index, _buffer, offset);
            for(int i=0;i<DataBlockLength;i++)
            {
                if (_buffer[i + 1] - _buffer[i] > 0)
                    buffer[offset + i] = 1.0;
                else
                    buffer[offset + i] = 0.0;
            }

            return DataBlockLength;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public ISensor Clone()
        {
            return new StateNormallzeSensor(_source);
        }
    }

    class RevertSensor : ISensor
    {
        private ISensor _source;
        private DataBlock _buffer;
        public RevertSensor(ISensor sen)
        {
            _source = sen;
            _buffer = new DataBlock(_source.DataBlockLength);
        }
        public int SkipCount
        {
            get { return _source.SkipCount; }
        }

        public int TotalLength
        {
            get { return _source.TotalLength; }
        }

        public int DataBlockLength
        {
            get { return _source.DataBlockLength; }
        }

        public DataSources.IDataSource DataSource
        {
            get { throw new NotImplementedException(); }
        }

        public DataSources.IDataSourceCtrl DataSourceCtrl
        {
            set { throw new NotImplementedException(); }
        }

        public int Copy(int index, DataBlock buffer, int offset)
        {
            _source.Copy(index, _buffer, offset);
            for(int i=0;i<DataBlockLength;i++)
            {
                buffer[offset + i] = _buffer[DataBlockLength - i - 1];
            }
            return DataBlockLength;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public ISensor Clone()
        {
            return new RevertSensor(_source);
        }
    }

    #endregion

    class RenkoParms
    {
        public int DataBlockLen = 32;
        public double Step = 0.5;
        public double StepMargin = 0.05;

        public override string ToString()
        {
            return "Len=" + DataBlockLen + "Step=" + Step + "Marg=" + StepMargin;
        }
    }
    class RenkoContorller : IController
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
        private DataBlock[] _inDataCache;


        public RenkoContorller(RenkoParms parms)
        {
            this.RenkoParma = parms;
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
                    _sensor.Copy(i,blk,0);
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
            // get { return _renkoDataCtrl[_sensor.SkipCount].Index; }
            get { return _sensor.SkipCount; }
        }

        public int TotalLength
        {
            // get { return _renkoDataCtrl[_renkoDataCtrl.Count-1].Index+1; }
            get { return _sensor.TotalLength; }
        }

        public int CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                _currentPosition = value;
            }
        }

        public Util.RateSet CurrentRateSet
        {
            get 
            {
                return _renkoDataCtrl[_currentPosition].RateSet;
            }
        }

        public MarketActions GetAction()
        {
            DataBlock output = _neuroNetwork.Compute(_inDataCache[_currentPosition]);

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
            RenkoContorller ctrl = (RenkoContorller)MemberwiseClone();
            // ctrl._sensor = _sensor.Clone();
            // ctrl._actor = _actor.Clone();
            // ctrl._normalizerArray = _normalizerArray.Clone() as Normalizer[];

            _currentPosition = Math.Max(_sensor.SkipCount, StartPosition);
            return ctrl;
        }

        public int SearchForPositionByIndex(int index)
        {
            int i;
            for (i = 0; i < _renkoDataCtrl.RenkoBlockArr.Length; i++)
            {
                RenkoBlock blk = _renkoDataCtrl.RenkoBlockArr[i];
                if (index < blk.Index)
                {
                    break;
                }
                else if (index == blk.Index)
                {
                    break;
                }
            }

            if (i == _renkoDataCtrl.RenkoBlockArr.Length)
            {
                i--;
            }

            return i;
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
            return new RevertSensor(new StateNormallzeSensor(new BlockSensor(buffer, len+1)));
        }


        

    }

    class RenkoTestCase : ITestCase
    {
        private ControllerFactory _ctrlFac;
        private RenkoContorller _testCtrl;
        private BasicTestDataLoader _loader;
        private AgentFactory _agentFac;
        private Trainer _trainer;

        private double _testRate = 0.7;
        private int _startPosition = 50000;
        private int _trainBlockLength = 512;
        private int _trainTryCount = 2;

        private int _startDataPosition;
        private int _trainDataLength;
        private int _testDataLength;

        private RenkoParms _renkoParms;

        public string Name
        {
            get { return "RenkoSwitch" + _renkoParms.ToString(); }
        }

        public string Description
        {
            get { return "Renko Test: " + _renkoParms.ToString(); }
        }
        public RenkoTestCase(RenkoParms parms)
        {
            _renkoParms = parms;
        }
        public void Run()
        {
            // Config Server IP
            DataBaseAddress.SetIP(CommonConfig.ServerIP);

            _trainBlockLength = CommonConfig.TrainingDataBlockLength;
            _trainTryCount = CommonConfig.TrainingTryCount;

            _loader = GetDataLoader();
            _loader.Load();

            _testCtrl = new RenkoContorller(_renkoParms);
            _testCtrl.DataSourceCtrl = new DataSources.LoaderSourceCtrl(_loader);

            _startDataPosition = _testCtrl.SearchForPositionByIndex(_startPosition);
            int trainEndPosition = _testCtrl.SearchForPositionByIndex(_startPosition + (int)((_loader.Count - _startPosition) * _testRate));
            int testEndPosition = _testCtrl.SearchForPositionByIndex(_loader.Count);

            _trainDataLength = trainEndPosition - _startDataPosition;
            _testDataLength = testEndPosition - _startDataPosition - _trainDataLength;

            if (_trainBlockLength == 0)
                _trainBlockLength = _trainDataLength;
            else if (_trainBlockLength > _trainDataLength)
                _trainBlockLength = _trainDataLength;

            RenkoContorller trainCtrl = (RenkoContorller)_testCtrl.Clone();
            trainCtrl.DataSourceCtrl = new DataSources.LoaderSourceCtrl(_loader); // TODO
            _ctrlFac = new ControllerFactory(trainCtrl);


            _agentFac = new AgentFactory(_ctrlFac);
            _agentFac.StartPosition = _startDataPosition;
            // _agentFac.TrainDataLength = _trainBlockLength;
            _agentFac.TrainDataLength = _trainBlockLength;

            _trainer = new Trainer(_agentFac);
            _trainer.PopulationSize = CommonConfig.PopulationSize;
            _trainer.SpecieCount = _trainer.PopulationSize / 10;
            _trainer.ComplexityThreshold = 100;
            // RbfTrainer trainer = new RbfTrainer(_agentFac);

            _trainer.CheckCtrl = CreateCheckCtrl();
            _trainer.TestName = "";

            _trainer.RunTestCase();
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
                    "P=" + _trainer.PopulationSize + "|" +
                    "S=" + _trainer.SpecieCount +"|" +
                    "C=" + _trainer.ComplexityThreshold + "|" +
                    "Offset=" + CommonConfig.BuyOffset + "," + CommonConfig.SellOffset + "|" +
                    "TrnBlk=" + _trainBlockLength + "," + "TrnCnt=" + _trainTryCount
                    ,

                Controller = testCtrl,
                TrainDataLength = _trainDataLength,
                TestDataLength = _testDataLength,
                StartPosition = _startDataPosition,
            });

            // mainCheckCtrl.Add(subCheckCtrl);
            mainCheckCtrl.Add(new TrainDataChangeJob(_agentFac, _startDataPosition, _trainBlockLength, _trainBlockLength, _trainTryCount));
            return mainCheckCtrl;

        }

    }

    class RenkoTestCaseList
    {
        static public List<ITestCase> GetTest()
        {
            int[] blkLen = new int[] { 64, 32, 16, 8 };
            double[] StepArr = new double[] { 1.0, 0.5, 0.1, 0.05 };
            double[] StepMarginArr = new double[] { 0.2, 0.1, 0.05 };

            List<ITestCase> testCaseList = new List<ITestCase>()
            {
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=64, Step=0.5, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=32, Step=0.5, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.5, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.5, StepMargin=0.05}),

                new RenkoTestCase(new RenkoParms(){ DataBlockLen=64, Step=1.0, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=32, Step=1.0, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=16, Step=1.0, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=8, Step=1.0, StepMargin=0.05}),


                new RenkoTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.5, StepMargin=0.0}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.5, StepMargin=0.1}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.5, StepMargin=0.2}),

                new RenkoTestCase(new RenkoParms(){ DataBlockLen=128, Step=0.1, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=64, Step=0.1, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=32, Step=0.1, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.1, StepMargin=0.05}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.1, StepMargin=0.05}),

                new RenkoTestCase(new RenkoParms(){ DataBlockLen=64, Step=0.05, StepMargin=0.02}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=32, Step=0.05, StepMargin=0.02}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.05, StepMargin=0.02}),

                new RenkoTestCase(new RenkoParms(){ DataBlockLen=512, Step=0.5, StepMargin=0.1}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=256, Step=0.5, StepMargin=0.1}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=128, Step=0.5, StepMargin=0.1}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=64, Step=0.5, StepMargin=0.1}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=32, Step=0.5, StepMargin=0.1}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=16, Step=0.5, StepMargin=0.1}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=8, Step=0.5, StepMargin=0.1}),
                new RenkoTestCase(new RenkoParms(){ DataBlockLen=4, Step=0.5, StepMargin=0.1}),

            };

            return testCaseList;
        }


    }

}
