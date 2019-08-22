using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FirebridgeShared.Networking
{
    public class DiscoveryServer
    {
        UdpClient Server;
        public DiscoveryServer()
        {
            Server = new UdpClient(8888);
        }

        private void loop()
        {
            var ResponseData = Encoding.ASCII.GetBytes("FireBridge Pong");
            while (true)
            {
                var iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var data = Server.Receive(ref iPEndPoint);
                var dataASCII = Encoding.ASCII.GetString(data);

                Console.WriteLine("Got request from " + iPEndPoint.Address.ToString() + " of " + dataASCII);
                if(dataASCII == "FireBridge Ping")
                    Server.Send(ResponseData, ResponseData.Length, iPEndPoint);
            }
        }

        public void Run()
        {
            Task.Run(() => loop());
        }

        public void Stop()
        {
            Server.Close();
        }
    }
}
