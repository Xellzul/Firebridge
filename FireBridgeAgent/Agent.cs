using FireBridgeCore;
using FireBridgeCore.Kernel;
using FireBridgeCore.Networking;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Loader;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace FireBridgeAgent
{
    internal class Agent
    {
        private bool _completed = false;
        private UserProgramContainer _userProgramContainer;

        private StreamWriter sw = new StreamWriter($"C://FireBridge/Agent-{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}-{Process.GetCurrentProcess().Id}.txt", true);
        private ConsoleConnection _connection;

        public Agent()
        {
           // Thread.Sleep(10000);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            _connection = new ConsoleConnection();
            _connection.MessageRecieved += _connection_MessageRecieved;

            _userProgramContainer = new UserProgramContainer();
            _userProgramContainer.Completed += _userProgramContainer_Completed;
        }

        private void _userProgramContainer_Completed(object sender, EventArgs e)
        {
            _completed = true;
        }

        private void _connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            lock (_userProgramContainer)
            {
                Console.WriteLine("Recieved: " + e.Message.Payload.GetType());
                switch (e.Message.Payload)
                {
                    case StartProgramModel spm:
                        Console.WriteLine("A0");
                        _userProgramContainer.Completed -= _userProgramContainer_Completed;
                        Console.WriteLine("A1");

                        //todo: try catch
                        if (spm.Assemblies != null && spm.Assemblies.Length > 0)
                        {
                            Console.WriteLine("A");
                            var assembly = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(spm.Assemblies));
                            Console.WriteLine("b");
                            var type = assembly.GetType(spm.Type);
                            Console.WriteLine("C");
                            UserProgram instance = Activator.CreateInstance(type) as UserProgram;
                            Console.WriteLine("D");

                            _userProgramContainer.StartAsync(instance, _connection, spm.ProcessId, e.Message.From);
                            Console.WriteLine("E");
                        }
                        else
                        {
                            Console.WriteLine("F");
                            var type = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.ManifestModule.Name == "FireBridgeCore.dll").First()
                                .GetType(spm.Type);
                            Console.WriteLine("G");
                            UserProgram instance = Activator.CreateInstance(type) as UserProgram;
                            Console.WriteLine("H");

                            _userProgramContainer.StartAsync(instance, _connection, spm.ProcessId, e.Message.From);
                            Console.WriteLine("I");
                        }

                        break;
                    default:
                        Console.WriteLine("def");
                        break;
                }
            }
        }

        internal void Start()
        {
            _connection.Start(Console.OpenStandardInput(), Console.OpenStandardOutput());
            sw.AutoFlush = true;

            Console.SetIn(new StreamReader(new MemoryStream()));
            Console.SetOut(sw);

            Console.WriteLine("Started!");
            
            while (!_completed)
                Thread.Sleep(100);
        }
    }
}