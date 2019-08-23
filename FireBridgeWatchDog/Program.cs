using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.TaskScheduler;
using System.Reflection;

namespace FireBridgeWatchDog
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            bool ConsoleEnabled = false;
            Console.ForegroundColor = ConsoleColor.Gray;
            const int SW_HIDE = 0;
            const int SW_SHOW = 5;

            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            Process process;
            try { 
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                registryKey.DeleteValue("FireBridge");
            }
            catch (Exception e)
            {

            }

            using (TaskService ts = new TaskService())
            {
                try { 
                ts.RootFolder.DeleteTask("FireBridge");
                }
                catch (Exception e) { }
                TaskDefinition td = ts.NewTask();
                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.Principal.LogonType = TaskLogonType.InteractiveToken;
                td.Triggers.Add(new LogonTrigger() { });
                td.Actions.Add(new ExecAction("FireBridgeWatchDog.exe", 
                    workingDirectory: Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)));
                ts.RootFolder.RegisterTaskDefinition(@"FireBridge", td);
            }

            Console.WriteLine("WatchDog started");

            while (true)
            {
                process = new Process();
                process.StartInfo.FileName = "FireBridgeZombie.exe";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;
                Console.WriteLine("Firebridge Started");
                process.Start();
                while (!process.WaitForExit(20)) { }
                    Console.WriteLine("Firebridge Stopping with exit code: " + process.ExitCode);

                if (process.ExitCode == 69)
                {
                    Console.WriteLine("Updating");
                    var folder = new DirectoryInfo(".");
                    var files = folder.GetFiles("*.new");
                    foreach (var file in files)
                    {
                        File.Delete(file.Name.Remove(file.Name.Length - 4));
                        File.Move(file.Name, file.Name.Remove(file.Name.Length - 4));
                    }
                    Console.WriteLine("Updated");
                }
                else if (process.ExitCode == 70)
                {
                    Console.WriteLine("Restarting on request");
                }
                else if (process.ExitCode == 71)
                {
                    Console.WriteLine("Toggling console");
                    if (ConsoleEnabled)
                        ShowWindow(handle, SW_HIDE);
                    else
                        ShowWindow(handle, SW_SHOW);
                    ConsoleEnabled =! ConsoleEnabled;
                }
                else
                {
                    Console.WriteLine("Possible crash... restarting");
                }
            }
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Data);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(e.Data);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
