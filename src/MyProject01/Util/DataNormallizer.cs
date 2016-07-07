using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util
{
    class DataNormallizer
    {
        private double[] _data;
        private int _startIndex;
        private int _length;

        private double _dataMaxValue;
        private double _dataMinValue;

        private double _dataScale;
        private double _dataOffset;

        public void Set(double[] array, int startIndex, int length)
        {
            _data = array;
            _startIndex = startIndex;
            _length = length;

        }

        public void DataValueAdjust(double min, double max)
        {
            _dataMaxValue = _dataMinValue = _data[_startIndex];
            for (int i = 0; i < _length; i++)
            {
                if (_data[_startIndex + i] > _dataMaxValue)
                    _dataMaxValue = _data[_startIndex + i];
                else if (_data[_startIndex + i] < _dataMinValue)
                    _dataMinValue = _data[_startIndex + i];
            }

            _dataScale = (max - min) / (_dataMaxValue - _dataMinValue);

            _dataOffset = (_dataMaxValue - _dataMinValue) * max / (max - min) - _dataMaxValue;


            Normallize(_dataOffset, _dataScale);
        }
        private double DataConv(double value, double offset, double scale)
        {
            return (value + offset) * scale;
        }

        private void Normallize(double offset, double scale)
        {
            _dataOffset = offset;
            _dataScale = scale;
            for (int i = 0; i < _length; i++)
            {
                _data[_startIndex + i] = DataConv(_data[_startIndex + i], offset, scale);
            }
        }

    }
}
