﻿using FireBridgeCore.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Controller
{
    public class AgentConnection : TCPConnection
    {
        public AgentConnection(Guid guid) : base(guid) { }
        public AgentConnection() { }

        public AgentInfo AgentInfo { private set; get; }
        protected override void PostStart()
        {
            RequestInfo();
            Task.Delay(2500).ContinueWith(t => CheckForConnected(0));
        }

        private void CheckForConnected(int tries)
        {
            if (AgentInfo != null)
                return;

            if(tries > 5)
            {
                Close();
                return;
            }

            RequestInfo();
            Task.Delay(2500).ContinueWith(t => CheckForConnected(tries+1));
        }

        private void RequestInfo()
        {
            Send(new Packet()
            {
                ToPort = -1,
                FromPort = -1,
                Payload = new RequestInfo()
            });
        }

        protected override void Receiving(Packet packet)
        {
            if (packet == null || packet.Payload == null)
                return;

            if(packet.ToPort == -1)
            {
                if(packet.Payload is AgentInfo ai)
                {
                    AgentInfo = ai;
                    if(Status == ConnectionStatus.Connecting)
                        Status = ConnectionStatus.Connected;
                }
                return;
            }

            base.Receiving(packet);
        }
    }
}