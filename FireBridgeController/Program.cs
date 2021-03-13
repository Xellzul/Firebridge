using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireBridgeCore.Controller;

namespace FireBridgeController
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ApplicationExit += Application_ApplicationExit;
            PluginManager.Instance.LoadFolder(Directory.GetCurrentDirectory() + "/plugins");
            PluginManager.Instance.StartPlugins();
            ConnectionManger.Instance.Start();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            PluginManager.Instance.Stop();
            ConnectionManger.Instance.Stop();
        }
    }
}
