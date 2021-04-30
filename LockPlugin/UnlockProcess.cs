using FireBridgeCore.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockPlugin
{
    public class UnlockProcess : UserProgram
    {
        public override void Main(UserProgramContainer container, object args)
        {
            UnlockArgs unlockArgs = args as UnlockArgs;

            if (unlockArgs == null)
                return;

            using (NamedPipeClientStream pipe = new NamedPipeClientStream(".", "MyPipe", PipeDirection.InOut))
            {
                pipe.Connect(1000);
                if (pipe.IsConnected)
                {
                    using (StreamWriter sw = new StreamWriter(pipe))
                    {
                        sw.Write(unlockArgs.Username);
                        sw.Write('\0');
                        sw.Write(unlockArgs.Password);
                        sw.Write('\0');
                        sw.Flush();
                    }
                }
            }
        }
    }
}
