using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.RequestObject
{
    enum SendOrderResult
    {
        None,
        OK,
        NG,
    }

    class SendOrderRequest : IRequest
    {
        public RequestType OrderType = RequestType.SendOrderRequest;
        public string SymbolName;
        public Cmd OrderCmd;
        public int MagicNumber;

        private int _result;

        public byte[] GetBytes()
        {
            // Clear Result
            _result = 0;

            DataSendBuffer sb = new DataSendBuffer();
            sb.Add((int)OrderType);
            sb.Add(SymbolName);
            sb.Add((int)OrderCmd);
            sb.Add(MagicNumber);

            return sb.GetBytes();
        }

        public void FromBytes(byte[] data, int length)
        {
            DataRcvBuffer rb = new DataRcvBuffer(data, length);
            
            // Get type
            rb.GetInt();
            _result = rb.GetInt();
        }

        public int Result
        {
            get { return _result; }
        }

        public enum Cmd
        {
            Nothing = 0,
            Buy = 1,
            Sell = 2,
            Close = 3,
        }
    }
}
