using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util
{
    class DataAnalyzerDesc
    {
        public double Max;
        public double Min;
        public int Count
        {
            get{ return DataList.Count; }
        }
        public List<double> DataList;

        public DataAnalyzerDesc()
        {
            DataList = new List<double>();
        }
        public override string ToString()
        {
            return "C=" + Count + "\tMax=" + Max + "\tMin=" + Min;
        }
    }
    class DataAnalyzer
    {
        private int _sectionNumber;
        public List<double> _dataList;

        public void Init(int sectionNumber)
        {
            _sectionNumber = sectionNumber;
            _dataList = new List<double>();
        }

        public void AddData(double d)
        {
            _dataList.Add(d);
        }
        public void AddData(double[] d)
        {
            foreach (double dat in d)
                _dataList.Add(dat);
        }

        public DataAnalyzerDesc[] GetResult()
        {
            _dataList.Sort();
            double min = _dataList[0];
            double max = _dataList[_dataList.Count - 1];

            double step = (max - min) / _sectionNumber;
            DataAnalyzerDesc[] descArr = new DataAnalyzerDesc[_sectionNumber];
            for(int i=0;i<descArr.Length;i++)
            {
                descArr[i] = new DataAnalyzerDesc() { Min = min + i * step, Max = min + i * step + step };
            }

            foreach(double dat in _dataList)
            {
                for(int i=0;i<descArr.Length;i++)
                {
                    DataAnalyzerDesc desc = descArr[i];
                    if (dat > desc.Max)
                    {
                        continue;
                    }

                    desc.DataList.Add(dat);
                    break;
                }
            }

            return descArr;
        }


    }
}
