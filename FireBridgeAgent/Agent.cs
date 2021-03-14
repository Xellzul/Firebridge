﻿using FireBridgeCore;
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
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace FireBridgeAgent
{
    internal class Agent
    {
        private bool _completed = false;
        private UserProgramContainer _userProgramContainer;

        private StreamWriter sw;
        private ConsoleConnection _connection;

        public Agent()
        {
            // Thread.Sleep(10000);
            var filename = $"C://FireBridge/Agent-{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}-{Process.GetCurrentProcess().Id}.txt";
            Directory.CreateDirectory("C://FireBridge/");
            sw = new StreamWriter(filename, true);

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
                        _connection.MessageRecieved -= _connection_MessageRecieved;

                        Assembly asm = null;
                        //todo: try catch
                        if (spm.Assemblies != null && spm.Assemblies.Length > 0)
                            asm = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(spm.Assemblies));
                        else
                            asm =  AppDomain.CurrentDomain.GetAssemblies().Where(x => x.ManifestModule.Name == "FireBridgeCore.dll").First();

                        var type = asm.GetType(spm.Type);
                        UserProgram instance = Activator.CreateInstance(type) as UserProgram;

                        object args = null;
                        if (spm.StartParameters != null && spm.StartParameters.Length > 0)
                        { 
                            var ms = new MemoryStream(spm.StartParameters, false);
                            args = new BinaryFormatter().Deserialize(ms);
                        }

                        _userProgramContainer.StartAsync(instance, args, _connection, spm.ProcessId, e.Message.From);

                        break;
                    default:
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