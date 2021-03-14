using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
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
            ServerEp = new IPEndPoint(IPAddress.Any, 8888);
            Client = new UdpClient(ServerEp);
            Client.EnableBroadcast = true;
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
            while (_run)
            {
                try
                {
                    IPEndPoint readerEP = new IPEndPoint(IPAddress.Any, 8888);
                    var ServerResponseData = Client.Receive(ref readerEP);
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
                                readerEP.Address,
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
            //https://stackoverflow.com/questions/1096142/broadcasting-udp-message-to-all-the-available-network-cards
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up && ni.SupportsMulticast && ni.GetIPProperties().GetIPv4Properties() != null)
                {
                    int id = ni.GetIPProperties().GetIPv4Properties().Index;
                    if (NetworkInterface.LoopbackInterfaceIndex != id)
                    {
                        foreach (UnicastIPAddressInformation uip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (uip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                IPEndPoint local = new IPEndPoint(uip.Address, 0);
                                UdpClient udpc = new UdpClient(local);
                                udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                                udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);

                                udpc.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8889));
                            }
                        }
                    }
                }
            }
        }

        public void OnClientResponded(ClientRespondedEventArgs e)
        {
            ClientResponded?.Invoke(this, e);
        }
        public event EventHandler<ClientRespondedEventArgs> ClientResponded;
    }
}