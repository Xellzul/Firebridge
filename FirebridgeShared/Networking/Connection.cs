using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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
            init();
        }

        public Connection(IPEndPoint iPEndPoint)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(iPEndPoint);
            init();
        }

        private void init()
        {
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
                    if (((SocketException)e.InnerException)?.ErrorCode == 10054 || ((SocketException)e.InnerException)?.ErrorCode == 10060)
                    {
                        OnDisconnected(EventArgs.Empty);
                        return; //TODO:
                    }
                    throw;
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Invalid Data: " + e.Message);

                    while (stream.DataAvailable)
                        stream.ReadByte();

                    continue;
                }
                OnMessageRecievedConnected(new MessageEventArgs(p));
            }
        }

        public void SendPacket(Packet p)
        {
            try
            {
                bf.Serialize(stream, p);
            }
            catch (IOException e)
            {
                if (((SocketException)e.InnerException)?.ErrorCode == 10054 || ((SocketException)e.InnerException)?.ErrorCode == 10060)
                {
                    OnDisconnected(EventArgs.Empty);
                    return; //TODO:
                }
                throw;
            }
        }

        protected virtual void OnMessageRecievedConnected(MessageEventArgs e)
        {
            MessageRecieved.SafeInvoke(this, e);
            //MessageRecieved?.Invoke(this, e);
        }

        protected virtual void OnDisconnected(EventArgs e)
        {
            MessageRecieved.SafeInvoke(this, e);
            //Disconnected?.Invoke(this, e);
        }

        public event EventHandler Disconnected;
        public event EventHandler MessageRecieved;
    }
}
