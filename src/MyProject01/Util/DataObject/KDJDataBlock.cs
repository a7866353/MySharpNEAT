using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util.DataObject
{
    public class KDJDataBlock : BasicDataBlock
    {
        private double[] _rateArr;
        private double[] _kArr;
        private double[] _dArr;
        private double[] _jArr;

        private int _aveRange = 9;
        private int _m1 = 3;
        private int _m2 = 3;

        public KDJDataBlock(DataLoader loader, int startIndex, int length, int blockLength) 
            : base(loader, startIndex, length, blockLength)
        {
            _blockLen = blockLength;

            _rateArr = new double[_dataBufferLength - _aveRange + 1];
            _kArr = new double[_rateArr.Length];
            _dArr = new double[_rateArr.Length];
            _jArr = new double[_rateArr.Length];

            for (int i = 0; i < _rateArr.Length; i++)
            {
                int index = i+_aveRange-1;
                double maxValue,minValue,rsv;
                FindMaxMin(index, out maxValue, out minValue);
                _rateArr[i] = this[index].Close;

                rsv = (this[index].Close - minValue) / (maxValue - minValue);
                if( i == 0)
                {
                    _kArr[i] = rsv;
                    _dArr[i] = _kArr[i];
                    _jArr[i] = 3 * _kArr[i] - 2 * _dArr[i];
                }
                else
                {
                    _kArr[i] = (1 * rsv + (_m1-1) * _kArr[i-1]) / _m1;
                    _dArr[i] = (1*_kArr[i] + (_m2-1) * _dArr[i-1]) / _m2;
                    _jArr[i] = 3*_kArr[i] - 2*_dArr[i];
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
        public override int BlockCount
        {
            get { return _dataBufferLength - _aveRange + 1 - _blockLength + 1; }
        }

        public override int BlockLength
        {
            get { return _blockLength * 4; }
        }

        public override double GetRate(int i)
        {
            return this[i + _aveRange - 1 + _blockLength - 1].Close;
        }

        public override DateTime GetDate(int i)
        {
            return this[i + _aveRange - 1 + _blockLength - 1].Time;
        }

        public override int Copy(double[] array, int index)
        {
            if (array == null || array.Length < 4 * _blockLength)
                throw (new Exception("Error!"));

            Array.Copy(_rateArr, index, array, 0 * _blockLength, _blockLength);
            Array.Copy(_kArr, index, array, 1 * _blockLength, _blockLength);
            Array.Copy(_dArr, index, array, 2 * _blockLength, _blockLength);
            Array.Copy(_jArr, index, array, 3 * _blockLength, _blockLength);

            return _blockLength;
        }

        public override BasicDataBlock GetNewBlock(int startIndex, int length)
        {
            KDJDataBlock res = new KDJDataBlock(_loader, _startIndex + startIndex, _blockLength + length - 1, _blockLength);
            return res;
        }

        public override BasicDataBlock Clone()
        {
            return (BasicDataBlock)MemberwiseClone();
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
}
