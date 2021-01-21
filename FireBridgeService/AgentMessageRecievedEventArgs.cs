using FireBridgeCore.Networking;
using System;

namespace FireBridgeService
{
    public class AgentMessageRecievedEventArgs : EventArgs
    {
        public Packet Packet { get; set; }
        public Agent Sender { get; set; }
    }
}
