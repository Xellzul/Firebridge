using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32.TaskScheduler;
using System.Reflection;
using System.Security.Principal;
using System.Linq;

namespace FireBridgeWatchDog
{
    class Program
    {
        static void Main()
        {
            bool IsElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Trace.Listeners.Add(new TextWriterTraceListener(assemblyLocation + "\\watchdog.txt"));
            Trace.AutoFlush = true;
            Trace.WriteLine(Environment.NewLine + "Firebridge Watchdog starting (" + DateTime.Now + ")");
            Trace.WriteLine("Eelevated: " + IsElevated);
            if(!IsElevated)
            {
                Trace.WriteLine("Need admin rights - ending");
                return;
            }

            if(Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                Trace.WriteLine("Watchlog already running - stopping");
                return;
            }

            Process process;

            try
            {
                using (TaskService ts = new TaskService())
                {
                    try
                    {
                        ts.RootFolder.DeleteTask("FireBridge");
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Cant delete task \"FireBridge\": " + e.Message);
                    }

                    TaskDefinition td = ts.NewTask();
                    td.Settings.Compatibility = TaskCompatibility.V2_3;
                    td.Principal.RunLevel = TaskRunLevel.Highest;
                    td.Principal.LogonType = TaskLogonType.ServiceAccount;
                    td.Principal.UserId = "NT AUTHORITY\\SYSTEM";

                    td.Triggers.Add(new LogonTrigger() { Enabled = true });
                    td.Actions.Add(new ExecAction("FireBridgeWatchDog.exe", workingDirectory: assemblyLocation));
                    ts.RootFolder.RegisterTaskDefinition(@"FireBridge", td);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error creating Task: " + e.Message);
            }


            if (!WindowsIdentity.GetCurrent().IsSystem)
            {
                Trace.WriteLine("WatchDog started as non SYSTEM account - installing? - ending");
                return;
            }

            Trace.WriteLine("WatchDog started");

            while (true)
            {
                Trace.WriteLine("Firebridge starting");

                ApplicationLoader.PROCESS_INFORMATION procInfo;
                //ApplicationLoader.StartProcessAndBypassUAC("FireBridgeZombie.exe", assemblyLocation, out procInfo);
                //ApplicationLoader.StartProcessAndBypassUAC("notepad.exe", @"C:\WINDOWS\system32", out procInfo);
                bool success = ApplicationLoader.StartProcessAndBypassUAC("FireBridgeZombie.exe", assemblyLocation, out procInfo);
                

                Trace.WriteLine("Firebridge Started : " + success);

                try
                {
                    process = Process.GetProcessById((int)procInfo.dwProcessId);
                    process.EnableRaisingEvents = true;
                    process.WaitForExit();
                    Trace.WriteLine("Firebridge Ended with exit code: '" + process.ExitCode);

                    // process = new Process();
                    //process.StartInfo.FileName = "FireBridgeZombie.exe";
                    //process.StartInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //process.StartInfo.CreateNoWindow = true;
                    //process.StartInfo.UseShellExecute = false;
                    //process.OutputDataReceived += Process_OutputDataReceived;
                    //process.ErrorDataReceived += Process_ErrorDataReceived;
                    
                    if (process.ExitCode == 69)
                    {
                        Trace.WriteLine("Updating");
                        var folder = new DirectoryInfo(assemblyLocation);
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
                catch (Exception e)
                { Trace.WriteLine("Failed to wait for proccess exit: " + e.Message); }
            }
        }
    }
}
