using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.RequestObject
{
    class RateInfo
    {
        public string time;
        public double open;
        public double close;
        public double high;
        public double low;
        public long tick_volume;
        public long real_volume;
        public int spread;

        public override string ToString()
        {
            return "Time:" + time + ", Rate: " + close;
        }
    }

    class RateDataIndicateRequest : IRequest
    {
        public RequestType OrderType = RequestType.RateDataIndicate;
        public RateInfo[] RateInfoArray;

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        public void FromBytes(byte[] data, int length)
        {
            DataRcvBuffer rb = new DataRcvBuffer(data, length);
            rb.GetInt();
            int count = rb.GetInt();
            if (count <= 0)
                return;
            RateInfoArray = new RateInfo[count];
            for (int i = 0; i < count;i++ )
            {
                RateInfoArray[i] = new RateInfo();
                RateInfoArray[i].time = rb.GetString();
                RateInfoArray[i].open = rb.GetDouble();
                RateInfoArray[i].close = rb.GetDouble();
                RateInfoArray[i].high = rb.GetDouble();
                RateInfoArray[i].low = rb.GetDouble();
                RateInfoArray[i].tick_volume = rb.GetLong();
                RateInfoArray[i].real_volume = rb.GetLong();
                RateInfoArray[i].spread = rb.GetInt();

            }
        }
    }
}
