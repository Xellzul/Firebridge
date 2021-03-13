using FireBridgeCore.Controller;
using FireBridgeCore.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerPlugin
{
    [FireBridgePlugin]
    public class ControllerMain : FireBridgePlugin
    {
        public override int Order => 100;

        FireBridgeControllerMenu fireBridgeControllerMenu;
        Thread formThread;

        public override void Start()
        {
            formThread = new Thread(startForm);
            formThread.SetApartmentState(ApartmentState.STA);
            formThread.Start();
            Thread.Sleep(100);
        }

        private void startForm()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            fireBridgeControllerMenu = new FireBridgeControllerMenu(this);
            foreach (var plugin in PluginManager.Instance.Plugins)
            {
                foreach (var action in plugin.PossibleActions)
                {
                    fireBridgeControllerMenu.AddAction(action.Key, action.Value);
                }
            }

            Application.Run(fireBridgeControllerMenu);
        }
    }
}
