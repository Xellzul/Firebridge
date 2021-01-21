using System;

namespace FireBridgeCore.Controller
{
    public class ServiceConnectionConnectedEventArgs : EventArgs
    {
        public ServiceConnection ServiceConnection { get; set; }
    }
}