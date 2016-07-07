using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProject01.Util
{
    class SlidingFilter
    {
        private double[] _dataArr;
        private int _length;
        private int _index;
        private bool _isInit;
        public SlidingFilter(int cnt)
        {
            _length = cnt;
            _index = 0;
            _dataArr = new double[cnt];
            _isInit = false;
        }
        public double Add(double value)
        {
            if (false == _isInit)
            {
                for (int i = 0; i < _length; i++)
                    _dataArr[i] = value;
                _isInit = true;
            }
            else
            {
                _dataArr[_index] = value;
            }
            if (++_index >= _length)
                _index = 0;

            return _dataArr.Sum() / _length;
        }
    }
}
