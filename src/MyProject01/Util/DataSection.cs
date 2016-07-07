using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject01.Util
{
    class DataSectionEnumerator : IEnumerator<double>
    {
        private DataSection _section;
        private int _index;
        private bool _isEnd;
        public DataSectionEnumerator(DataSection section)
        {
            _section = section;
            _index = 0;
            _isEnd = false;
        }


        public double Current
        {
            get { return _section[_index]; }
        }

        public void Dispose()
        {
            ;
        }

        object IEnumerator.Current
        {
            get { return _section[_index]; }
        }

        public bool MoveNext()
        {
            if (_isEnd == true)
                return false;
            _index++;
            if (_index >= _section.Length - 1)
                _isEnd = true;
            return true;
        }

        public void Reset()
        {
            _index = 0;
        }
    }
    class DataSection
    {
        private double[] _data;
        private int _startIndex;
        private int _length;
        public DataSection(double[] data, int startIndex, int length)
        {
            _data = data;
            _startIndex = startIndex;
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }

        public double this[int index]
        {
            get 
            {
                /*
                if (index < 0 || index >= _startIndex + _length)
                    throw (new Exception("数组越界！"));
                 */ 
                return _data[index + _startIndex]; 
            }
            set
            {
                /*
                if (index < 0 || index >= _startIndex + _length)
                    throw (new Exception("数组越界！"));
                */
                _data[index + _startIndex] = value; 
            }
        }

        public void CopyTo(DataSection sec)
        {
            if (_length > sec.Length)
                throw (new Exception("数组越界！"));

            for(int i=0;i<_length;i++)
            {
                sec[i] = this[i];
            }
        }
        public void CopyTo(double[] dst)
        {
            CopyTo(dst, 0); 
        }
        public void CopyTo(double[] dst, int startIndex)
        {
            Array.Copy(_data, _startIndex, dst, startIndex, _length);
        }

    }
}
