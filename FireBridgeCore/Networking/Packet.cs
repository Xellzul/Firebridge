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
        public object Payload { get; set; }
        public int FromPort { get; set; }
        public int ToPort { get; set; }

        public const int Broadcast = int.MaxValue;

        public override string ToString()
        {
            return $"({FromPort}) -> ({ToPort}) - ({Payload.GetType()} / {Payload})";
        }
    }
}
