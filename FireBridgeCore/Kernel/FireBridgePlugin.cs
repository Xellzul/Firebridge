using FireBridgeCore.Controller;
using System;
using System.Collections.Generic;

namespace FireBridgeCore.Kernel
{
    public abstract class FireBridgePlugin
    {
        public abstract ICollection<KeyValuePair<string, Action<ServiceConnection[]>>> Register();
    }
}
