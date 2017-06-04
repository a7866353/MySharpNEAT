using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.RequestObject
{
    class DataSendBuffer
    {
        private List<byte[]> _bufferList;

        public DataSendBuffer()
        {
            _bufferList = new List<byte[]>();
        }

        public void Add(int data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            _bufferList.Add(buffer);
        }

        public void Add(double data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            _bufferList.Add(buffer);
        }

        public void Add(bool data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            _bufferList.Add(buffer);
        }

        public void Add(string data)
        {
            byte[] str = ASCIIEncoding.Default.GetBytes(data);
            byte[] buffer = new byte[str.Length + 1];
            str.CopyTo(buffer, 0);
            buffer[buffer.Length - 1] = 0;
            _bufferList.Add(buffer);
        }
        public void Add(DateTime data)
        {
            // byte[] buffer = BitConverter.GetBytes(data.ToFileTime());
            string dateStr = data.ToString("yyyy.MM.dd HH:mm:ss");
            Add(dateStr);

        }
        public byte[] GetBytes()
        {
            int length = 0;
            foreach(byte[] arr in _bufferList)
            {
                length += arr.Length;
            }

            byte[] buffer = new byte[length];
            int index = 0;
            foreach(byte[] arr in _bufferList)
            {
                arr.CopyTo(buffer, index);
                index += arr.Length;
            }
            return buffer;
        }

        public void Clear()
        {
            _bufferList.Clear();
        }



    }

    class DataRcvBuffer
    {
        private byte[] _buffer;
        private int _dataLength;
        private int _index;

        public DataRcvBuffer(byte[] data, int length)
        {
            _buffer = data;
            _dataLength = length;
        }

        public string GetString()
        {
            int length = 0;
            int pos = 0;
            for (; _index + pos < _buffer.Length; pos++)
            {
                if (_buffer[_index + pos] == 0)
                    break;
            }

            if (pos == 0)
                return "";
            else if (pos + _index >= _buffer.Length)
                length = _buffer.Length - _index;
            else
                length = pos+1;
            string str = Encoding.ASCII.GetString(_buffer, _index, pos);

            _index += length;
            return str;
        }
        public long GetLong()
        {
            int length = sizeof(long);
            long ret = BitConverter.ToInt64(_buffer, _index);
            _index += length;
            return ret;
        }
        public int GetInt()
        {
            int length = sizeof(int);
            int ret = BitConverter.ToInt32(_buffer, _index);
            _index += length;
            return ret;
        }

        public double GetDouble()
        {
            int length = sizeof(double);
            double ret = BitConverter.ToDouble(_buffer, _index);
            _index += length;
            return ret;
        }

        public bool GetBool()
        {
            int length = sizeof(bool);
            bool ret = BitConverter.ToBoolean(_buffer, _index);
            _index += length;
            return ret;
        }

        public void Reset()
        {
            _index = 0;
        }

    }


}
