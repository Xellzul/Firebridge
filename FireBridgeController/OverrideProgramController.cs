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

namespace FireBridgeController
{
    [Serializable]
    public class OverrideProgramController : UserProcess
    {
        public override void Main()
        {
            this.MessageRecieved += OverrideProgramController_MessageRecieved;
            while (this.RemoteConnection.Status == ConnectionStatus.Connected)
            {
                Thread.Sleep(1000);
            }
        }

        private void OverrideProgramController_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            switch(e.Message.Payload)
            {
                case Image i:
                    OnImageRecieved(new ImageRecievedEventArgs() { Image = i });
                    break;
                default:
                    break;
            }
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
            Respond(overridePorgramSettings);
        }
    }
}
