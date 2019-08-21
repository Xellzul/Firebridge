using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FirebridgeShared;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using FirebridgeShared.Networking;
using System.Net;

namespace FireBridgeTestAPP
{
    class Program
    {
        static string[] GetCode()
        {
            return new string[]
            {
                @"using System;
 
                namespace DynamicNS
                {
                    public static class DynamicCode
                    {
                        public static void Main()
                        {
                            Console.WriteLine(""Hello, world!"");
                        }
                    }
                }"
            };
        }

        static void Main(string[] args)
        {
            //CSharpCodeProvider provider = new CSharpCodeProvider();
            //CompilerParameters parameters = new CompilerParameters();
            //parameters.GenerateInMemory = true;
            //parameters.ReferencedAssemblies.Add("System.dll");
            //CompilerResults results = provider.CompileAssemblyFromSource(parameters, GetCode());
            //results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));

            //var cls = results.CompiledAssembly.GetType("DynamicNS.DynamicCode");
            //var method = cls.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);
            //method.Invoke(null, null);



            Server s = new Server();
            s.ClientConnected += S_ClientConnected;
            s.ConnectionDisconnected += S_ConnectionDisconnected;
            s.Start(new IPEndPoint(IPAddress.Any, 6969));
            Console.ReadKey();
        }

        private static void S_ConnectionDisconnected(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("CLIENT DISCONNECTED");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void S_ClientConnected(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("CLIENT CONNECTED");
            Console.ForegroundColor = ConsoleColor.White;
            var client = ((ServerConnectionEventArgs)e).Connection;
            client.SendPacket(new Packet() { Id = 0, Data = "Ahoj, pripojil ses ke mne, ODESLANO OD ZOMBIE" });
            client.MessageRecieved += Client_MessageRecieved;
        }

        private static void Client_MessageRecieved(object sender, EventArgs e)
        {
            var ea = (MessageEventArgs)e;
            var se = (Connection)sender;
            se.SendPacket(new Packet() { Id = 0, Data = "prisla mi zprava, ODESLANO OD ZOMBIE" });

            switch (ea.Packet.Id)
            {
                case 0: //Message
                    Console.WriteLine((string)ea.Packet.Data);
                    se.SendPacket(new Packet() { Id = 0, Data = "GOT " + ea.Packet.Data });
                    break;
                case 1: //Run Code
                    break;
                default:
                    Console.WriteLine("Unknown Packet of " + ea.Packet.Id);
                    se.SendPacket(new Packet() { Id = 0, Data = "Unknown Packet of " + ea.Packet.Id });
                    break;
            }
        }
    }
}
