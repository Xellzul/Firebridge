using FireBridgeCore.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel
{
    public class UserKernel
    {
        private ConcurrentDictionary<Guid, UserProcess> _processes;
        public UserKernel()
        {
            _processes = new ConcurrentDictionary<Guid, UserProcess>();
        }

        public bool StartProcessAttached(UserProgram program, Connection connection, Guid localID, Guid remoteID)
        {
            var process = new AttachedUserProcess(program, connection, localID, remoteID);
            return _startProcess(process);
        }

        public bool StartProcessDetached(StartProgramModel startProgramModel, Connection connection)
        {
            var process = new DetachedUserProcess(startProgramModel, connection);
            return _startProcess(process);
        }

        private bool _startProcess(UserProcess process)
        {
            if (!_processes.TryAdd(process.Id, process))
                return false;

            process.Completed += Process_Completed;
            if (!process.Start())
            {
                process.Completed -= Process_Completed;
                _processes.TryRemove(process.Id, out _);
                return false;
            }
            return true;
        }

        private void Process_Completed(object sender, EventArgs e)
        {
            UserProcess up = sender as UserProcess;
            if (up == null)
                return;

            RemoveProcess(up);
        }

        public bool RemoveProcess(UserProcess process)
        {
            process.Completed -= Process_Completed;
            process.Stop();
            return _processes.TryRemove(process.Id, out _);
        }

        public void Stop()
        {
            foreach (var item in _processes)
            {
                RemoveProcess(item.Value);
            }
        }
    }
}
