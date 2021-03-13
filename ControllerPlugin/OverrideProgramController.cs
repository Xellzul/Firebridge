using FireBridgeCore.Kernel;
using FireBridgeCore.Kernel.UserPrograms;
using FireBridgeCore.Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerPlugin
{
    [Serializable]
    public class OverrideProgramController : UserProgram
    {
        UserProgramContainer _container;
        public override void Main(UserProgramContainer container)
        {
            _container = container;
            while (container.Connection.Status == ConnectionStatus.Connected)
                Thread.Sleep(1000);
        }

        public override void OnDataRecieved(Packet packet)
        {
            switch (packet.Payload)
            {
                case Image i:
                    OnImageRecieved(new ImageRecievedEventArgs() { Image = i });
                    break;
                default:
                    break;
            }

            base.OnDataRecieved(packet);
        }

        private void OnImageRecieved(ImageRecievedEventArgs e)
        {
            if (ImageRecieved == null)
                return;

            foreach (EventHandler<ImageRecievedEventArgs> reciever in ImageRecieved.GetInvocationList())
                Task.Run(() => { reciever.Invoke(this, e); });
        }

        [field: NonSerialized]
        public event EventHandler<ImageRecievedEventArgs> ImageRecieved;

        internal void ChangeSettings(OverridePorgramSettings overridePorgramSettings)
        {
            _container?.Respond(overridePorgramSettings);
        }
    }
}
