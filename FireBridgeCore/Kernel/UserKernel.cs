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
        private Object _idLock;
        private int _lastID = 1;
        private ConcurrentDictionary<int, UserProcess> _programs;

        public UserKernel()
        {
            _idLock = new Object();
            _programs = new ConcurrentDictionary<int, UserProcess>();
        }

        public void SendToProcess(Connection connection, Packet message)
        {
            _programs.TryGetValue(message.ToPort, out UserProcess userProcess);

            if(userProcess != null)
                Task.Run(() => { userProcess?.DataRecieved(connection, message); });
        }

        public bool StartProcess(Connection connection, UserProcess up)
        {
            int id;
            lock(_idLock)
            {
                id = _lastID;
                _lastID++;
            }

            _programs.TryAdd(id, up);
            up.Completed += Process_Completed;
            up.Start(this, id, connection);
            return true;
        }

        private void Process_Completed(object sender, EventArgs e)
        {
            _programs.TryRemove(((UserProcess)sender).ProcessID, out _);
        }
    }
}
