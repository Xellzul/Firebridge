using System;
using System.Collections.Generic;
using System.Text;

namespace FirebridgeShared.Networking
{
    public class ClientRespondedEventArgs : EventArgs
    {
        public string Ip { get; set; }
        public ClientRespondedEventArgs(string ip)
        {
            Ip = ip;
        }
    }
}
