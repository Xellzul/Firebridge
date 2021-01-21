using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public class AgentConnection : TCPConnection
    {
        public override void Start(IPAddress IP, int port)
        {
            base.Start(IP, port);
            Status = ConnectionStatus.Connected;
        }
    }
}
