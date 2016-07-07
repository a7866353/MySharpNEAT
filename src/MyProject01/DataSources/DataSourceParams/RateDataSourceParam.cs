using MyProject01.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.DataSources.DataSourceParams
{
    class FixDataSource : IDataSource
    {
        private RateSet[] _rateSet;
        private DataBlock _dataBuffer;
        private double _lengthLimit;

        public FixDataSource(DataSourceCtrl ctrl, double lengthLimit = 1.0)
        {
            _rateSet = ctrl.SourceLoader;
            _dataBuffer = new DataBlock((int)(_rateSet.Length * lengthLimit));
            for (int i = 0; i < _dataBuffer.Length; i++)
            {
                _dataBuffer[i] = _rateSet[i].Close;
            }
        }

        public void Copy(int index, DataBlock buffer, int offset, int length)
        {
            DataBlock.Copy(_dataBuffer, index - length + 1, buffer, offset, length);
        }
        public int TotalLength
        {
            get
            {
                return _dataBuffer.Length;
            }
        }
        public RateSet this[int index]
        {
            get 
            {
                if (index>=_rateSet.Length)
                {
                    index = index;
                }
                return _rateSet[index]; 
            }
        }


        public int SkipCount
        {
            get { return 0; }
        }
    }

    [Serializable]
    class RateDataSourceParam: IDataSourceParam
    {
        private int _timeFrame;
        private double _lengthLimit;

        public int TimeFrame { get { return _timeFrame; } }

        public RateDataSourceParam(int timeFrame, double countLimit = 1.0)
        {
            _timeFrame = timeFrame;
            _lengthLimit = countLimit;
        }

        public bool CompareTo(IDataSourceParam param)
        {
            if (this.GetType() != param.GetType())
                return false;

            RateDataSourceParam inParam = (RateDataSourceParam)param;
            do
            {
                if (inParam._timeFrame != this._timeFrame)
                    return false;
                if (inParam._lengthLimit != this._lengthLimit)
                    return false;

            } while (false);

            return true;
        }

     
        public IDataSource Create(DataSourceCtrl ctrl)
        {
            return new FixDataSource(ctrl, _lengthLimit);
        }

    }


    class KDJDataSource : IDataSource
    {
        private int _skipCount;
        private RateSet[] _rateArr;
        private double[] _kArr;
        private double[] _dArr;
        private double[] _jArr;

        private int _aveRange = 9;
        private int _m1 = 3;
        private int _m2 = 3;

        public KDJDataSource(DataSourceCtrl loader, int aveRange, int m1, int m2) 
        {
            _rateArr = loader.SourceLoader;

            _aveRange = aveRange;
            _skipCount = aveRange - 1;
            _m1 = m1;
            _m2 = m2;


            _kArr = new double[_rateArr.Length];
            _dArr = new double[_rateArr.Length];
            _jArr = new double[_rateArr.Length];

            double lastRsv = 0.5;
            for (int i = _skipCount; i < _rateArr.Length; i++)
            {
                double maxValue, minValue, rsv;
                FindMaxMin(i, out maxValue, out minValue);

                rsv = (_rateArr[i].Close - minValue) / (maxValue - minValue);
                if (maxValue == minValue)
                    rsv = lastRsv;

                lastRsv = rsv;

                if (i == 0)
                {
                    _kArr[i] = rsv;
                    _dArr[i] = _kArr[i];
                    _jArr[i] = 3 * _kArr[i] - 2 * _dArr[i];
                }
                else
                {
                    _kArr[i] = (1 * rsv + (_m1 - 1) * _kArr[i - 1]) / _m1;
                    _dArr[i] = (1 * _kArr[i] + (_m2 - 1) * _dArr[i - 1]) / _m2;
                    _jArr[i] = 3 * _kArr[i] - 2 * _dArr[i];
                }

                if (_kArr[i] > 1)
                    _kArr[i] = 1;
                else if (_kArr[i] < 0)
                    _kArr[i] = 0;

                if (_dArr[i] > 1)
                    _dArr[i] = 1;
                else if (_dArr[i] < 0)
                    _dArr[i] = 0;

                if (_jArr[i] > 1)
                    _jArr[i] = 1;
                else if (_jArr[i] < 0)
                    _jArr[i] = 0;
            }
        }

        public void Copy(int index, DataBlock buffer, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public int TotalLength
        {
            get { return _rateArr.Length; }
        }

        public RateSet this[int index]
        {
            get { return _rateArr[index]; }
        }

        public int SkipCount
        {
            get { return _skipCount; }
        }

        public void CopyK(int index, DataBlock buffer, int offset, int length)
        {
            CopyData(_kArr, index, buffer, offset, length);
        }
        public void CopyD(int index, DataBlock buffer, int offset, int length)
        {
            CopyData(_dArr, index, buffer, offset, length);
        }
        public void CopyJ(int index, DataBlock buffer, int offset, int length)
        {
            CopyData(_jArr, index, buffer, offset, length);
        }

        public double[] KArr
        {
            get { return _kArr; }
        }
        public double[] DArr
        {
            get { return _dArr; }
        }
        public double[] JArr
        {
            get { return _jArr; }
        }
        private void CopyData(double[] data, int index, DataBlock buffer, int offset, int length)
        {
            Array.Copy(data, index - length + 1, buffer.Data, offset, length);
        }

        private void FindMaxMin(int index, out double maxValue, out double minValue)
        {
            maxValue = this[index].High;
            minValue = this[index].Low;
            for(int i=1;i<_aveRange;i++)
            {
                if (this[index-i].High > maxValue)
                    maxValue = this[index - i].High;
                if (this[index - i].Low < minValue)
                    minValue = this[index - i].Low;
            }
        }


    }

    [Serializable]
    class KDJDataSourceParam : IDataSourceParam
    {
        public int AveRange = 9;
        public int M1 = 3;
        public int M2 = 3;

        public KDJDataSourceParam()
        {

        }

        public bool CompareTo(IDataSourceParam param)
        {
            if (this.GetType() != param.GetType())
                return false;

            KDJDataSourceParam inParam = (KDJDataSourceParam)param;
            do
            {
                if (inParam.AveRange != this.AveRange)
                    return false;
                if (inParam.M1 != this.M1)
                    return false;
                if (inParam.M2 != this.M2)
                    return false;

            } while (false);

            return true;
        }

        public IDataSource Create(DataSourceCtrl ctrl)
        {
            KDJDataSource src = new KDJDataSource(ctrl, AveRange, M1, M2);
            return src;
        }
    }

    class MADataSource : IDataSource
    {
        private int _aveCount;
        private int _skipCount;
        private RateSet[] _rateArr;
        private double[] _kArr;
        private double[] _dArr;
        private double[] _jArr;

        private int _aveRange = 9;
        private int _m1 = 3;
        private int _m2 = 3;

        public MADataSource(DataSourceCtrl loader, int aveCount)
        {
            _aveCount = aveCount;
        }

        public void Copy(int index, DataBlock buffer, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public int TotalLength
        {
            get { return _rateArr.Length; }
        }

        public RateSet this[int index]
        {
            get { return _rateArr[index]; }
        }

        public int SkipCount
        {
            get { return _skipCount; }
        }

        public void CopyK(int index, DataBlock buffer, int offset, int length)
        {
            CopyData(_kArr, index, buffer, offset, length);
        }
        public void CopyD(int index, DataBlock buffer, int offset, int length)
        {
            CopyData(_dArr, index, buffer, offset, length);
        }
        public void CopyJ(int index, DataBlock buffer, int offset, int length)
        {
            CopyData(_jArr, index, buffer, offset, length);
        }

        public double[] KArr
        {
            get { return _kArr; }
        }
        public double[] DArr
        {
            get { return _dArr; }
        }
        public double[] JArr
        {
            get { return _jArr; }
        }
        private void CopyData(double[] data, int index, DataBlock buffer, int offset, int length)
        {
            Array.Copy(data, index - length + 1, buffer.Data, offset, length);
        }

        private void FindMaxMin(int index, out double maxValue, out double minValue)
        {
            maxValue = this[index].High;
            minValue = this[index].Low;
            for (int i = 1; i < _aveRange; i++)
            {
                if (this[index - i].High > maxValue)
                    maxValue = this[index - i].High;
                if (this[index - i].Low < minValue)
                    minValue = this[index - i].Low;
            }
        }


    }


    [Serializable]
    class MADataSourceParam : IDataSourceParam
    {
        public int Count = 30;

        public MADataSourceParam()
        {

        }

        public bool CompareTo(IDataSourceParam param)
        {
            if (this.GetType() != param.GetType())
                return false;

            MADataSourceParam inParam = (MADataSourceParam)param;
            do
            {
                if (inParam.Count != this.Count)
                    return false;

            } while (false);

            return true;
        }

        public IDataSource Create(DataSourceCtrl ctrl)
        {
            MADataSource src = new MADataSource(ctrl, Count);
            return src;
        }
    }

}
