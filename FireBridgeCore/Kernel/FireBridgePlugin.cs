using FireBridgeCore.Controller;
using System;
using System.Collections.Generic;

namespace FireBridgeCore.Kernel
{
    public abstract class FireBridgePlugin
    {
        public List<KeyValuePair<string, EventHandler>> PossibleActions = new List<KeyValuePair<string, EventHandler>>();
        public abstract void Start();
        public abstract int Order { get; }
        public byte[] AssemblyData { get; set; }
    }
}
