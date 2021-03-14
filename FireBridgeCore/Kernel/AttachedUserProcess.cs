using FireBridgeCore.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel
{
    public class AttachedUserProcess : UserProcess
    {
        private UserProgram program;
        private Guid remoteID;
        UserProgramContainer userProgramContainer;

        public AttachedUserProcess(UserProgram program, Connection connection, Guid localID, Guid remoteID) : base(localID, connection)
        {
            this.program = program;
            this.remoteID = remoteID;
        }

        public override bool Start()
        {
            userProgramContainer = new UserProgramContainer();
            userProgramContainer.StartAsync(program, null, Connection, Id, remoteID);
            return true;
        }

        public override void Stop()
        {
        }
    }
}
