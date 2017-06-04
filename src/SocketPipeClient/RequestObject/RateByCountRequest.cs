using SocketTestClient.ConnectionContoller;
using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.RequestObject
{
    class RateByCountRequest : IRequest
    {
        public RequestType OrderType = RequestType.RateByCount;

        public string SymbolName;
        public int TimeFrame;
        public DateTime StartTime;
        public int Count;

        public byte[] GetBytes()
        {
            DataSendBuffer sb = new DataSendBuffer();
            sb.Add((int)OrderType);
            sb.Add(SymbolName);
            sb.Add(TimeFrame);
            sb.Add(StartTime);
            sb.Add(Count);

            return sb.GetBytes();
        }


        public RateInfo[] RateInfoArray;
        public void FromBytes(byte[] data, int length)
        {
            RateDataIndicateRequest idc = new RateDataIndicateRequest();
            idc.FromBytes(data, length);
            RateInfoArray = idc.RateInfoArray;
        }
    }
}
