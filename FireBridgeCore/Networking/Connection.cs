using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Networking
{
    public abstract class Connection : Connectable
    {

        protected bool _shouldEnd = false;
        protected Stream _stream;
        protected Thread _readerThread;

        public virtual bool Send(Packet packet)
        {
            if (!_stream.CanWrite)
                return false;

            lock(_stream)
            {
                try
                {
                    Console.WriteLine("Sending Packet:" + packet.ToString());

                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(_stream, packet);
                }
                catch(Exception e) {
                    Console.WriteLine("Error sending packet: " + e.Message);
                    return false; 
                }
                return true;
            }
        }

        protected void ReadLoop()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            while (_stream.CanRead && !_shouldEnd)
            {
                try
                {
                    Packet p = binaryFormatter.Deserialize(_stream) as Packet;

                    if (_stream.CanRead == false || p == null || p.Payload == null)
                        continue;

                    Console.WriteLine("Reading packet: " + p.ToString());

                    Receiving(p);
                }
                catch (Exception e)
                {
                    _shouldEnd = true;
                    Console.WriteLine("Error reading: " + e.Message);
                }
            }

            Status = ConnectionStatus.Disconnected;
        }
        protected virtual void Receiving(Packet packet)
        {
            OnMessageRecieved(
                new MessageRecievedEventArgs() { 
                    Connection = this, 
                    Message=packet }
                );
        }
        
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        protected virtual void OnMessageRecieved(MessageRecievedEventArgs e)
        {
            if (MessageRecieved == null)
                return;

            foreach (EventHandler<MessageRecievedEventArgs> reciever in MessageRecieved.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }

         public abstract void Close();
       // protected abstract void close();
       /* public void Close()
        {
            Status = ConnectionStatus.Disconnected;
            _shouldEnd = true;
            close();
        }*/
    }
}
