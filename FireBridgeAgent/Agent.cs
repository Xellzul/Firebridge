using FireBridgeCore;
using FireBridgeCore.Kernel;
using FireBridgeCore.Networking;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;

namespace FireBridgeAgent
{
    internal class Agent
    {
        public UserKernel Kernel { get; private set; }
        public TCPServer TCPServer { get; private set; }
        public TCPConnection ServiceConnection { get; private set; }
        public AgentInfo AgentInfo { get; private set; }
        public bool DebugMode { get; private set; } = false;

        public Agent()
        {
            Console.WriteLine("Agent is in DEUBUG MODE");
            DebugMode = true;
            constructor(Guid.NewGuid());
        }

        public Agent(Guid agentID)
        {
            Console.WriteLine("Agent");
            constructor(agentID);
        }

        private void constructor(Guid agentID)
        {
            AgentInfo = new AgentInfo();
            AgentInfo.AgentID = agentID;
            AgentInfo.AgentVersion = 1;
            AgentInfo.Elevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            AgentInfo.ProcessID = Process.GetCurrentProcess().Id;
            AgentInfo.SessionID = (uint)Process.GetCurrentProcess().SessionId;
            AgentInfo.IntegrityLevel = IntegrityLevelHelper.GetCurrentIntegrity();

            Kernel = new UserKernel();
        }

        internal void Start()
        {
            Console.WriteLine($"Agent ID {AgentInfo.AgentID} Starting");
            TCPServer = new TCPServer();

            TCPServer.ClientConnected += TCPServer_ClientConnected;

            int port = 50000;
            while (!TCPServer.Start(port))
                port++;

            AgentInfo.Port = port;

            Console.WriteLine($"TCP Server started at port {port}");

            ServiceConnection = new TCPConnection();
            ServiceConnection.MessageRecieved += ServiceConnection_MessageRecieved;
            ServiceConnection.ConnectionStatusChanged += ServiceConnection_ConnectionStatusChanged;

            ServiceConnection.Start(IPAddress.Parse("127.0.0.1"), 6970);
        }

        private void ServiceConnection_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (e.Before == ConnectionStatus.Connected && e.Now == ConnectionStatus.Disconnected)
                this.Stop();
        }

        private void Stop()
        {
            TCPServer.Stop();
            ServiceConnection.Close();
        }

        private void TCPServer_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("TCPServer_ClientConnected");
            e.Connection.MessageRecieved += Connection_MessageRecieved;
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            Console.WriteLine("Connection_MessageRecieved");
            switch (e.Message.Payload)
            {
                case UserProcess up:
                    Console.Write($"STARTING PROCESS: {up.RemoteProcessID}");
                    Kernel.StartProcess(e.Connection, up);
                    break;
                case RequestInfo:
                    e.Connection.Send(new Packet() {
                        FromPort=-1, 
                        ToPort=-1, 
                        Payload = this.AgentInfo});
                    break;
                default:
                    Kernel.SendToProcess(e.Connection, e.Message);
                    break;
            }
        }

        private void ServiceConnection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch(e.Message.Payload)
            {
                case RequestInfo RI:
                    e.Connection.Send(new Packet()
                    {
                        FromPort = -1,
                        ToPort = -1,
                        Payload = AgentInfo
                    });
                    break;

                default:
                    break;
            }
        }
    }
}