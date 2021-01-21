using System;

namespace FireBridgeCore.Networking
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public Packet Message { get; set; }
        public Connection Connection { get; set; }
    }
}