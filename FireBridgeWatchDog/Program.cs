using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.TaskScheduler;
using System.Reflection;
using System.Security.Principal;
using System.Threading;

namespace FireBridgeWatchDog
{
    class Program
    {
        static void Main()
        {
            bool IsElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            Trace.Listeners.Add(new TextWriterTraceListener(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\watchdog.txt"));
            Trace.AutoFlush = true;
            Trace.Indent();
            Trace.WriteLine("Firebridge Watchdog started");
            Trace.WriteLine("Eelevated: " + IsElevated);


            Process process;


            try
            {
                using (TaskService ts = new TaskService())
                {
                    try
                    {
                        ts.RootFolder.DeleteTask("FireBridge");
                    }
                    catch (Exception e) {
                        Trace.WriteLine(e.Message);
                    }
                    TaskDefinition td = ts.NewTask();
                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.Principal.LogonType = TaskLogonType.InteractiveToken;
                td.Principal.GroupId = "Users";

                td.Triggers.Add(new LogonTrigger() { Enabled = true });
                td.Actions.Add(new ExecAction("FireBridgeWatchDog.exe", 
                    workingDirectory: Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)));
                //ts.RootFolder.RegisterTaskDefinition(@"FireBridge", td);
            }


                }
                catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
            Trace.WriteLine("WatchDog started");

            while (true)
            {

                ApplicationLoader.PROCESS_INFORMATION procInfo;
                //ApplicationLoader.StartProcessAndBypassUAC("FireBridgeZombie.exe", Assembly.GetExecutingAssembly().Location, out procInfo);
                //ApplicationLoader.StartProcessAndBypassUAC("notepad.exe", @"C:\WINDOWS\system32", out procInfo);
                ApplicationLoader.StartProcessAndBypassUAC("FireBridgeZombie.exe", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), out procInfo);

                Trace.WriteLine("Firebridge Started");

                try
                {
                    process = Process.GetProcessById((int)procInfo.dwProcessId);
                    process.WaitForExit();
                }
                catch(Exception e)
                { Trace.WriteLine(e.Message); }

                Thread.Sleep(250);

                Trace.WriteLine("Firebridge Ended, - restarting");
                continue;

                //process = Process.GetProcessById((int)procInfo.dwProcessId);
                // process = new Process();
                //process.StartInfo.FileName = "FireBridgeZombie.exe";
                //process.StartInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //process.StartInfo.CreateNoWindow = true;
                //process.StartInfo.UseShellExecute = false;
                //process.OutputDataReceived += Process_OutputDataReceived;
                //process.ErrorDataReceived += Process_ErrorDataReceived;


                //process.Start();
                while (!process.WaitForExit(20)) { }
                Trace.WriteLine("Firebridge Stopping with exit code: " + process.ExitCode);

                if (process.ExitCode == 69)
                {
                    Trace.WriteLine("Updating");
                    var folder = new DirectoryInfo(".");
                    var files = folder.GetFiles("*.new");
                    foreach (var file in files)
                    {
                        File.Delete(file.Name.Remove(file.Name.Length - 4));
                        File.Move(file.Name, file.Name.Remove(file.Name.Length - 4));
                    }
                    Trace.WriteLine("Updated");
                }
                else if (process.ExitCode == 70)
                {
                    Trace.WriteLine("Restarting on request");
                }
                else
                {
                    Trace.WriteLine("Possible crash... restarting");
                }
            }
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.WriteLine(e.Data);
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Trace.WriteLine(e.Data);
        }
    }
}
