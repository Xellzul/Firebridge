using FireBridgeCore;
using FireBridgeCore.Kernel;
using FireBridgeCore.Networking;
using Microsoft.Win32;
using NetFwTypeLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;

namespace FireBridgeService
{
    class FireBridgeService : ServiceBase
    {
        Settings settings;
        UserKernel Kernel;
        TCPServer TCPServer;
        DiscoveryServer DiscoveryServer;

        public FireBridgeService()
        {

            this.ServiceName = "FireBridge";
            CanStop = true;
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        private static bool AddFirewallException()
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                    Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

                if (firewallPolicy.Rules.Item("FireBridge") == null)
                    return true;

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
            Kernel.Stop();
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

            Kernel = new UserKernel();
            TCPServer = new TCPServer();
            DiscoveryServer = new DiscoveryServer(settings.Guid);

            TCPServer.ClientConnecting += TCPServer_ClientConnecting;
            TCPServer.Start(6969);
            DiscoveryServer.Run();
        }

        protected override void OnStart(string[] args)
        {
            Starting();
            base.OnStart(args);
        }

        private void TCPServer_ClientConnecting(object sender, ClientConnectedEventArgs e)
        {
            e.Connection.MessageRecieved += Connection_MessageRecieved;
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch(e.Message.Payload)
            {
                case RequestInfo:
                    e.Connection.Send(new Packet(this.settings.Guid, Guid.Empty, GetServiceInfo()));
                    break;

                case StartProgramModel spm:
                    if (spm.SessionId == uint.MaxValue)
                        spm.SessionId = ApplicationLoader.GetActiveSession();

                    var procc = new DetachedUserProcess(spm, e.Connection);
                    Kernel.StartProcess(procc);
                    break;
                default:
                    break;
            }    
        }

        private void Process_Completed(object sender, EventArgs e)
        {
            DetachedUserProcess process = sender as DetachedUserProcess;
            if (process == null)
                return;
        }

        public ServiceInfo GetServiceInfo()
        {
            return new ServiceInfo()
            {
                ActiveSession = ApplicationLoader.GetActiveSession(), 
                ServiceRevision = Assembly.GetEntryAssembly().GetName().Version,
                ID = this.settings.Guid
            };
        }
    }
}