using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FirebridgeShared.Networking
{
    public class Client
    {
        TcpClient tcpClient;
        BinaryFormatter bf;
        NetworkStream stream;
        public Client()
        {
        }

        public void Connect()
        {
            tcpClient = new TcpClient("localhost", 6969);
            bf = new BinaryFormatter();
            stream = tcpClient.GetStream();
            Task.Run(() => Read(stream));
        }

        private void Read(NetworkStream _stream)
        {
            BinaryFormatter _bf = new BinaryFormatter();
            while (true) { 
                var packet = (Packet)_bf.Deserialize(_stream);
                OnMessageRecievedConnected(new MessageEventArgs(packet));
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

    public class MessageEventArgs : EventArgs
    {
        public Packet Packet { get; set; }
        public MessageEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }
}
