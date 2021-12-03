using FireBridgeCore.Kernel;
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
    public class OverrideProgramController : UserProgram
    {
        UserProgramContainer _container;
        private bool shouldEnd = false;
        public override void Main(UserProgramContainer container, object args)
        {
            container.RemoteProcessStopped += Container_RemoteProcessStopped;
            _container = container;
            while (container.Connection.Status == ConnectionStatus.Connected && !shouldEnd)
                Thread.Sleep(1000);
        }

        private void Container_RemoteProcessStopped(object sender, EventArgs e)
        {
            shouldEnd = true;
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

        public override void OnEnding()
        {
            Ending?.Invoke(this, EventArgs.Empty);
            base.OnEnding();
        }

        private void OnImageRecieved(ImageRecievedEventArgs e) => ImageRecieved?.Invoke(this, e);

        public event EventHandler<ImageRecievedEventArgs> ImageRecieved;
        public event EventHandler Ending;

        internal void ChangeSettings(OverridePorgramSettings overridePorgramSettings) => _container?.Respond(overridePorgramSettings);
        internal void ChangeKeyboard(KeyboardEvent keyboardEvent) => _container?.Respond(keyboardEvent);
        internal void ChangeMouse(MouseEvent mouseEvent) => _container?.Respond(mouseEvent);
    }
}
