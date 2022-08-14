﻿//using Firebridge.Core.Networking;
//using FireBridgeCore;
//using FireBridgeCore.Kernel;
//using FireBridgeCore.Networking;
//using NetFwTypeLib;
//using System;
//using System.IO;
//using System.Reflection;
//using System.ServiceProcess;

//namespace FireBridgeService
//{
//    class FireBridgeService : ServiceBase
//    {
//        UserKernel Kernel;
//        TCPServer TCPServer;
//        DiscoveryServer DiscoveryServer;

//        Guid MachineId = MachineFingerprintGenerator.GetFingerprint();

//        public FireBridgeService()
//        {
//            this.ServiceName = "FireBridge";
//            CanStop = true;
//            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

//            DiscoveryServer = new DiscoveryServer(MachineId, "FireBridgeSecret", 47000);
//        }

//        private static bool AddFirewallException()
//        {
//            try
//            {
//                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
//                    Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

//                if (firewallPolicy.Rules.Item("FireBridge") != null)
//                    return true;

//                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
//                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
//                firewallRule.Description = "FireBridgeService";
//                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
//                firewallRule.Enabled = true;
//                firewallRule.InterfaceTypes = "All";
//                firewallRule.Name = "FireBridge";
//                firewallPolicy.Rules.Add(firewallRule);
//            }
//            catch (Exception)
//            {
//                //TODO: Log
//                return false;
//            }
//            return true;
//        }

//        protected override void OnStop()
//        {
//            DiscoveryServer.Stop();
//            Kernel.Stop();
//            TCPServer.Stop();
//            base.OnStop();
//        }

//        public void Starting()
//        {
//            AddFirewallException();

//            Kernel = new UserKernel();
//            TCPServer = new TCPServer();

//            TCPServer.ClientConnecting += TCPServer_ClientConnecting;
//            TCPServer.Start(Constants.DiscoveryPort);
//            //_ = DiscoveryServer.RunAsync();
//        }

//        protected override void OnStart(string[] args)
//        {
//            Starting();
//            base.OnStart(args);
//        }

//        private void TCPServer_ClientConnecting(object sender, ClientConnectedEventArgs e)
//        {
//            e.Connection.MessageRecieved += Connection_MessageRecieved;
//        }

//        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
//        {
//            switch(e.Message.Payload)
//            {
//                case RequestInfo:
//                    e.Connection.Send(new Packet(MachineId, Guid.Empty, GetServiceInfo()));
//                    break;

//                case StartProgramModel spm:
//                    if (spm.SessionId == uint.MaxValue)
//                        spm.SessionId = ApplicationLoader.GetActiveSession();

//                    var procc = new DetachedUserProcess(spm, e.Connection);
//                    Kernel.StartProcess(procc);
//                    break;
//                default:
//                    break;
//            }    
//        }

//        private void Process_Completed(object sender, EventArgs e)
//        {
//            DetachedUserProcess process = sender as DetachedUserProcess;
//            if (process == null)
//                return;
//        }

//        public ServiceInfo GetServiceInfo()
//        {
//            return new ServiceInfo()
//            {
//                Agents = Kernel.GetReport(),
//                Controllers = TCPServer.GetReport(),
//                ActiveSession = ApplicationLoader.GetActiveSession(), 
//                ServiceVersion = Assembly.GetEntryAssembly().GetName().Version,
//                IntegrityLevel = IntegrityLevelHelper.GetCurrentIntegrity().ToString(),
//                Id = MachineId
//            };
//        }
//    }
//}

using Microsoft.Extensions.Hosting;

namespace FireBridge.Win.Service;

public class FireBridgeService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        Environment.Exit(1);
    }
}
