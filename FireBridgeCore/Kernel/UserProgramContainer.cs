using FireBridgeCore.Networking;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel
{
    public class UserProgramContainer
    {
        public Connection Connection { get; private set; }
        public Guid Id { get; private set; }
        public Guid RemoteId { get; private set; }
        public UserProgram Program { get; private set; }
        protected Thread mainThread { get; private set; }

        public void Start(UserProgram program, object args, Connection connection, Guid id, Guid remoteId)
        {
            Program = program;
            Connection = connection;
            Id = id;
            RemoteId = remoteId;
            connection.MessageRecieved += Connection_MessageRecieved;

            main(args);
        }

        public void StartAsync(UserProgram program, object args, Connection connection, Guid id, Guid remoteId)
        {
            Console.WriteLine("StartAsync - " + id + "TO" + remoteId);
            Program = program;
            Connection = connection;
            Id = id;
            RemoteId = remoteId;
            connection.MessageRecieved += Connection_MessageRecieved;

            mainThread = new Thread(new ParameterizedThreadStart(main));
            mainThread.IsBackground = true;
            mainThread.Start(args);
            Console.WriteLine("StartAsyncEND - " + id + "TO" + remoteId);
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            byte[] data = e.Message.Payload as byte[];
            if (data == null)
                return;

            var ms = new MemoryStream(data);
            var obj = new BinaryFormatter().Deserialize(ms);
            e.Message.Payload = obj;

            var rp = e.Message.Payload as RemoteProcessStatusChangedModel;

            if (rp != null)
            {
                if (rp.Status == Status.Starting)
                {
                    RemoteId = e.Message.From;
                    OnRemoteProcessStarted(EventArgs.Empty);
                }
                else
                {
                    RemoteId = Guid.Empty;
                    OnRemoteProcessStopped(EventArgs.Empty);
                }
            }

            Program?.OnDataRecieved(e.Message);
        }

        private void main(object args)
        {
            Respond(new RemoteProcessStatusChangedModel()
            {
                Status = Status.Starting
            });

            bool error = false;
            string errorMsg = "";

            try
            {
                Program.Main(this, args);
            }
            catch (Exception e)
            {
                error = true;
                errorMsg = e.Message;
            }

            Program.OnEnding();
            OnCompleted(EventArgs.Empty);

            Respond(new RemoteProcessStatusChangedModel()
            {
                Status = Status.Stopping,
                Errored = error,
                ErrorMessage = errorMsg
            });
        }

        public bool Respond(object payload)
        {
            if (RemoteId == Guid.Empty)
                return false;

            var ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, payload);

            Connection.Send(new Packet(Id, RemoteId, ms.ToArray()));
            return true;
        }

        private void OnCompleted(EventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        private void OnRemoteProcessStarted(EventArgs e)
        {
            RemoteProcessStarted?.Invoke(this, e);
        }

        private void OnRemoteProcessStopped(EventArgs e)
        {
            RemoteProcessStopped?.Invoke(this, e);
        }

        public event EventHandler Completed;
        public event EventHandler RemoteProcessStarted;
        public event EventHandler RemoteProcessStopped;
    }
}
