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
            AcceptConenction();
            //TcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), TcpListener);
        }

        public void Stop()
        {
            TcpListener.Stop();
        }

        private void AcceptConenction()
        {
            while (true) {
                TcpClient client;
                try { 
                  client = TcpListener.AcceptTcpClient();
                }
                catch (SocketException e) {
                    if(e.ErrorCode == 10004)
                        return;
                    throw;
                }
                var newConnection = new Connection(client);
                newConnection.Disconnected += NewConnection_Disconnected;
                Connections.TryAdd(newConnection, true);
                OnClientConnected(new ServerConnectionEventArgs(newConnection));
            }
        }

        //private void AcceptTcpClientCallback(IAsyncResult ar)
        //{
        //    var TcpListener = (TcpListener)ar.AsyncState;
        //    TcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), TcpListener);
        //    TcpClient client = TcpListener.EndAcceptTcpClient(ar);
        //    var newConnection = new Connection(client);
        //    newConnection.Disconnected += NewConnection_Disconnected;
        //    Connections.TryAdd(newConnection, true);
        //    OnClientConnected(new ServerConnectionEventArgs(newConnection));
        //}

        private void NewConnection_Disconnected(object sender, EventArgs e)
        {
            bool connected;
            Connections.TryRemove((Connection)sender, out connected);
            OnConnectionDisconnected(new ServerConnectionEventArgs((Connection)sender));
        }

        protected virtual void OnClientConnected(ServerConnectionEventArgs e)
        {
            ClientConnected?.SafeInvoke(this, e);
        }

        public event EventHandler ClientConnected;


        protected virtual void OnConnectionDisconnected(ServerConnectionEventArgs e)
        {
            ConnectionDisconnected?.SafeInvoke(this, e);
        }
        public event EventHandler ConnectionDisconnected;
    }
}
