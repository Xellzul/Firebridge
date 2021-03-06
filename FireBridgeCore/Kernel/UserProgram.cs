using FireBridgeCore.Networking;
using System;

namespace FireBridgeCore.Kernel
{
    [Serializable]
    public abstract class UserProgram
    {
        public abstract void Main(UserProgramContainer container);
        public virtual void OnEnding() { }
        public virtual void OnDataRecieved(Packet packet) { }
    }
}
