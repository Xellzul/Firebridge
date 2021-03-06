using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    [Serializable]
    public sealed class Packet
    {
        public Packet(Guid from, Guid to, object payload)
        {
            Payload = payload;
            From = from;
            To = to;
        }
        public object Payload { get; set; }
        public Guid From { get; set; }
        public Guid To { get; set; }

        public override string ToString()
        {
            return $"({From}) -> ({To}) - <{Payload}>";
        }
    }
}
