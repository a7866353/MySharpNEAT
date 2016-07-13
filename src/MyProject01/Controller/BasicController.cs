using MyProject01.DataSources;
using MyProject01.DataSources.DataSourceParams;
using MyProject01.NeuroNetwork;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Controller
{
    public interface IController
    {
        DataSourceCtrl DataSourceCtrl { set; }
        void UpdateNetwork(INeuroNetwork network);

        int InputVectorLength { get; }
        int OutputVectorLength { get; }
        int SkipCount { get; }
        int TotalLength { get; }
        int CurrentPosition { get; set; }
        RateSet CurrentRateSet { get; }

        MarketActions GetAction();

        RateSet GetRateSet(DateTime time);
        RateSet GetRateSet(int pos);

        IController Clone();
    }

    public class ControllerFactory
    {
        public IController BaseController;

        public ControllerFactory(IController baseController)
        {
            this.BaseController = baseController;
        }
        public IController Get()
        {
            IController ctrl = BaseController.Clone();
            return ctrl;
        }
        public void Free(IController ctrl)
        {
            // TODO Nothing
        }
    }
   
    class BasicController : IController
    {
        private ISensor _sensor;
        private IActor _actor;
        private Normalizer[] _normalizerArray;

        private DataBlock _inData;
        private int _currentPosition;

        private DataSourceCtrl _dataSourceCtrl;
        private INeuroNetwork _neuroNetwork;
        private IDataSource _dataSource;
        public BasicController(ISensor sensor, IActor actor)
        {
            _sensor = sensor;
            _actor = actor;
            _currentPosition = 0;
            _inData = new DataBlock(NetworkInputVectorLength);
        }

        public int SkipCount
        {
            get { return _sensor.SkipCount; }
        }
        public int TotalLength
        {
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

        public Normalizer[] NormalizerArray
        {
            set { _normalizerArray = value; }
        }

        public MarketActions GetAction()
        {
            _sensor.Copy(_currentPosition, _inData, 0);
            
            // Normalize
            for (int i = 0; i < _inData.Length; i++)
            {
                _inData[i] = _normalizerArray[i].Convert(_inData[i]);
            }

            DataBlock output = _neuroNetwork.Compute(_inData);

            MarketActions result = _actor.GetAction(output);
            return result;
        }

        public void UpdateNetwork(INeuroNetwork network)
        {
            _neuroNetwork = network;
        }

        public int NetworkInputVectorLength
        {
            get { return _sensor.DataBlockLength; }
        }

        public int NetworkOutputVectorLenth
        {
            get { return _actor.DataLength; }
        }

        public IController Clone()
        {
            BasicController ctrl = (BasicController)MemberwiseClone();
            ctrl._sensor = _sensor.Clone();
            ctrl._actor = _actor.Clone();
            // ctrl._normalizerArray = _normalizerArray.Clone() as Normalizer[];

            _inData = new DataBlock(NetworkInputVectorLength);
            _currentPosition = _sensor.SkipCount;
            return ctrl;
        }
        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                _dataSourceCtrl = value; 
                _sensor.DataSourceCtrl = value;

                RateDataSourceParam param = new RateDataSourceParam(5);
                _dataSource = _dataSourceCtrl.Get(param);
                _currentPosition = _sensor.SkipCount;
            }
            get
            {
                return _dataSourceCtrl;
            }
        }

        public ControllerPacker GetPacker()
        {
            ControllerPacker packer = new ControllerPacker(_sensor, _actor, _neuroNetwork, _normalizerArray);
            return packer;
        }

        public void Normilize(double middleValue, double limit)
        {
            FwtDataNormalizer norm = new FwtDataNormalizer();
            DataBlock buffer = new DataBlock(NetworkInputVectorLength);


            _sensor.Copy(SkipCount, buffer, 0);
            norm.Init(buffer.Data, middleValue, limit);

            for (int i = SkipCount+1; i < TotalLength;i++ )
            {
                _sensor.Copy(i, buffer, 0);
                norm.Set(buffer.Data);
            }

            _normalizerArray = norm.NromalizerArray;
        }

        public int GetIndexByTime(DateTime time)
        {
            return _dataSourceCtrl.GetIndexByTime(time);
        }

        public RateSet CurrentRateSet
        {
            get { return GetRateSet(_currentPosition); }
        }

        public RateSet GetRateSet(DateTime time)
        {
            return GetRateSet(GetIndexByTime(time));
        }

        public RateSet GetRateSet(int pos)
        {
            return _dataSourceCtrl.SourceLoader[pos];
        }


        public int InputVectorLength
        {
            get { return _sensor.DataBlockLength; }
        }

        public int OutputVectorLength
        {
            get { return _actor.DataLength; }
        }
    }

    class BasicControllerWithCache : IController
    {
        private ISensor _sensor;
        private IActor _actor;
        private Normalizer[] _normalizerArray;
        
        private int _currentPosition;

        private DataSourceCtrl _dataSourceCtrl;
        private INeuroNetwork _neuroNetwork;
        private IDataSource _dataSource;

        private DataBlock[] _inDataCache;

        public int StartPosition = 0;
        public BasicControllerWithCache(ISensor sensor, IActor actor)
        {
            _sensor = sensor;
            _actor = actor;
            _currentPosition = 0;

            _inDataCache = null;
        }

        public int SkipCount
        {
            get { return _sensor.SkipCount; }
        }
        public int TotalLength
        {
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

        public Normalizer[] NormalizerArray
        {
            set { _normalizerArray = value; }
        }

        public virtual MarketActions GetAction()
        {
            DataBlock inData = _inDataCache[_currentPosition];

            DataBlock output = _neuroNetwork.Compute(inData);

            MarketActions result = _actor.GetAction(output);
            return result;
        }

        public void UpdateNetwork(INeuroNetwork network)
        {
            _neuroNetwork = network;
        }

        public IController Clone()
        {
            BasicControllerWithCache ctrl = (BasicControllerWithCache)MemberwiseClone();
            ctrl._sensor = _sensor.Clone();
            ctrl._actor = _actor.Clone();
            // ctrl._normalizerArray = _normalizerArray.Clone() as Normalizer[];

            _currentPosition = Math.Max(_sensor.SkipCount , StartPosition);
            return ctrl;
        }
        public DataSourceCtrl DataSourceCtrl
        {
            set
            {
                _dataSourceCtrl = value;
                _sensor.DataSourceCtrl = value;

                RateDataSourceParam param = new RateDataSourceParam(5);
                _dataSource = _dataSourceCtrl.Get(param);
                _currentPosition = Math.Max(_sensor.SkipCount, StartPosition);
            }
            get
            {
                return _dataSourceCtrl;
            }
        }

        public ControllerPacker GetPacker()
        {
            ControllerPacker packer = new ControllerPacker(_sensor, _actor, _neuroNetwork, _normalizerArray);
            return packer;
        }

        public void Normilize(double middleValue, double limit)
        {
            FwtDataNormalizer norm = new FwtDataNormalizer();
            DataBlock buffer = new DataBlock(InputVectorLength);

            int startPos = Math.Max(_sensor.SkipCount, StartPosition);

            _sensor.Copy(startPos, buffer, 0);
            norm.Init(buffer.Data, middleValue, limit);

            for (int i = startPos + 1; i < TotalLength; i++)
            {
                _sensor.Copy(i, buffer, 0);
                norm.Set(buffer.Data);
            }

            _normalizerArray = norm.NromalizerArray;


            // Create cache data
            _inDataCache = new DataBlock[TotalLength];
            for (int i = startPos; i < TotalLength; i++)
            {
                buffer = new DataBlock(InputVectorLength);
                _sensor.Copy(i, buffer, 0);
                for (int j = 0; j < buffer.Length; j++)
                {
                    buffer[j] = _normalizerArray[j].Convert(buffer[j]);
                }

                _inDataCache[i] = buffer;
            }

        }

        public int GetIndexByTime(DateTime time)
        {
            return _dataSourceCtrl.GetIndexByTime(time);
        }

        public RateSet CurrentRateSet
        {
            get { return GetRateSet(_currentPosition); }
        }

        public RateSet GetRateSet(DateTime time)
        {
            return GetRateSet(GetIndexByTime(time));
        }

        public RateSet GetRateSet(int pos)
        {
            return _dataSourceCtrl.SourceLoader[pos];
        }
        public int InputVectorLength
        {
            get { return _sensor.DataBlockLength; }
        }

        public int OutputVectorLength
        {
            get { return _actor.DataLength; }
        }
    }

}
