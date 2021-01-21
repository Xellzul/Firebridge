using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public class DiscoveryClient 
    {
        private UdpClient Client;
        private IPEndPoint ServerEp;
        private byte[] RequestData = Encoding.ASCII.GetBytes("FireBridge Ping");
        private bool _run = true;
        private Thread ListenThread;
        private Thread SenderThread;

        public DiscoveryClient()
        {
            Client = new UdpClient();
            Client.EnableBroadcast = true;
            ServerEp = new IPEndPoint(IPAddress.Any, 8888);
        }
        public void End()
        {
            _run = false;
            Client.Close();
        }

        public void Run()
        {
            ListenThread = new Thread(loop);
            ListenThread.Start();

            SenderThread = new Thread(Sender);
            SenderThread.Start();
        }

        private void Sender()
        {
            while (_run)
            {
                SendPing();
                Thread.Sleep(1000);
            }
        }

        private void loop()
        {
            Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
            while (_run)
            {
                try
                {
                    var ServerResponseData = Client.Receive(ref ServerEp);
                    var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);

                    Guid id;
                    try
                    {
                        id = Guid.Parse(
                            ServerResponse.Split("FireBridge Pong |")[1]
                            );

                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (ServerResponse.StartsWith("FireBridge Pong |"))
                        this.OnClientResponded(
                            new ClientRespondedEventArgs(
                                ServerEp.Address, 
                                id
                           ));
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Firbridge Discovery error: " + e.Message);
                }
            }
        }

        public void SendPing()
        {
            Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
        }

        public void OnClientResponded(ClientRespondedEventArgs e)
        {
            if (ClientResponded == null)
                return;

            foreach (EventHandler<ClientRespondedEventArgs> reciever in ClientResponded.GetInvocationList())
                Task.Run(() => {
                    reciever.Invoke(this, e);
                });
        }
        public event EventHandler<ClientRespondedEventArgs> ClientResponded;
    }
}
