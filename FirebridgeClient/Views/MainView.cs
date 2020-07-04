using FirebridgeShared.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FirebridgeClient.Controls;

namespace FirebridgeClient
{
    public partial class MainView : Form
    {
        private DiscoveryClient discoveryClient;
        private ConcurrentBag<ZombieView> devicesConnected = new ConcurrentBag<ZombieView>();
        private List<ZombieView> selectedDevices = new List<ZombieView>();

        public MainView()
        {
            InitializeComponent();
            discoveryClient = new DiscoveryClient();
            discoveryClient.ClientResponded += DiscoveryClient_ClientResponded;
            discoveryClient.Run();
            zombieActionsBar1.zombieViews = selectedDevices;
        }

        private void DiscoveryClient_ClientResponded(object sender, EventArgs e)
        {
            var data = e as ClientRespondedEventArgs;
            if (data == null || sender == null)
                return;

            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.DiscoveryClient_ClientResponded(sender, e)));

            if (devicesConnected.Count(x => x.Ip == data.Ip) == 0)
            {
                var zombieView = new ZombieView(data.Ip);
                zombieView.SelectionChange += ZombieView_SelectionChange;
                devicesConnected.Add(zombieView);
                flowLayoutPanel1.Controls.Add(zombieView);
            }
        }

        private void ZombieView_SelectionChange(object sender, EventArgs e)
        {
            ZombieView zm = sender as ZombieView;
            SelectionChangeEventArgs sch = e as SelectionChangeEventArgs;
            if (sch == null || e == null)
                return;

            if (sch.Selected)
                selectedDevices.Add(zm);
            else
                selectedDevices.Remove(zm);
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool changeTo = true;
            if (selectedDevices.Count == devicesConnected.Count)
                changeTo = false;
            foreach (ZombieView view in devicesConnected)
                view.IsSelected = changeTo;
        }

        private void pingTimer_Tick(object sender, EventArgs e)
        {
            discoveryClient.SendPing();
        }
    }
}
