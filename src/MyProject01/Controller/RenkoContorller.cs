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

        public RenkoDataCtrl(RateSet[] rateSets)
        {
            NormalRenkoCreator creator = new NormalRenkoCreator();
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

        public DataSources.DataSourceCtrl DataSourceCtrl
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

        public DataSources.DataSourceCtrl DataSourceCtrl
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

        public DataSources.DataSourceCtrl DataSourceCtrl
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

    class RenkoContorller : IController
    {
        private DataSourceCtrl _dataSourceCtrl;
        private INeuroNetwork _neuroNetwork;
        private ISensor _sensor;
        private IActor _actor;
        private RenkoDataCtrl _renkoDataCtrl;
        private int _currentPosition;

        public int DataBlockLen = 32;
        public int StartPosition = 0;
        private DataBlock[] _inDataCache;

        public DataSourceCtrl DataSourceCtrl
        {
            set 
            {
                _dataSourceCtrl = value;
                _renkoDataCtrl = new RenkoDataCtrl(_dataSourceCtrl.SourceLoader);
                _sensor = CreateSensor(_renkoDataCtrl.ValueArr, DataBlockLen);
                _actor = new BasicActor();

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
            get { return _renkoDataCtrl[_sensor.SkipCount].Index; }
        }

        public int TotalLength
        {
            get { return _renkoDataCtrl[_renkoDataCtrl.Count-1].Index+1; }
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
                for( i=0;i<_renkoDataCtrl.RenkoBlockArr.Length; i++)
                {
                    RenkoBlock blk = _renkoDataCtrl.RenkoBlockArr[i];
                    if(value < blk.Index)
                    {
                        _currentPosition = _renkoDataCtrl[i].Index;
                        break;
                    }
                    else if(value == blk.Index)
                    {
                        _currentPosition = value;
                        break;
                    }
                }

                if (i == _renkoDataCtrl.RenkoBlockArr.Length)
                {
                    _currentPosition = _renkoDataCtrl[i-1].Index+1;
                }
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
            DataBlock output = _neuroNetwork.Compute(_inDataCache[CurrentRenkoBlockIndex]);

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

        private int CurrentRenkoBlockIndex
        {
            get
            {
                for (int i = 0; i < _renkoDataCtrl.RenkoBlockArr.Length; i++)
                {
                    RenkoBlock blk = _renkoDataCtrl.RenkoBlockArr[i];
                    if (_currentPosition <= blk.Index)
                    {
                        return i;
                    }

                }

                return _renkoDataCtrl.RenkoBlockArr.Length - 1;
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

        private double _testRate = 0.7;
        private int _startPosition = 50000;
        private int _trainBlockLength = 32;
        private int _trainTryCount = 2;

        private int _trainDataLength;
        private int _testDataLength;

        public string Name
        {
            get { return "Renko"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public void Run()
        {
            // Config Server IP
            DataBaseAddress.SetIP(CommonConfig.ServerIP);

            _trainBlockLength = CommonConfig.TrainingDataBlockLength;
            _trainTryCount = CommonConfig.TrainingTryCount;

            _loader = GetDataLoader();
            _loader.Load();

            _testCtrl = new RenkoContorller();
            _testCtrl.DataSourceCtrl = new DataSources.DataSourceCtrl(_loader);

            int totalDataLength = _testCtrl.TotalLength - _startPosition;
            _trainDataLength = (int)(totalDataLength * _testRate);
            _testDataLength = totalDataLength - _trainDataLength;


            RenkoContorller trainCtrl = (RenkoContorller)_testCtrl.Clone();
            trainCtrl.DataSourceCtrl = new DataSources.DataSourceCtrl(_loader); // TODO
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
            // subCheckCtrl.Add(new UpdateTestCaseJob() 
            IController testCtrl = _ctrlFac.Get();
            testCtrl.DataSourceCtrl = new DataSources.DataSourceCtrl(_loader);
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
            mainCheckCtrl.Add(new TrainDataChangeJob(_agentFac, _startPosition, _trainDataLength, _trainBlockLength / 4, _trainTryCount));
            return mainCheckCtrl;

        }

    }

    class RenkoTestCaseList
    {
        static public List<ITestCase> GetTest()
        {
            List<ITestCase> testCaseList = new List<ITestCase>()
            {
                new RenkoTestCase(),

            };

            return testCaseList;
        }


    }

}
