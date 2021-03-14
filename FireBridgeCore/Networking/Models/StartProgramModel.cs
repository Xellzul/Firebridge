using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    [Serializable]
    public class StartProgramModel
    {
        public string Type { get; set; }
        public byte[] Assemblies { get; set; } //todo: list of assemblies
        public byte[] StartParameters { get; set; }
        public Guid ProcessId { get; set; }
        public Guid RemoteId { get; set; }
        public uint SessionId { get; set; }
        public Kernel.IIntegrityLevel  IntegrityLevel { get; set; }
    }
}
