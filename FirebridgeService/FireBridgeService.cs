using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FirebridgeService
{
    public partial class FireBridgeService : ServiceBase
    {
        EventLog eventLog;
        Process process;
        public FireBridgeService()
        {
            InitializeComponent();

            eventLog = new EventLog();
            if (!EventLog.SourceExists("FireBridge"))
                EventLog.CreateEventSource("FireBridge", "FireBridgeLog");
            eventLog.Source = "FireBridge";
            EventLog.Log = "FireBridgeLog";
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Firebridge Starting");
            StartApp();
        }

        private void StartApp()
        {
            if(process != null)
                if(!process.HasExited)
                    process?.Kill();
            process = new Process();
            //process.StartInfo.Verb = "runas";
            process.StartInfo.FileName = "FireBridgeZombie.exe";
            process.StartInfo.UserName = "admin";
            process.StartInfo.Domain = "";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.Exited += Process_Exited;
        }

        private void Process_Exited(object sender, EventArgs e)
        {

            if (process.ExitCode == 69)
            {
                eventLog.WriteEntry("Update Requsted");
                File.Delete("FireBridgeZombie.exe");
                File.Move("FireBridgeZombie.exe.new", "FireBridgeZombie.exe");
            }
            else if (process.ExitCode == 70)
            {
                eventLog.WriteEntry("Restart Requested");
            }
            else
            {
                eventLog.WriteEntry("Possible crash");
            }

            StartApp();
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("Firebridge Stopping");
            if (!process.HasExited)
                process?.Kill();
        }
    }
}
