using FireBridgeCore.Kernel;
using System;
using System.Runtime.Serialization;

namespace FireBridgeCore.Networking
{
    [Serializable]
    public class AgentInfo
    {
        public int AgentVersion { get; set; }
        public Guid AgentID { get; set; }
        public uint SessionID { get; set; }
        public int ProcessID { get; set; }
        public bool Elevated { get; set; }
        public IIntegrityLevel? IntegrityLevel { get; set; }
        public int Port { get; set; }
    }
}
