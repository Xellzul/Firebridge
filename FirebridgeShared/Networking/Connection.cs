using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FirebridgeShared.Networking
{
    public class Connection
    {
        TcpClient tcpClient;
        BinaryFormatter bf;
        NetworkStream stream;
        public Connection(TcpClient client)
        {
            tcpClient = client;
            bf = new BinaryFormatter();
            stream = tcpClient.GetStream();
            Task.Run(() => Read(stream));
        }

        public Connection(IPEndPoint iPEndPoint)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(iPEndPoint);
            bf = new BinaryFormatter();
            stream = tcpClient.GetStream();
            Task.Run(() => Read(stream));
        }

        private void Read(NetworkStream _stream)
        {
            BinaryFormatter _bf = new BinaryFormatter();
            while (true)
            {
                if (!tcpClient.Connected && !stream.DataAvailable)
                    return;
                Packet p;
                try
                {
                    p = (Packet)bf.Deserialize(stream);
                }
                catch (IOException e)
                {
                    if (((SocketException)e.InnerException)?.ErrorCode == 10054)
                        return; //TODO: 
                    throw;
                }
                OnMessageRecievedConnected(new MessageEventArgs(p));
            }
        }

        public void SendPacket(Packet p)
        {
            bf.Serialize(stream, p);
        }

        protected virtual void OnMessageRecievedConnected(MessageEventArgs e)
        {
            MessageRecieved?.Invoke(this, e);
        }

        public event EventHandler MessageRecieved;
    }
}
