using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.Sender
{
    enum DeamonState
    {
        Disconnected,
        Connected,
        Receiving,
        Sending,
        Disconnecting,
    }

    interface ISender
    {
        int Send(IRequest req);
        DeamonState State { get; }
        
    }

    interface IRequest
    {
        byte[] GetBytes();
        void FromBytes(byte[] data, int length);
    }
}
