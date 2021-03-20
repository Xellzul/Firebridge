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
            Program = program;
            Connection = connection;
            Id = id;
            RemoteId = remoteId;
            connection.MessageRecieved += Connection_MessageRecieved;

            mainThread = new Thread(new ParameterizedThreadStart(main));
            mainThread.IsBackground = true;
            mainThread.Start(args);
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch (e.Message.Payload)
            {
                case RemoteProcessStatusChangedModel rp:
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
                    break;
                case ProgramMessageModel pm:
                    var ms = new MemoryStream(pm.Data);
                    var obj = new BinaryFormatter().Deserialize(ms);
                    e.Message.Payload = obj;
                    break;
                default:
                    break;
            }

            Program?.OnDataRecieved(e.Message);
        }

        private void main(object args)
        {
            RespondRaw(new RemoteProcessStatusChangedModel()
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

            Console.WriteLine("Ending - " + errorMsg);

            Program.OnEnding();
            OnCompleted(EventArgs.Empty);

            RespondRaw(new RemoteProcessStatusChangedModel()
            {
                Status = Status.Stopping,
                Errored = error,
                ErrorMessage = errorMsg
            });

            Connection.Flush();
        }

        public bool RespondRaw(object payload) => Connection.Send(new Packet(Id, RemoteId, payload));

        public bool Respond(object payload)
        {
            var ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, payload);

            return RespondRaw(new ProgramMessageModel() { Data = ms.ToArray() });
        }

        private void OnCompleted(EventArgs e) => Completed?.Invoke(this, e);
        private void OnRemoteProcessStarted(EventArgs e) => RemoteProcessStarted?.Invoke(this, e);
        private void OnRemoteProcessStopped(EventArgs e) => RemoteProcessStopped?.Invoke(this, e);

        public event EventHandler Completed;
        public event EventHandler RemoteProcessStarted;
        public event EventHandler RemoteProcessStopped;
    }
}
