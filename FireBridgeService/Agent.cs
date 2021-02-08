using FireBridgeCore;
using FireBridgeCore.Kernel;
using FireBridgeCore.Networking;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace FireBridgeService
{
    public class Agent : Connectable
    {
        public uint SessionID { get; private set; }
        public IIntegrityLevel Elevation { get; private set; }
        public AgentInfo AgentInfo { get; private set; }
        public Connection _connection { private set; get; }

        private Process _process;
        public Guid Id { get; }

        public Agent(uint sessionID, IIntegrityLevel elevation)
        {
            Id = Guid.NewGuid();
            SessionID = sessionID;
            Elevation = elevation;
        }

        public void Connected(Connection connection, AgentInfo agentInfo)
        {
            _connection = connection;
            AgentInfo = agentInfo;
            AgentInfo.IntegrityLevel = Elevation;
            _connection.ConnectionStatusChanged += _connection_ConnectionStatusChanged;
            this.Status = ConnectionStatus.Connected;
        }

        private void _connection_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (e.Now == ConnectionStatus.Disconnected)
                Close();
        }

        public void StartProcess()
        {
            this.Status = ConnectionStatus.Connecting;

            _process = ApplicationLoader.StartProcess(
                AppDomain.CurrentDomain.BaseDirectory + "FireBridgeAgent.exe",
                new[] { Id.ToString() },
                SessionID,
                Elevation
            ).process;

            if (_process == null)
            {
                this.Status = ConnectionStatus.Disconnected;
                return;
            }

            _process.Exited += _process_Exited;

            if (_process.HasExited)
                Close();
        }

        private void _process_Exited(object sender, EventArgs e)
        {
            Close();
        }

        internal void Close()
        {
            _connection?.Close();
            this.Status = ConnectionStatus.Disconnected;
        }
    }
}
