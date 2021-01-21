using FireBridgeCore.Kernel;
using FireBridgeCore.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeService
{
    internal class AgentManager
    {
        private ConcurrentDictionary<Guid, Agent> Agents { get; set; } = new ConcurrentDictionary<Guid, Agent>();
        private TCPServer _TCPServer;

        public AgentManager()
        {
            _TCPServer = new TCPServer();
            _TCPServer.ClientConnected += _TCPServer_ClientConnected;
            _TCPServer.Start(6970);
        }
        public void StartSession(uint sessionID)
        {
            AddAgent(sessionID, IIntegrityLevel.System);
            AddAgent(sessionID, IIntegrityLevel.High);
            AddAgent(sessionID, IIntegrityLevel.Medium);
            //if(sessionID != 0)
            //    AddAgent(sessionID, IntegrityLevel.Low);
        }

        private void AddAgent(uint sessionID, IIntegrityLevel il)
        {
            var agent = new Agent(sessionID, il);
            agent.ConnectionStatusChanged += Agent_ConnectionStatusChanged;
            Agents.TryAdd(agent.Id, agent);
            agent.StartProcess();
        }

        private void Agent_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (sender == null || e == null || !(sender is Agent))
                return;

            Agent agent = (Agent)sender;
            OnAgentStatusChanged(new AgentStatusChangedEventArgs()
            {
                ConnectionStatus = e.Now,
                Agent = agent
            });

            if(e.Now == ConnectionStatus.Disconnected)
            {
                Close((Agent)sender);
                AddAgent(agent.SessionID, agent.Elevation);
            }
        }

        private void _TCPServer_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Connection.MessageRecieved += Connection_MessageRecieved;
            e.Connection.Send(new Packet() { FromPort = -1, ToPort = -1, Payload = new RequestInfo() });
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            if (e.Message.Payload is AgentInfo ai)
            {
                e.Connection.MessageRecieved -= Connection_MessageRecieved;

                Agents.TryGetValue(ai.AgentID, out Agent agent);
                agent?.Connected(e.Connection, ai);
            }
        }

        public void CloseSession(uint sessionID)
        {
            var toRemove = Agents.Where(x => x.Value.SessionID == sessionID);
            foreach (var ra in toRemove)
                Close(ra.Value);
        }

        public void CloseAll()
        {
            foreach (var ra in Agents)
                Close(ra.Value);
        }

        private void Close(Agent agent)
        {
            agent.ConnectionStatusChanged -= Agent_ConnectionStatusChanged;
            Agents.TryRemove(agent.Id, out _);
            agent.Close();
            OnAgentStatusChanged(new AgentStatusChangedEventArgs { Agent = agent, ConnectionStatus = ConnectionStatus.Disconnected });
        }

        public void Stop()
        {
            CloseAll();
            _TCPServer?.Stop();
        }

        public List<AgentInfo> GetAgents()
        {
            return Agents
                .Where(y => y.Value.Status == ConnectionStatus.Connected)
                .Select(x => x.Value.AgentInfo).ToList();
        }

        protected virtual void OnAgentStatusChanged(AgentStatusChangedEventArgs e)
        {
            if (AgentStatusChanged == null)
                return;

            foreach (EventHandler<AgentStatusChangedEventArgs> reciever in AgentStatusChanged.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }
        public event EventHandler<AgentStatusChangedEventArgs> AgentStatusChanged;
    }
}