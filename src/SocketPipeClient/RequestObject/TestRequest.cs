using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.RequestObject
{
    class TestRequest : IRequest
    {
        public RequestType OrderType = RequestType.TestRequest;
        public double doubleTest = 0.5;
        public int intTest = 1234567;
        public string stringTest = "0123456789ABCDEFGH";
        public string longStringTest;


        private SendOrderResult _result;

        public byte[] GetBytes()
        {
            // Clear Result
            _result = SendOrderResult.None;

            DataSendBuffer sb = new DataSendBuffer();
            sb.Add((int)OrderType);
            sb.Add(doubleTest);
            sb.Add(intTest);
            sb.Add(stringTest);

            longStringTest = "";
            for (int i = 0; i < 4000;i++ )
            {
                longStringTest += i + ": " + stringTest + "\r\n";
            }
            sb.Add(longStringTest);
            return sb.GetBytes();
        }

        public void FromBytes(byte[] data, int length)
        {
            DataRcvBuffer rb = new DataRcvBuffer(data, length);
            bool isOK = true;
            do{
                int type = rb.GetInt();
                if (type != (int)OrderType) { isOK = false; break; }

                double valueDouble = rb.GetDouble();
                if (valueDouble != doubleTest) { isOK = false; break; }

                int valueint = rb.GetInt();
                if (valueint != intTest) { isOK = false; break; }

                string valueStr = rb.GetString();
                if (valueStr.CompareTo(stringTest) != 0) { isOK = false; break; }

                string valueLongStr = rb.GetString();
                if (valueLongStr.CompareTo(longStringTest) != 0) { isOK = false; break; }


            }while(false);
            if (isOK == false)
                System.Console.WriteLine("TestRequest: Error!");
            else
                System.Console.WriteLine("TestRequest: OK!");
        }

        public SendOrderResult Result
        {
            get { return _result; }
        }
    }
}
