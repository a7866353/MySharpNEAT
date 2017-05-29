using MyProject01.DataSources.DataSourceParams;
using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MyProject01.DataSources
{
    class DataSourcePack
    {
        private IDataSource _source;
        private IDataSourceParam _param;

        private IDataSourceCtrl _ctrl;

        public DataSourcePack(IDataSourceCtrl ctrl, IDataSourceParam param)
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
        public void Set(IDataSource src)
        {
            this._source = src;
        }
        public bool CompareTo(IDataSourceParam param)
        {
            return param.CompareTo(this._param);
        }


    }

    public interface IDataSourceCtrl
    {
        RateSet[] SourceLoader { get; }
        IDataSource Get(IDataSourceParam param);
        int GetIndexByTime(DateTime time);
    }
    public class LoaderSourceCtrl : IDataSourceCtrl
    {
        private List<DataSourcePack> _packList;
        private RateSet[] _rateArr;

        public RateSet[] SourceLoader
        {
            get { return _rateArr; }
        }

        public LoaderSourceCtrl(DataLoader loader, double lengthLimit = 1.0)
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

    public class CSVSourceCtrl : IDataSourceCtrl
    {
        enum CSVIndexType
        {
            Date = 0,
            Open,
            High,
            Low,
            Close,
            Max
        }
        class NormItem
        {
            List<double> _dataList;
            CSVItem _CSVItem;

            public NormItem(CSVItem item)
            {
                _CSVItem = item;
                _dataList = new List<double>();
            }
            public void AddValue()
            {
                try
                {
                    _dataList.Add(double.Parse(_CSVItem.Value));
                }
                catch(Exception e)
                {
#if false
                    if (_dataList.Count == 0)
                        _dataList.Add(0);
                    else
                        _dataList.Add(_dataList[_dataList.Count-1]);
#endif
                    _dataList.Add(double.NaN);
                }
            }
            public string Name
            {
                get { return _CSVItem.Name; }
            }
            public List<double> DataList
            {
                get { return _dataList; }
            }
        }
        private List<DataSourcePack> _packList;
        private RateSet[] _rateArr;
        private string[] ReadCSVLine(StreamReader stream)
        {
            return stream.ReadLine().ToLower().Split(',');
        }
        public RateSet[] SourceLoader
        {
            get { return _rateArr; }
        }

        public CSVSourceCtrl(string path)
        {
            StreamReader sr = new StreamReader(File.OpenRead(path));
            string[] spName = new string[]
            {
                "date",
                "open",
                "high",
                "low",
                "close"
            };
            CSVItem[] spItem = new CSVItem[spName.Length];
            List<NormItem> normItem = new List<NormItem>();

            CSVReader reader = new CSVReader();
            reader.Open(path);

            List<RateSet> rateSetList = new List<RateSet>();

            // Read Title
            string[] titleArr = reader.FirstLine;
            for(int i=0;i<titleArr.Length;i++)
            {
                bool isFind = false;
                string name = titleArr[i].ToLower();
                if (string.IsNullOrWhiteSpace(name) == true)
                    continue;
                for(int spIdx=0; spIdx<spName.Length;spIdx++)
                {
                    if( name.CompareTo(spName[spIdx]) == 0 )
                    {
                        isFind = true;
                        spItem[spIdx] = new CSVItem(reader, spName[spIdx], i);
                        break;
                    }
                }
                if( isFind == true )
                    continue;
                normItem.Add(new NormItem(new CSVItem(reader, name, i)));
            }

            // Read Data
            while (reader.NextLine() == true)
            {
                RateSet set = new RateSet();
                if (spItem[0] != null)
                    set.Time = DateTime.Parse(spItem[0].Value);
                if (spItem[1] != null)
                    set.Open = double.Parse(spItem[1].Value);
                if (spItem[2] != null)
                    set.High = double.Parse(spItem[2].Value);
                if (spItem[3] != null)
                    set.Low = double.Parse(spItem[3].Value);
                if (spItem[4] != null)
                    set.Close = double.Parse(spItem[4].Value);
                rateSetList.Add(set);

                foreach(NormItem item in normItem)
                {
                    item.AddValue();
                }
            }

            _rateArr = rateSetList.ToArray();
            _packList = new List<DataSourcePack>();
            foreach(NormItem item in normItem)
            {
                DataSourcePack newPack = new DataSourcePack(
                    this, 
                    new StringSourceParam(item.Name)
                    );

                newPack.Set(
                    new StringDataSource(
                        this,
                        new DataBlock(item.DataList.ToArray(), false)
                        )
                    );
                _packList.Add(newPack);
            }
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
            for (idx = 0; idx < _rateArr.Length; idx++)
            {
                if (_rateArr[idx].Time >= time)
                    break;
            }

            return idx;
        }
    }

}
