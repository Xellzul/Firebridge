using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FirebridgeShared.Networking
{
    public class ServerConnection
    {

        private TcpClient client;
        BinaryFormatter bf = new BinaryFormatter();

        public ServerConnection(TcpClient client)
        {
            this.client = client;
        }

        public void SendPacket(Packet packet)
        {
            bf.Serialize(client.GetStream(), packet);
        }

        public void Run()
        {
            var stream = client.GetStream();
            while (client.Connected)
            {
                if (!client.Connected && !stream.DataAvailable)
                    return;
                Packet p;
                try
                {
                    p = (Packet)bf.Deserialize(stream);
                }
                catch(IOException e)
                {
                    if (((SocketException)e.InnerException)?.ErrorCode == 10054)
                        return; //TODO: 
                    throw;
                }

                switch (p.Id)
                {
                    case 0: //Message
                        Console.WriteLine((string)p.Data);
                        SendPacket(new Packet() { Id = 0, Data = "GOT " + p.Data });
                        break;
                    case 1: //Run Code
                        break;
                    default:
                        Console.WriteLine("Unknown Packet of " + p.Id);
                        SendPacket(new Packet() { Id = 0, Data = "Unknown Packet of " + p.Id });
                        break;
                }
            }
        }
    }
}
