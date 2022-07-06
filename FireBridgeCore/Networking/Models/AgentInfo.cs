using System;

namespace FireBridgeCore.Networking
{
    [Serializable]
    public class AgentInfo
    {
        public Guid AgentID { get; set; }
        public uint SessionID { get; set; }
        public int ProcessID { get; set; }
        public string IntegrityLevel { get; set; }
    }
}
