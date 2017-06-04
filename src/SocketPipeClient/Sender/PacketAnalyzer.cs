using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.Sender
{
    class ReceivePacket
    {
        private byte[] _buffer;
        private int _index;
        public ReceivePacket(int length)
        {
            _buffer = new byte[length];
        }
        public byte[] GetBuffer()
        {
            if (_index == _buffer.Length)
                return _buffer;
            else
                return null;
        }

        public int SetData(byte[] buffer, int startIndex, int length)
        {
            int copyCount = Math.Min(length, (_buffer.Length - _index));
            Array.Copy(buffer, startIndex, _buffer, _index, copyCount);
            _index += copyCount;
            return copyCount;
        }
    }
    class PacketAnalyzer
    {
        private byte[] _sendBuffer;

        private byte _headerMark = 0xAA;
        private byte[] _receiveBuffer;
        enum Status
        {
            FinishReveice,
            Reveice,
        }
        private Status _reveiceStatus;
        private int _packetLength;
        private byte[] _targetBuffer;
        private int _bufferLength;
        private int _index;
        private ReceivePacket _rcvPacket;


        public PacketAnalyzer()
        {
            _sendBuffer = new byte[5];
            _sendBuffer[0] = _headerMark;

            _reveiceStatus = Status.FinishReveice;
        }

        public void SetBufferLength(int length)
        {
            byte[] lengthArr = BitConverter.GetBytes(length);
            Array.Copy(lengthArr, 0, _sendBuffer, 1, lengthArr.Length);
        }

        public byte[] GetHeader()
        {
            return _sendBuffer;
        }

        public byte[] SetBuffer(byte[] buffer, int length)
        {
            byte[] resultBuffer = null;
            _bufferLength = length;
            _targetBuffer = buffer;
            _index = 0;
            while(true)
            {
                if( _index >= _bufferLength )
                    break;
                resultBuffer = CheckBuffer();
                if (resultBuffer != null)
                    break;
            }

            return resultBuffer;
        }

        private byte[] CheckBuffer()
        {
            byte[] resultBuffer = null;
            switch (_reveiceStatus)
            {
                case Status.FinishReveice:
                    {
                        CheckHeaer();
                        if (_index < _bufferLength)
                        {
                            _reveiceStatus = Status.Reveice;
                            _packetLength = BitConverter.ToInt32(_targetBuffer, _index);
                            _rcvPacket = new ReceivePacket(_packetLength);
                            _index += 4;
                        }
                        break;
                    }

                case Status.Reveice:
                    {
                        CheckPacket();
                        resultBuffer = _rcvPacket.GetBuffer();
                        if (resultBuffer != null)
                        {
                            _reveiceStatus = Status.FinishReveice;
                            _rcvPacket = null;
                        }
                        break;
                    }
            }
            return resultBuffer;
        }

        private void CheckHeaer()
        {
            while(true)
            {
                if (_index >= _bufferLength)
                    break;
                byte flag = _targetBuffer[_index];
                _index++;
                if (flag == _headerMark)
                    break;
            }
        }
        private void CheckPacket()
        {
            int copyCount = _rcvPacket.SetData(_targetBuffer, _index, (_bufferLength - _index));
            _index += copyCount;
        }
         

    }
}
