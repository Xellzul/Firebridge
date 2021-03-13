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
    public class LockPlugin : FireBridgePlugin
    {
        public override int Order => 20000;

        public LockPlugin() : base()
        {
            PossibleActions.Add(new KeyValuePair<string, EventHandler>("Lock PC", new EventHandler(LockPC)));
            PossibleActions.Add(new KeyValuePair<string, EventHandler>("Unlock PC", new EventHandler(UnlockPC)));
        }

        private void UnlockPC(object sender, EventArgs e)
        {
            PasswordForm frm = new PasswordForm();
            if (frm.ShowDialog() != DialogResult.OK)
                return;

            /*
            foreach (var sc in ConnectionManger.Instance.GetSelectedServices())
                sc.StartProgram(item.GetAgent(IIntegrityLevel.System, 0), new UnlockPcProcess() { 
                Password = frm.tb_password.Text, Username = frm.tb_username.Text });*/
        }
        private void LockPC(object sender, EventArgs e)
        {
            foreach (var sc in ConnectionManger.Instance.GetSelectedServices())
                sc.StartProgram(typeof(LockProcess), AssemblyData);
        }

        public override void Start()
        {

        }
    }
}
