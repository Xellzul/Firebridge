using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public class DiscoveryServer
    {
        private Thread _thread;
        private UdpClient Server;
        private Guid _id;
        public DiscoveryServer(Guid id)
        {
            _id = id;
            Server = new UdpClient(8888);
        }

        private void loop()
        {
            var ResponseData = Encoding.ASCII.GetBytes("FireBridge Pong |" + _id.ToString());
            while (true)
            {
                var iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                var data = Server.Receive(ref iPEndPoint);
                var dataASCII = Encoding.ASCII.GetString(data);

                if (dataASCII == "FireBridge Ping")
                    Server.Send(ResponseData, ResponseData.Length, iPEndPoint);
            }
        }

        public void Run()
        {
            _thread = new Thread(() => loop());
            _thread.Start();
        }

        public void Stop()
        {
            Server.Close();
        }
    }
}
