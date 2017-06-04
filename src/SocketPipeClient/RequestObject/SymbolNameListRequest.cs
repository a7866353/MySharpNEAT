using SocketTestClient.ConnectionContoller;
using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.RequestObject
{
    class SymbolNameListRequest : IRequest
    {
        public RequestType OrderType = RequestType.SymbolNameListRequest;

        public byte[] GetBytes()
        {
            DataSendBuffer sb = new DataSendBuffer();
            sb.Add((int)OrderType);
            return sb.GetBytes();
        }

        public string[] symbols;
        public void FromBytes(byte[] data, int length)
        {
            DataRcvBuffer rb = new DataRcvBuffer(data, length);
            rb.GetInt(); // Get type
            int count = rb.GetInt();
            symbols = new string[count];
            for (int i = 0; i < count; i++)
            {
                symbols[i] = rb.GetString();
            }
            return;
        }
    }
}
