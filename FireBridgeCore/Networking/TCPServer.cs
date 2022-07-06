using FireBridgeCore.Networking.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public class TCPServer : IServer
    {
        protected ConcurrentDictionary<Guid, Connection> Connections = new ConcurrentDictionary<Guid, Connection>();
        protected Thread ListenerThread;
        protected bool _end = false;
        protected TcpListener _tcpListener;
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientConnectedEventArgs> ClientConnecting;

        //Todo: Move somewhere else?
        public List<ControllerInfo> GetReport()
        {
            return Connections.Select(x => ((TCPConnection)x.Value).GetReport()).ToList();
        }


        private void Connection_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (sender == null)
                return;

            if (e.Now == ConnectionStatus.Disconnected && sender is TCPConnection)
                Connections.TryRemove(((TCPConnection)sender).Id, out _);

        }

        public void SendAll(Packet packet)
        {
            foreach (var conn in this.Connections)
                conn.Value.Send(packet);
        }

        public bool Start(int port)
        {
            _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));

            try
            {
                _tcpListener.Start();
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    _tcpListener = null;
                    return false;
                }
                throw;
            }

            ListenerThread = new Thread(AcceptConenction);
            ListenerThread.Start();

            return true;
        }

        void AcceptConenction()
        {
            while (!_end)
            {
                if (!_tcpListener.Server.IsBound)
                    break;

                TcpClient client;

                try
                {
                    client = _tcpListener.AcceptTcpClient();
                }
                catch (SocketException)
                {
                    //todo: error handling?
                    continue;
                }


                if (client == null || !client.Connected)
                    continue;

                TCPConnection connection = new TCPConnection();

                connection.ConnectionStatusChanged += Connection_ConnectionStatusChanged;

                Connections.TryAdd(connection.Id, connection);

                OnClientConnecting(new ClientConnectedEventArgs() { Connection = connection });

                connection.Start(client);
                
                OnClientConnected(new ClientConnectedEventArgs() { Connection = connection });
            }

        }

        public void Stop()
        {
            this._end = true;
            _tcpListener.Stop();
        }

        public Connection GetConnection(Guid id)
        {
            Connections.TryGetValue(id, out Connection conn);
            return conn;
        }

        void OnClientConnected(ClientConnectedEventArgs e) => ClientConnected?.Invoke(this, e);
        void OnClientConnecting(ClientConnectedEventArgs e) => ClientConnecting?.Invoke(this, e);
    }
}
