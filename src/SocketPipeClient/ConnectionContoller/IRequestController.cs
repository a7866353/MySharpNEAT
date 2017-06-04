using SocketTestClient.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketTestClient.ConnectionContoller
{
    interface IRequestController
    {
        IRequest GetRequest();
        void SetResult(IRequest req);
    }
}
