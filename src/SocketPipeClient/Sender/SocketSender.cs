using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.Sender
{
    class SocketSender : ISender
    {
        private Socket _socket;
        private byte[] _rvcBuffer;
        private int _rcvBufferLength = 1024 * 1024;


        public SocketSender()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _rvcBuffer = new byte[_rcvBufferLength];
            // For UDP
            // Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); 
            _socket.Connect("127.0.0.1", 9000);
        }

        public int Send(IRequest req)
        {
            _socket.Send(req.GetBytes());
            int ret = _socket.Receive(_rvcBuffer);
            if (ret < 0)
                return ret;
            req.FromBytes(_rvcBuffer, ret);

            return 0;
        }


        public DeamonState State
        {
            get { throw new NotImplementedException(); }
        }
    }
}
