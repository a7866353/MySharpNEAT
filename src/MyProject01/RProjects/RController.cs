using MyProject01.Controller;
using MyProject01.DataSources;
using MyProject01.NeuroNetwork;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.RProjects
{
    class RController : IController
    {
        private int _totalLength;
        private int _currentPosition;

        private IDataSourceCtrl _dataSourceCtrl;
        private INeuroNetwork _neuroNetwork;
        private RNetwork _network;
        

        public int StartPosition = 0;
        
        public RController()
        {

        }

        public IDataSourceCtrl DataSourceCtrl
        {
            set
            {
                _dataSourceCtrl = value;
                if (_network != null)
                    _network.IsInited = false;

                _totalLength = _dataSourceCtrl.SourceLoader.Length;
            }
            get
            {
                return _dataSourceCtrl;
            }
        }

        public void UpdateNetwork(NeuroNetwork.INeuroNetwork network)
        {
            _network = (RNetwork)network;
            _network.IsInited = false;
        }

        public int InputVectorLength
        {
            get { return _network.InputNum; }
        }

        public int OutputVectorLength
        {
            get { return _network.OutputNum; }
        }

        public int SkipCount
        {
            get { return StartPosition; }
        }

        public int TotalLength
        {
            get { return _totalLength; }
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


        public RateSet CurrentRateSet
        {
            get { return GetRateSet(_currentPosition); }
        }

        public MarketActions GetAction()
        {
            if( _network.IsInited == false)
                _network.SetData(_dataSourceCtrl.SourceLoader, StartPosition, TotalLength-StartPosition);

            // TODO: RateSet和Action可能存在对不上的情况。现在+1之后，得到的结果最佳，但是问题原因不明。
#if false
            int offset = CurrentPosition - StartPosition + 1;
            if (offset >= (TotalLength - StartPosition))
                offset = (TotalLength - StartPosition - 1);
#else
            int offset = CurrentPosition - StartPosition;
#endif
            return _network.GetAction(offset);
        }

        public RateSet GetRateSet(DateTime time)
        {
            return GetRateSet(GetIndexByTime(time));
        }

        public RateSet GetRateSet(int pos)
        {
            return _dataSourceCtrl.SourceLoader[pos];
        }

        public IController Clone()
        {
            return (IController)MemberwiseClone();
        }

        public int GetIndexByTime(DateTime time)
        {
            return _dataSourceCtrl.GetIndexByTime(time);
        }



        public BasicControllerPacker GetPacker()
        {
            return _network.GetPacker();
        }
    }

    [Serializable]
    public class RControllerPacker : BasicControllerPacker
    {
        private byte[] _netData;
        private int _inNum;
        private int _outNum;
        public RControllerPacker(byte[] netData, int inNum, int outNum)
        {
            _netData = netData;
            _inNum = inNum;
            _outNum = outNum;
        }

        public override IController GetController()
        {
            RNetwork net = new RNetwork(_netData, _inNum, _outNum);
            IController ctrl = new RController();
            ctrl.UpdateNetwork(net);
                
            return ctrl;
        }
    }

  
}
