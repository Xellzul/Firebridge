using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FirebridgeShared.Networking
{
    public class Server
    {
        TcpListener TcpListener;
        ConcurrentDictionary<Connection, bool> Connections = new ConcurrentDictionary<Connection, bool>();
        public Server()
        {
        }

        public void Start(IPEndPoint endpoint)
        {
            TcpListener = new TcpListener(endpoint);
            TcpListener.Start();
            TcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), TcpListener);
        }

        public void Stop()
        {
            TcpListener.Stop();
        }

        private void AcceptTcpClientCallback(IAsyncResult ar)
        {
            var TcpListener = (TcpListener)ar.AsyncState;
            TcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), TcpListener);
            TcpClient client = TcpListener.EndAcceptTcpClient(ar);
            var newConnection = new Connection(client);
            Connections.TryAdd(newConnection, true);
            OnClientConnected(new ServerConnectionEventArgs(newConnection));
            bool connected;
            Connections.TryRemove(newConnection, out connected);
        }

        protected virtual void OnClientConnected(ServerConnectionEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        public event EventHandler ClientConnected;
    }

    public class ServerConnectionEventArgs : EventArgs
    {
        public Connection Connection { get; set; }
        public ServerConnectionEventArgs(Connection connection)
        {
            Connection = connection;
        }
    }
}
