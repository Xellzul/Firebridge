using FireBridgeCore.Networking;
using System;

namespace FireBridgeService
{
    public class AgentStatusChangedEventArgs : EventArgs
    {
        public Agent Agent { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
    }
}