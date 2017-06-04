using SocketTestClient.ConnectionContoller;
using SocketTestClient.RequestObject;
using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            
            System.Console.WindowWidth = (int)(System.Console.LargestWindowWidth * 0.7);
            System.Console.WindowHeight = (int)(System.Console.LargestWindowHeight * 0.7);
            System.Console.WriteLine("Test Started!");

            TestClientDeamon();
            // TestSocketDeamon();
         
        }


        static void TestSocketDeamon()
        {
            SocketDeamonSender sender = new SocketDeamonSender();
            TestRequest testReq = new TestRequest();

            while (true)
            {
                if (sender.State != DeamonState.Connected)
                {
                    Thread.Sleep(2000);
                    continue;
                }

                sender.Send(testReq);
                Thread.Sleep(2000);

            }

        }
        static void TestRateRequest()
        {
            SocketDeamonSender sender = new SocketDeamonSender();
            RateByTimeRequest rateReq = new RateByTimeRequest();
            rateReq.SymbolName = "USDJPYpro";
            rateReq.TimeFrame = 5;
            rateReq.StartTime = DateTime.Now.AddDays(-100);
            rateReq.StopTime = DateTime.Now;

            while (true)
            {
                if (sender.State != DeamonState.Connected)
                {
                    Thread.Sleep(2000);
                    continue;
                }

                sender.Send(rateReq);
                Thread.Sleep(2000);

            }

        }
        static void TestOrderSendRequest()
        {
            SocketDeamonSender sender = new SocketDeamonSender();
            SendOrderRequest req = new SendOrderRequest();
            req.OrderCmd = SendOrderRequest.Cmd.Buy;
            req.SymbolName = "USDJPYpro";

            while (true)
            {
                if (sender.State != DeamonState.Connected)
                {
                    Thread.Sleep(2000);
                    continue;
                }
                // Buy
                req.OrderCmd = SendOrderRequest.Cmd.Buy;
                req.SymbolName = "USDJPYpro";
                sender.Send(req);
                Thread.Sleep(2000);

                // Sell
                req.OrderCmd = SendOrderRequest.Cmd.Sell;
                req.SymbolName = "USDJPYpro";
                sender.Send(req);
                Thread.Sleep(2000);

                Thread.Sleep(20000);
              
            }

        }
        static void TestSocketSend()
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // For UDP
            // Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); 

            s.Connect("127.0.0.1", 9000);

            string testStr = "Socket Connection test!\r\n";
            byte[] dataBuffer = Encoding.ASCII.GetBytes(testStr);

            while (true)
            {
                s.Send(dataBuffer);
                Thread.Sleep(1000);
            }
        }
        static void TestClientDeamon()
        {
            ClientControl ctrl = new ClientControl();
            ctrl.StartListen();
        }
    }
}
