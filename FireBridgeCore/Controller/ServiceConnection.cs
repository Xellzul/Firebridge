using FireBridgeCore.Networking;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FireBridgeCore.Kernel;

namespace FireBridgeCore.Controller
{
    public class ServiceConnection : TCPConnection
    {
        public ServiceInfo ServiceInfo { private set; get; }

        private UserKernel _kernel;

        public ServiceConnection(Guid serviceId) : base(serviceId)
        {
            _kernel = new UserKernel();
        }

        protected override void PostStart()
        {
            RequestInfo();
            Task.Delay(2500).ContinueWith(t => CheckForConnected(0));
        }

        private void RequestInfo()
        {
            Send(new Packet(Guid.Empty, Guid.Empty, new RequestInfo()));
        }
        private void CheckForConnected(int tries)
        {
            if (ServiceInfo != null)
                return;

            if (tries > 5)
            {
                Close();
                return;
            }

            RequestInfo();
            Task.Delay(2500).ContinueWith(t => CheckForConnected(tries + 1));
        }
        protected override void Receiving(Packet packet)
        {
            if (packet == null || packet.Payload == null)
                return;

            if (packet.To == Guid.Empty)
            {
                if (packet.Payload is ServiceInfo si)
                {
                    ServiceInfo = si;
                    Status = ConnectionStatus.Connected;

                }
                return;
            }

            base.Receiving(packet);
        }

        public void StartProgram(Type remoteProgram, UserProgram localProgram = null)
        {
            var remoteGuid = Guid.NewGuid();
            var localGuid = localProgram == null ? Guid.Empty : Guid.NewGuid();
            var toSend = new Packet(localGuid, remoteGuid, new StartProgramModel()
            {
                SessionId = uint.MaxValue,
                Type = remoteProgram.ToString(),
                ProcessId = remoteGuid,
                RemoteId = localGuid,
                IntegrityLevel = IIntegrityLevel.Medium // todo CHANGE
            });


            if (localProgram != null)
            {
                _kernel.StartProcessAttached(localProgram, this, localGuid, remoteGuid);
            }

            Send(toSend);
        }
    }
}
