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
        ConcurrentDictionary<ServerConnection, bool> Connections = new ConcurrentDictionary<ServerConnection, bool>();
        public Server()
        {
        }

        public void Start()
        {
            TcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, 6969));
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
            var client2 = TcpListener.AcceptTcpClient();
            TcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), TcpListener);
            TcpClient client = TcpListener.EndAcceptTcpClient(ar);
            var newConnection = new ServerConnection(client);
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
        public ServerConnection Client { get; set; }
        public ServerConnectionEventArgs(ServerConnection client)
        {
            Client = client;
        }
    }
}
