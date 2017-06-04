using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.RequestObject
{
    enum RequestType
    {
        None = 0,
        TestRequest,
        SendOrderRequest,
        SendOrderResult,
        RateDataRequest,
        RateDataIndicate,
        SymbolNameListRequest,
        SymbolNameListResult,
        RateByCount,
        RateByCount_Result,
        RateByTime,
    }
    class Request
    {
        public static IRequest FromBytes(byte[] data, int length)
        {
            IRequest res = null;
            RequestType type = GetType(data);
            switch(type)
            {
                case RequestType.RateDataRequest:
                    res = new RateByTimeRequest();
                    break;
                case RequestType.TestRequest:
                    res = new TestRequest();
                    break;
                case RequestType.RateDataIndicate:
                    res = new RateDataIndicateRequest();
                    break;
                case RequestType.RateByCount_Result:
                    res = new RateDataIndicateRequest();
                    break;
            }
            if (res == null)
                return null;
            res.FromBytes(data, length);
            return res;
        }
        private static RequestType GetType(byte[] data)
        {
            RequestType type = (RequestType)BitConverter.ToInt32(data,0);
            return type;
        }
    }
}
