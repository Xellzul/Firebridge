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

namespace FireBridgeCore.Controller
{
    public sealed class ConnectionManger
    {
        private DiscoveryClient _discoveryClient;
        private ConcurrentDictionary<Guid, ServiceConnection> _services;
        private ConcurrentDictionary<Guid, ServiceConnection> _selectedServices;

        static ConnectionManger _instance;

        public static ConnectionManger Instance
        {
            get { return _instance ??= new ConnectionManger(); }
        }

        private ConnectionManger()
        {
            _services = new ConcurrentDictionary<Guid, ServiceConnection>();
            _selectedServices = new ConcurrentDictionary<Guid, ServiceConnection>();
            _discoveryClient = new DiscoveryClient();
            _discoveryClient.ClientResponded += _discoveryClient_ClientResponded;
        }

        public bool SelectService(Guid serviceID)
        {
            ServiceConnection sc = null;
            if(_services.TryGetValue(serviceID, out sc))
            {
                if (sc.Status == ConnectionStatus.Connected)
                    return _selectedServices.TryAdd(serviceID, sc);
            }
            return false;
        }

        public bool DeselectService(Guid serviceID)
        {
            return _selectedServices.TryRemove(serviceID, out _);
        }

        public ICollection<ServiceConnection> GetSelectedServices()
        {
            return _selectedServices.Values.Where(x => x.Status == ConnectionStatus.Connected).ToList();
        }

        public ICollection<ServiceConnection> GetServices()
        {
            return _services.Values.ToList();
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
            }
        }

        private void Conn_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (sender == null || e == null || !(sender is ServiceConnection)) { 

                if (e.Now == ConnectionStatus.Disconnected)
                    _services.TryRemove(((ServiceConnection)sender).Id, out _);
            }
            else if (e.Now == ConnectionStatus.Connected)
                OnClientConnected(new ServiceConnectionConnectedEventArgs() { ServiceConnection = (ServiceConnection)sender });
        }

        public void Start()
        {
            _discoveryClient.Run();
        }

        public void Stop()
        {
            _discoveryClient.End();
        }

        public void OnClientConnected(ServiceConnectionConnectedEventArgs e)
        {
            if (ClientConnected == null || e == null)
                return;

            ClientConnected?.Invoke(this, e);
        }
        public event EventHandler<ServiceConnectionConnectedEventArgs> ClientConnected;

        public void OnClientMessageRecieved(ServiceConnectionMessageRecievedEventArgs e)
        {
            if (ClientMessageRecieved == null || e == null)
                return;

            ClientMessageRecieved?.Invoke(this, e);
        }
        public event EventHandler<ServiceConnectionMessageRecievedEventArgs> ClientMessageRecieved;
    }
}
