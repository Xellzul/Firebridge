using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking.Models
{
    [Serializable]
    public class ControllerInfo
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool Connected { get; set; }
    }
}
