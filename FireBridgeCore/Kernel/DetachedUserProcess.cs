﻿using FireBridgeCore.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBridgeCore.Kernel
{
    public class DetachedUserProcess : UserProcess
    {
        public uint SessionID { get; private set; }
        public IIntegrityLevel IntegrityLevel { get; private set; }
        public Process Process { get; private set; }
        public ConsoleConnection ProgramConnetion { get; private set; }

        public StartProgramModel StartProgramModel { get; private set; }
        public DetachedUserProcess(StartProgramModel startProgramModel, Connection connection) : base(startProgramModel.ProcessId, connection)
        {
            SessionID = startProgramModel.SessionId;
            IntegrityLevel = startProgramModel.IntegrityLevel;
            StartProgramModel = startProgramModel;
        }

        public override bool Start()
        {
            Connection.ConnectionStatusChanged += Connection_ConnectionStatusChanged;

            var result = ApplicationLoader.StartProcess(
                AppDomain.CurrentDomain.BaseDirectory + "FireBridgeAgent.exe",
                new[] { Id.ToString() },
                SessionID,
                IntegrityLevel
            );

            if(result.process != null && !result.process.HasExited)
                result.process.Exited += Process_Exited;
            else
            {
                Stop();
                return false;
            }

            if (result.process.HasExited)
            {
                Stop();
                return false;
            }

            Process = result.process;
            ProgramConnetion = new ConsoleConnection();
            ProgramConnetion.MessageRecieved += ProgramConnetion_MessageRecieved;
            ProgramConnetion.ConnectionStatusChanged += ProgramConnetion_ConnectionStatusChanged;


            Connection.MessageRecieved += Connection_MessageRecieved;

            ProgramConnetion.Start(result.read, result.write);

            ProgramConnetion.Send(new Packet(Guid.Empty, Guid.Empty, StartProgramModel));

            return true;
        }

        private void ProgramConnetion_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (e.Now == ConnectionStatus.Disconnected)
                Stop();
        }

        private void ProgramConnetion_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            Connection.Send(e.Message);
        }

        private void Connection_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            ProgramConnetion.Send(e.Message);
        }

        private void Connection_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            if (e.Now == ConnectionStatus.Disconnected)
                Stop();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Stop();
        }

        public override void Stop()
        {
            Process?.Kill();
            base.Stop();
        }
    }
}