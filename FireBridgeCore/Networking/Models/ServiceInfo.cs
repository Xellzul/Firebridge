using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    [Serializable]
    public class ServiceInfo
    {
        //public List<AgentInfo> Agents { get; set; }
        public int ServiceRevision { get; set; }
        public Guid ID { get; set; }
        public uint ActiveSession { get; set; }
    }
}
