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

        public ConcurrentDictionary<Guid, AgentConnection> _agents;
        public ConcurrentDictionary<Guid, AgentConnection> _pendingAgents;

        public ServiceConnection(Guid serviceId) : base(serviceId)
        {
            _pendingAgents = new ConcurrentDictionary<Guid, AgentConnection>();
            _agents = new ConcurrentDictionary<Guid, AgentConnection>();
            _kernel = new UserKernel();
        }

        protected override void PostStart()
        {
            RequestInfo();
            Task.Delay(2500).ContinueWith(t => CheckForConnected(0));
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

            if (packet.ToPort == -1)
            {
                if (packet.Payload is ServiceInfo si)
                {
                    ServiceInfo = si;

                    lock (_pendingAgents)
                    {
                        foreach (var agent in si.Agents)
                        {
                            if (_pendingAgents.ContainsKey(agent.AgentID) || _agents.ContainsKey(agent.AgentID))
                                continue;

                            AddConnectingAgent(agent.Port, agent.AgentID);
                        }
                    }
                }
                return;
            }

            base.Receiving(packet);
        }

        public AgentConnection GetAgent(IIntegrityLevel il, uint SessionID)
        {
            var agnt = _agents.Where(
                x => x.Value.AgentInfo.IntegrityLevel == il && 
                x.Value.AgentInfo.SessionID == SessionID
            ).Select(x => (KeyValuePair<Guid, AgentConnection>?)x).FirstOrDefault();

            return agnt?.Value;
        }

        private void AddConnectingAgent(int port, Guid id)
        {
            //TODO: AgentConnecting Event
            AgentConnection ac = new AgentConnection(id);

            //IPAddress, item.Port
            ac.ConnectionStatusChanged += Ac_Pending_ConnectionStatusChanged;
            ac.MessageRecieved += Ac_MessageRecieved;
            _pendingAgents.TryAdd(ac.Id, ac);

            ac.Start(IPAddress, port);
        }

        private void Ac_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            _kernel.SendToProcess(e.Connection, e.Message);
        }

        private void Ac_Pending_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (e==null || sender == null || !(sender is AgentConnection))
                return;

            var agent = (AgentConnection)sender;

            if (e.Now == ConnectionStatus.Connected)
            {
                agent.ConnectionStatusChanged -= Ac_Pending_ConnectionStatusChanged;
                lock (_pendingAgents)
                {
                    //todo: Agent Connected?
                    _agents.TryAdd(agent.Id, agent);
                    _pendingAgents.TryRemove(agent.Id, out _);

                    if (_pendingAgents.Count == 0 && Status != ConnectionStatus.Disconnected)
                        Status = ConnectionStatus.Connected;
                }
            }

            if (e.Now == ConnectionStatus.Disconnected)
            {
                lock (_pendingAgents)
                {
                    //todo: Agent disconnected?
                    _pendingAgents.TryRemove(agent.Id, out _);
                    if(ServiceInfo.Agents.Any(x => x.AgentID == agent.Id)) 
                        AddConnectingAgent(agent.Port, agent.Id);
                }
            }
        }

        public void StartProgram(AgentConnection ac, UserProcess remoteProcess, UserProcess localProcess = null)
        {
            var toSend = new Packet()
            {
                ToPort = -1,
                FromPort = -1,
                Payload = remoteProcess
            };

            if (localProcess != null)
            {
                _kernel.StartProcess(ac, localProcess);
                toSend.FromPort = localProcess.ProcessID;
                remoteProcess.RemoteProcessID = localProcess.ProcessID;
            }

            ac.Send(toSend);
        }
    }
}
