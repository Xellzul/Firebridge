//using Firebridge.Core.Networking;
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
