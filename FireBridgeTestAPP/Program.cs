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
using System.Threading;

namespace FireBridgeTestAPP
{
    class Program
    {
        static string[] GetCode()
        {
            return new string[]
            {
                @"using System;
                using FirebridgeShared.Networking;
 
                namespace FireBridgeTestAPP
                {
                    public static class DynamicCode
                    {
                        public static void Main(Server s)
                        {

                            s.ClientConnected += S_ClientConnected;
                            Console.WriteLine(""Hello, world!"");
                            Console.ReadKey();

                        }
                        private static void S_ClientConnected(object sender, EventArgs e)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(""CLIENT CONNECTED FROM COMPILED CODE"");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
    }"
            };
        }

        static void Main(string[] args)
        {
            Console.WriteLine("START" + Thread.CurrentThread.ManagedThreadId);
            Server s = new Server();
            s.ClientConnected += S_ClientConnected;
            s.ConnectionDisconnected += S_ConnectionDisconnected;
            s.Start(new IPEndPoint(IPAddress.Any, 6969));




            Console.ReadKey();
        }

        private static void S_ConnectionDisconnected(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("CLIENT DISCONNECTED" + Thread.CurrentThread.ManagedThreadId);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void S_ClientConnected(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("CLIENT CONNECTED" + Thread.CurrentThread.ManagedThreadId);
            Console.ForegroundColor = ConsoleColor.White;
            var client = ((ServerConnectionEventArgs)e).Connection;
            client.SendPacket(new Packet() { Id = 0, Data = "Ahoj, pripojil ses ke mne, ODESLANO OD ZOMBIE" });
            client.MessageRecieved += Client_MessageRecieved;
        }

        private static void Client_MessageRecieved(object sender, EventArgs e)
        {
            var ea = (MessageEventArgs)e;
            var se = (Connection)sender;
            Console.WriteLine("GOT MSG" + Thread.CurrentThread.ManagedThreadId);
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
