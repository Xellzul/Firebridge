using FireBridgeCore.Networking;
using System;

namespace FireBridgeCore.Kernel
{
    public abstract class UserProgram
    {
        public abstract void Main(UserProgramContainer container, object args);
        public virtual void OnEnding() { }
        public virtual void OnDataRecieved(Packet packet) { }
    }
}
