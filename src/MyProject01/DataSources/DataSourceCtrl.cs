using MyProject01.DataSources.DataSourceParams;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.DataSources
{
    class DataSourcePack
    {
        private IDataSource _source;
        private IDataSourceParam _param;

        private DataSourceCtrl _ctrl;

        public DataSourcePack(DataSourceCtrl ctrl, IDataSourceParam param)
        {
            this._param = param;
            this._ctrl = ctrl;
        }

        public IDataSource Get()
        {
            if(this._source == null)
                this._source = _param.Create(_ctrl);
            return this._source;
        }

        public bool CompareTo(IDataSourceParam param)
        {
            return param.CompareTo(this._param);
        }


    }

    public class DataSourceCtrl
    {
        private List<DataSourcePack> _packList;
        private RateSet[] _rateArr;

        public RateSet[] SourceLoader
        {
            get { return _rateArr; }
        }

        public DataSourceCtrl(DataLoader loader, double lengthLimit = 1.0)
        {
            int len = (int)(loader.Count * lengthLimit);
            _rateArr = new RateSet[len];
            for (int i = 0; i < _rateArr.Length; i++)
            {
                _rateArr[i] = loader[i];
            }
            _packList = new List<DataSourcePack>();
        }

        public IDataSource Get(IDataSourceParam param)
        {
            foreach (DataSourcePack pack in _packList)
            {
                if (pack.CompareTo(param) == true)
                {
                    return pack.Get();
                }
            }

            // Not found.
            DataSourcePack newPack = new DataSourcePack(this, param);
            _packList.Add(newPack);

            return newPack.Get();
        }

        public int GetIndexByTime(DateTime time)
        {
            int idx;
            for(idx=0;idx<_rateArr.Length;idx++)
            {
                if (_rateArr[idx].Time >= time)
                    break;
            }

            return idx;
        }
    }
}
