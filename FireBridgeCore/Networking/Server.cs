using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public abstract class Server
    {
        protected ConcurrentDictionary<Guid, Connection> Connections = new ConcurrentDictionary<Guid, Connection>();
        protected Thread ListenerThread;
        protected bool _end = false;

        public abstract bool Start(int port);

        protected abstract void AcceptConenction();

        public abstract void Stop();

        public Connection GetConnection(Guid id)
        {
            Connections.TryGetValue(id, out Connection conn);
            return conn;
        }

        protected virtual void OnClientConnected(ClientConnectedEventArgs e)
        {
            if (ClientConnected == null)
                return;

            foreach (EventHandler<ClientConnectedEventArgs> reciever in ClientConnected.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
    }
}
