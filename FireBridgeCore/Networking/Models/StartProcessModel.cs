using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking.Models
{
    [Serializable]
    public class StartProcessModel
    {
        public ICollection<byte[]> Assemblies { get; set; }
    }
}
