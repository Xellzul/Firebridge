using FireBridgeCore.Controller;
using FireBridgeCore.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace FireBridgeController
{
    public class ConnectionManger
    {
        private DiscoveryClient _discoveryClient;
        private ConcurrentDictionary<Guid, ServiceConnection> _services;
        public ConnectionManger()
        {
            _services = new ConcurrentDictionary<Guid, ServiceConnection>();
            _discoveryClient = new DiscoveryClient();
            _discoveryClient.ClientResponded += _discoveryClient_ClientResponded;
        }

        public ICollection<ServiceConnection> GetConnectedServices()
        {
            return _services.Values.Where(x => x.Status == ConnectionStatus.Connected).ToList();
        }

        private void _discoveryClient_ClientResponded(object sender, ClientRespondedEventArgs e)
        {
            if (_services.ContainsKey(e.Id)) 
                return;

            var conn = new ServiceConnection(e.Id);
            if(_services.TryAdd(e.Id, conn))
            {
                conn.ConnectionStatusChanged += Conn_ConnectionStatusChanged;
                conn.Start(e.Address, 6969); 
                OnClientConnected(new ServiceConnectionConnectedEventArgs() { ServiceConnection = conn });
            }
        }

        private void Conn_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (sender == null || e == null || !(sender is ServiceConnection))
                if (e.Now == ConnectionStatus.Disconnected)
                    _services.TryRemove(((ServiceConnection)sender).Id, out _);
        }

        public void Start()
        {
            _discoveryClient.Run();
        }

        public void Stop()
        {
            _discoveryClient.End();
        }

        protected virtual void OnClientConnected(ServiceConnectionConnectedEventArgs e)
        {
            if (ClientConnected == null || e == null)
                return;

            foreach (EventHandler<ServiceConnectionConnectedEventArgs> reciever in ClientConnected.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }
        public event EventHandler<ServiceConnectionConnectedEventArgs> ClientConnected;

        protected virtual void OnClientMessageRecieved(ServiceConnectionMessageRecievedEventArgs e)
        {
            if (ClientMessageRecieved == null || e == null)
                return;

            foreach (EventHandler<ServiceConnectionMessageRecievedEventArgs> reciever in ClientMessageRecieved.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }
        public event EventHandler<ServiceConnectionMessageRecievedEventArgs> ClientMessageRecieved;
    }
}
