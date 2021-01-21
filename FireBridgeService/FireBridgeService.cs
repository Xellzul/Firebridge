using FireBridgeCore.Networking;
using NetFwTypeLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace FireBridgeService
{
    class FireBridgeService : ServiceBase
    {
        public const int Revision = 1;
        private uint _selectedSession = 0;

        AgentManager AgentManager;
        TCPServer TCPServer;
        Settings settings;

        DiscoveryServer DiscoveryServer;
        public uint SelectedSession { get => _selectedSession; set => _selectedSession = value; }

        public FireBridgeService()
        {
            this.ServiceName = "FireBridge";
            CanHandleSessionChangeEvent = true;
            CanStop = true;
        }

        private static bool AddFirewallException()
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                    Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                firewallRule.Description = "FireBridgeService";
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
                firewallRule.Enabled = true;
                firewallRule.InterfaceTypes = "All";
                firewallRule.Name = "FireBridge";
                firewallPolicy.Rules.Add(firewallRule);
            }
            catch (Exception e)
            {
                Debug("Firewall: " + e.Message);
                return false;
            }
            return true;
        }

        public static void Debug(string text, EventLogEntryType type = EventLogEntryType.Information)
        {
            EventLog.WriteEntry("FireBridge", text, type);
        }

        protected override void OnStop()
        {
            DiscoveryServer.Stop();
            AgentManager.Stop();
            TCPServer.Stop();
            base.OnStop();
        }

        public void Starting()
        {
            settings = Settings.Load("FireBridgeSettings.json");
            if (settings == null) { 
                settings = new Settings();
                settings.Save("FireBridgeSettings.json");
            }
            AddFirewallException();

            AgentManager = new AgentManager();
            TCPServer = new TCPServer();
            DiscoveryServer = new DiscoveryServer(settings.Guid);

            TCPServer.ClientConnected += TCPServer_ClientConnected;

            var sessions = SessionsHelper.ListSessions();

            foreach (var a in sessions)
            {
                AgentManager.StartSession(a.Key);

                if (a.Value)
                    SelectedSession = a.Key;
            }

            TCPServer.Start(6969);
            DiscoveryServer.Run();
        }

        protected override void OnStart(string[] args)
        {
            Starting();
            base.OnStart(args);
        }

        private void TCPServer_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Connection.MessageRecieved += Connection_MessageRecieved;
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch(e.Message.Payload)
            {
                case RequestInfo rw:
                    e.Connection.Send(new Packet() { 
                        FromPort = -1,
                        ToPort = -1,
                        Payload = GetServiceInfo()
                    });
                    break;
                default:
                    break;
            }    
        }

        public ServiceInfo GetServiceInfo()
        {
            return new ServiceInfo()
            {
                ActiveSession = SelectedSession, 
                ServiceRevision = Revision,
                ID = this.settings.Guid,
                Agents = AgentManager.GetAgents()
            };
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogoff: //delete session
                    AgentManager.CloseSession((uint)changeDescription.SessionId);
                    break;
                case SessionChangeReason.SessionLogon: //new session
                    AgentManager.StartSession((uint)changeDescription.SessionId);
                    break;
                case SessionChangeReason.SessionUnlock:
                    SelectedSession = (uint)changeDescription.SessionId;
                    break;
                case SessionChangeReason.SessionLock:
                    SelectedSession = 0;
                    break;
                default:
                    break;
            }

            TCPServer.SendAll(new Packet() { 
                FromPort = -1,
                ToPort = -1,
                Payload = GetServiceInfo()
            });

            base.OnSessionChange(changeDescription);
        }
    }
}

//Process.Start("C:\\test/a.exe ", "100");
//ApplicationLoader.PROCESS_INFORMATION procInfo;
//ApplicationLoader.StartProcessAndBypassUAC("C:\\test/a.exe 103", "C:\\test", out procInfo);
/*
s = new Server();
DiscoveryServer = new DiscoveryServer();
SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
s.ClientConnected += S_ClientConnected;
s.ConnectionDisconnected += S_ConnectionDisconnected;

DiscoveryServer.Run();
Task.Run(() =>
{
    s.Start(new IPEndPoint(IPAddress.Any, 6969));
});*/