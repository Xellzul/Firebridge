using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    [Serializable]
    public sealed class ProgramMessageModel
    {
        public byte[] Data { get; set; }
    }
}
