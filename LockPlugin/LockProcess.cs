using FireBridgeCore.Kernel;
using System;
using System.Runtime.InteropServices;

namespace LockPlugin
{
    [Serializable]
    public class LockProcess : UserProgram
    {
        [DllImport("user32")]
        public static extern bool LockWorkStation();

        public override void Main(UserProgramContainer container)
        {
            LockWorkStation();
        }
    }
}
