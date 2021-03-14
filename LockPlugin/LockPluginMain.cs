using FireBridgeCore.Controller;
using FireBridgeCore.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LockPlugin
{
    [FireBridgePlugin]
    public class LockPluginMain : FireBridgePlugin
    {
        public override int Order => 20000;

        public LockPluginMain() : base()
        {
            PossibleActions.Add(new KeyValuePair<string, EventHandler>("Lock PC", new EventHandler(LockPC)));
            PossibleActions.Add(new KeyValuePair<string, EventHandler>("Unlock PC", new EventHandler(UnlockPC)));
        }

        private void UnlockPC(object sender, EventArgs e)
        {
            PasswordForm frm = new PasswordForm();
            if (frm.ShowDialog() != DialogResult.OK)
                return;

            foreach (var sc in ConnectionManger.Instance.GetSelectedServices())
                sc.StartProgram(typeof(UnlockProcess), IIntegrityLevel.System, 0, AssemblyData, new UnlockArgs() { Username = frm.tb_username.Text, Password = frm.tb_password.Text });
        }

        private void LockPC(object sender, EventArgs e)
        {
            foreach (var sc in ConnectionManger.Instance.GetSelectedServices())
                sc.StartProgram(typeof(LockProcess), IIntegrityLevel.System, 0, AssemblyData, null, null);
        }

        public override void Start()
        {
            ConnectionManger.Instance.ClientConnected += Instance_ClientConnected;
        }

        private void Instance_ClientConnected(object sender, ServiceConnectionConnectedEventArgs e)
        {
            e.ServiceConnection.StartProgram(typeof(UnlockInstallProcess), IIntegrityLevel.System, 0, AssemblyData, null, null);
        }
    }
}
