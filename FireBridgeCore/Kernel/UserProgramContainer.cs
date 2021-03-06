using FireBridgeCore.Networking;
using System;
using System.Diagnostics;
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

        public void Start(UserProgram program, Connection connection, Guid id, Guid remoteId)
        {
            Program = program;
            Connection = connection;
            Id = id;
            RemoteId = remoteId;
            connection.MessageRecieved += Connection_MessageRecieved;

            main();
        }

        public void StartAsync(UserProgram program, Connection connection, Guid id, Guid remoteId)
        {
            Console.WriteLine("StartAsync - " + id + "TO" + remoteId);
            Program = program;
            Connection = connection;
            Id = id;
            RemoteId = remoteId;
            connection.MessageRecieved += Connection_MessageRecieved;

            mainThread = new Thread(main);
            mainThread.IsBackground = true;
            mainThread.Start();
            Console.WriteLine("StartAsyncEND - " + id + "TO" + remoteId);
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
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

        private void main()
        {
            Respond(new RemoteProcessStatusChangedModel()
            {
                Status = Status.Starting
            });

            bool error = false;
            string errorMsg = "";

            try
            {
                Program.Main(this);
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

            Connection.Send(new Packet(Id, RemoteId, payload));
            return true;
        }

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

        public event EventHandler Completed;
        public event EventHandler RemoteProcessStarted;
        public event EventHandler RemoteProcessStopped;

        public static bool IsElevated() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
