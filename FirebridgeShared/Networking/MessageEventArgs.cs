using System;

namespace FirebridgeShared.Networking
{
    public class MessageEventArgs : EventArgs
    {
        public Packet Packet { get; set; }
        public MessageEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }
}
