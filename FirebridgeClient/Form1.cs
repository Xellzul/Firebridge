﻿using FirebridgeShared.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirebridgeClient
{
    public partial class Form1 : Form
    {
        Connection c;
        Random r = new Random();
        int num;
        public Form1()
        {
            num = r.Next();
            InitializeComponent();
            c = new Connection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969));
            c.MessageRecieved += C_MessageRecieved;
            this.timer1.Enabled = true;
        }

        private void C_MessageRecieved(object sender, EventArgs e)
        {
            Console.WriteLine(((MessageEventArgs)e).Packet.Data.ToString());
            if (this.InvokeRequired)
                this.Invoke(new Action(() => _textBoxConsole.Text += ((MessageEventArgs)e).Packet.Data.ToString() + Environment.NewLine));
            else
                _textBoxConsole.Text += ((MessageEventArgs)e).Packet.Data.ToString() + Environment.NewLine;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
           
        }

        private void _buttonConsoleSubmit_Click(object sender, EventArgs e)
        {
            c.SendPacket(new Packet() { Id = 0, Data = _textBoxSubmitText.Text+ num + r.Next(500).ToString() });
        }

        private void _textBoxConsole_TextChanged(object sender, EventArgs e)
        {
            _textBoxConsole.SelectionStart = _textBoxConsole.Text.Length;
            _textBoxConsole.ScrollToCaret();
        }
    }
}
