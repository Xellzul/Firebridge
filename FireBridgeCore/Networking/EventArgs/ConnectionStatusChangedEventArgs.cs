using System;

namespace FireBridgeCore.Networking
{
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatus Before { get; set; }
        public ConnectionStatus Now { get; set; }
    }
}