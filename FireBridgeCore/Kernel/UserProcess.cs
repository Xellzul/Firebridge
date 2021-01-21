using FireBridgeCore.Networking;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel
{
    [Serializable]
    public abstract class UserProcess
    {
        [field: NonSerialized]
        protected Thread mainThread { get; private set; }
        public int ProcessID { get; private set; } = int.MaxValue;
        public int RemoteProcessID { get; set; } = int.MaxValue;
        [field: NonSerialized]
        public Connection RemoteConnection { get; private set; }
        [field: NonSerialized]
        protected UserKernel Kernel { get; private set; }

        public void Start(
            UserKernel kernel, int processID, 
            Connection remoteConnection)
        {
            Kernel = kernel;
            RemoteConnection = remoteConnection;
            ProcessID = processID;

            mainThread = new Thread(main);
            mainThread.IsBackground = true;
            mainThread.Start();
        }

        public void main()
        {
            Respond(new RemoteProcessStatusChangedModel()
            {
                Status = Status.Starting
            });

            bool error = false;
            string errorMsg = "";

            try
            {
                Main();
            }
            catch (Exception e)
            {
                error = true;
                errorMsg = e.Message;
            }

            OnCompleted(EventArgs.Empty);

            Respond(new RemoteProcessStatusChangedModel()
            {
                Status = Status.Stopping,
                Errored = error,
                ErrorMessage = errorMsg
            });
        }

        internal void DataRecieved(Connection connection, Packet message)
        {
            var rp = message.Payload as RemoteProcessStatusChangedModel;

            if (rp != null)
            {
                if (rp.Status == Status.Starting)
                {
                    RemoteProcessID = message.FromPort;
                    OnRemoteProcessStarted(EventArgs.Empty);
                }
                else
                {
                    RemoteProcessID = int.MaxValue;
                    OnRemoteProcessStopped(EventArgs.Empty);
                }
            }

            OnMessageRecieved(new MessageRecievedEventArgs() {
                Connection = connection, Message = message });
        }
        protected void Respond(object payload)
        {
            if(RemoteProcessID != Packet.Broadcast)
            {
                Console.WriteLine("RESPONDING" + payload.GetType());
                RemoteConnection.Send(new Packet() {
                    FromPort = ProcessID, 
                    ToPort = RemoteProcessID,
                    Payload = payload
                });
            }
        }
        public abstract void Main();

        private void OnCompleted(EventArgs e)
        {
            if (Completed == null)
                return;

            foreach (EventHandler reciever in Completed.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }

        private void OnRemoteProcessStarted(EventArgs e)
        {
            if (RemoteProcessStarted == null)
                return;

            foreach (EventHandler reciever in RemoteProcessStarted.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }

        private void OnRemoteProcessStopped(EventArgs e)
        {
            if (RemoteProcessStopped == null)
                return;

            foreach (EventHandler reciever in RemoteProcessStopped.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }

        private void OnMessageRecieved(MessageRecievedEventArgs e)
        {
            if (MessageRecieved == null)
                return;

            foreach (EventHandler<MessageRecievedEventArgs> reciever in MessageRecieved.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }

        [field: NonSerialized]
        public event EventHandler Completed;
        [field: NonSerialized]
        public event EventHandler RemoteProcessStarted;
        [field: NonSerialized]
        public event EventHandler RemoteProcessStopped;
        [field: NonSerialized]
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
    }
}