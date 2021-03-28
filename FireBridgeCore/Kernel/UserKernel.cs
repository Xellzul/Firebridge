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

        public bool StartProcess(UserProcess process)
        {
            if (!_processes.TryAdd(process.Id, process))
                return false;

            process.Completed += Process_Completed;
            if (!process.Start())
            {
                removeProcess(process);
                return false;
            }

            return true;
        }

        private void Process_Completed(object sender, EventArgs e)
        {
            UserProcess up = sender as UserProcess;

            if (up == null)
                return;

            removeProcess(up);
        }

        private bool removeProcess(UserProcess process)
        {
            process.Completed -= Process_Completed;
            return _processes.TryRemove(process.Id, out _);
        }

        public bool StopProcess(UserProcess process)
        {
            process.Completed -= Process_Completed;
            process.Stop();
            var removed = _processes.TryRemove(process.Id, out _);
            return removed;
        }

        public void Stop()
        {
            foreach (var item in _processes)
            {
                StopProcess(item.Value);
            }
        }
    }
}
