using System;

namespace FirebridgeShared.Networking
{
    [Serializable]
    public class Packet
    {
        public uint Id { get; set; }
        public object Data { get; set; }
    }
}
