using FireBridgeCore.Networking;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel
{
    public abstract class UserProcess
    {
        public Connection Connection { get; private set; }
        public Guid Id { get; private set; }
        public Guid RemoteId { get; private set; }
        public UserProcess(Guid id, Guid remoteId, Connection connection)
        {
            RemoteId = remoteId;
            Id = id;
            Connection = connection;
        }

        public event EventHandler Completed;
        private void OnCompleted(EventArgs e)
        {
            Completed?.Invoke(this, e);
        }


        public virtual void Stop()
        {
            OnCompleted(EventArgs.Empty);
        }

        public abstract bool Start();
    }
}