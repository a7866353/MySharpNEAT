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
        private int _skipCount;
        private int _totalLength;
        private int _currentPosition;

        private IDataSourceCtrl _dataSourceCtrl;
        private INeuroNetwork _neuroNetwork;

        public int StartPosition = 0;

        public IDataSourceCtrl DataSourceCtrl
        {
            set
            {
                _dataSourceCtrl = value;
            }
            get
            {
                return _dataSourceCtrl;
            }
        }

        public void UpdateNetwork(NeuroNetwork.INeuroNetwork network)
        {
            throw new NotImplementedException();
        }

        public int InputVectorLength
        {
            get { throw new NotImplementedException(); }
        }

        public int OutputVectorLength
        {
            get { throw new NotImplementedException(); }
        }

        public int SkipCount
        {
            get { return _skipCount; }
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public int GetIndexByTime(DateTime time)
        {
            return _dataSourceCtrl.GetIndexByTime(time);
        }





    }
}
