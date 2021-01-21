using System;
using System.Net;

namespace FireBridgeCore.Networking
{
    public class ClientRespondedEventArgs : EventArgs
    {
        public IPAddress Address { get; set; }
        public Guid Id { get; set; }

        public ClientRespondedEventArgs(IPAddress address, Guid id)
        {
            this.Address = address;
            this.Id = id;
        }
    }
}