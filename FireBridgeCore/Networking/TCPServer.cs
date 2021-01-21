using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public class TCPServer : Server
    {
        private TcpListener _tcpListener;

        public override bool Start(int port)
        {
            _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));

            try {
                _tcpListener.Start();
            }
            catch(SocketException e)
            {
                if(e.SocketErrorCode == SocketError.AddressAlreadyInUse)
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

        public override void Stop()
        {
            this._end = true;
            _tcpListener.Stop();
        }

        protected override void AcceptConenction()
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

                connection.Start(client);

                OnClientConnected(new ClientConnectedEventArgs() { Connection = connection });
            }

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
    }
}
