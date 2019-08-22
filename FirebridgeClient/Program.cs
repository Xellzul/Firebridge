using FirebridgeShared.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirebridgeClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.num+
        /// </summary>
        [STAThread]
        static void Main()
        {
            var Client = new UdpClient();
            var RequestData = Encoding.ASCII.GetBytes("FireBridge Ping");
            var ServerEp = new IPEndPoint(IPAddress.Any, 0);

            Client.EnableBroadcast = true;
            Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

            Thread.Sleep(500);
            int count = 0;
            while(Client.Available > 0)
            {
                var ServerResponseData = Client.Receive(ref ServerEp);
                var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                Console.WriteLine("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString());
                count++;
            }
            Console.WriteLine(count);

            Client.Close();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
