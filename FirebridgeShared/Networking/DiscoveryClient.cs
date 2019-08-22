using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FirebridgeShared.Networking
{
    public class DiscoveryClient
    {
        UdpClient Client;
        IPEndPoint ServerEp;
        byte[] RequestData = Encoding.ASCII.GetBytes("FireBridge Ping");
        public DiscoveryClient()
        {
            Client = new UdpClient();
            Client.EnableBroadcast = true;
            ServerEp = new IPEndPoint(IPAddress.Any, 8888);
        }
        public void End()
        {
            Client.Close();
        }
        public void Run()
        {
            Task.Run(() => loop());
        }
        private void loop()
        {
            Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
            while (true)
            {
                var ServerResponseData = Client.Receive(ref ServerEp);
                var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                if (ServerResponse == "FireBridge Pong")
                    this.OnConnectionDisconnected(new ClientRespondedEventArgs(ServerEp.Address.ToString()) { });
            }

            //

            //int count = 0;
            //while (Client.Available > 0)
            //{
            //    var ServerResponseData = Client.Receive(ref ServerEp);
            //    var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
            //    Console.WriteLine("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString());
            //    count++;
            //}
            //Console.WriteLine(count);

            //Client.Close();
        }


        public void SendPing()
        {
            Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
        }

        protected virtual void OnConnectionDisconnected(ClientRespondedEventArgs e)
        {
            ClientResponded?.BeginInvoke(this, e, null, null);
        }
        public event EventHandler ClientResponded;
    }
}
