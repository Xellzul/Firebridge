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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            DiscoveryClient DiscoveryClient = new DiscoveryClient();
            DiscoveryClient.ClientResponded += DiscoveryClient_ClientResponded;
            DiscoveryClient.Run();
            Console.ReadKey();
        }

        private static void DiscoveryClient_ClientResponded(object sender, EventArgs e)
        {
            var a = (ClientRespondedEventArgs)e;
            new Form1(a.Ip).ShowDialog();
        }
    }
}
