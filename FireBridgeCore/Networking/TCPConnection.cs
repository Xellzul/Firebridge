using FireBridgeCore.Networking.Models;
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
    public class TCPConnection : Connection
    {
        protected TcpClient _client;

        //TODO: maybe not here
        public Guid Id { get; } 
        public IServer Owner { get; private set; }
        public TCPConnection(Guid guid)
        {
            Id = guid;
        }

        public TCPConnection() 
        {
            Id = Guid.NewGuid();
        }

        public IPAddress IPAddress
        {
            get
            {
                if (_client == null)
                    return null;

                return ((IPEndPoint)_client.Client.RemoteEndPoint).Address;
            }
        }

        public int Port
        {
            get
            {
                if (_client == null || _client.Client == null)
                    return -1;

                return ((IPEndPoint)_client.Client.RemoteEndPoint).Port;
            }
        }

        //TODO: not here
        public ControllerInfo GetReport()
        {
            return new ControllerInfo() 
            {
                Port = Port,
                IpAddress = IPAddress.ToString(),
                Connected = Status == ConnectionStatus.Connected
            };
        }

        public virtual void Start(TcpClient client, IServer server)
        {
            Owner = server;
            Status = ConnectionStatus.Connecting;
            _client = client;
            StartReader();
        }

        public virtual void Start(TcpClient client)
        {
            if (_shouldEnd)
                return;

            Status = ConnectionStatus.Connecting;
            _client = client;
            StartReader();
        }

        public virtual void Start(IPAddress IP, int port)
        {
            if (_shouldEnd)
                return;

            Status = ConnectionStatus.Connecting;

            _client = new TcpClient();
            _client.Connect(new IPEndPoint(IP, port));

            StartReader();
        }

        protected virtual void PostStart()
        {
            Status = ConnectionStatus.Connected;
        }

        private void StartReader()
        {
            if (!_client.Connected)
            {
                Status = ConnectionStatus.Disconnected;
                return;
            }

            _writeStream = _client.GetStream();
            _readStream = _client.GetStream();

            this._readerThread = new Thread(ReadLoop);
            this._readerThread.IsBackground = true;
            this._readerThread.Name = "ReaderThread TCP - " + _client.Client.RemoteEndPoint.ToString();
            this._readerThread.Start();

            PostStart();
        }

        public override void Close()
        {
            Status = ConnectionStatus.Disconnected;
            _shouldEnd = true;
            _client.GetStream().Flush();
            _client?.Close();
        }
    }
}
