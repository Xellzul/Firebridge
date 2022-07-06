using FireBridgeCore.Networking.Models;
using System;
using System.Collections.Generic;

namespace FireBridgeCore.Networking
{
    [Serializable]
    public class ServiceInfo
    {
        public List<AgentInfo> Agents { get; set; }
        public List<ControllerInfo> Controllers { get; set; }
        public Version ServiceVersion { get; set; }
        public Guid Id { get; set; }
        public uint ActiveSession { get; set; }
        public string IntegrityLevel { get; set; }
    }
}
