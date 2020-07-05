using FirebridgeShared.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FirebridgeClient.Controls;
using Newtonsoft.Json;
using Microsoft.Win32.SafeHandles;

namespace FirebridgeClient
{
    public partial class MainView : Form
    {
        private DiscoveryClient discoveryClient;
        private ConcurrentBag<ZombieView> devicesConnected = new ConcurrentBag<ZombieView>();
        private List<ZombieView> selectedDevices = new List<ZombieView>();
        private List<string> sortedDevices = new List<string>();

        public MainView()
        {
            InitializeComponent();

            try
            {
                sortedDevices = JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText(@"./sortedMachines"));
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            discoveryClient = new DiscoveryClient();
            discoveryClient.ClientResponded += DiscoveryClient_ClientResponded;
            zombieActionsBar1.zombieViews = selectedDevices;

            discoveryClient.Run();
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
                zombieView.MachineNameChanged += ZombieView_MachineNameChanged;
                zombieView.SelectionChange += ZombieView_SelectionChange;

                //request machine name, zombie rev
                zombieView.SendPacket(new Packet() { Id = 5 });
                zombieView.SendPacket(new Packet() { Id = 6 });

                devicesConnected.Add(zombieView);
                flowLayoutPanel1.Controls.Add(zombieView);
            }
        }

        private void ZombieView_MachineNameChanged(object sender, EventArgs e)
        {
            if (!sortedDevices.Contains(((OnMachineNameChangedEventArgs)e).MachineName))
            {
                sortedDevices.Add(((OnMachineNameChangedEventArgs)e).MachineName);
            }

            List<ZombieView> views = flowLayoutPanel1.Controls.OfType<ZombieView>().ToList();
            views = views.OrderBy(d => sortedDevices.FindIndex(x => x == d.MachineName)).ToList();
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.AddRange(views.ToArray());


            try
            {
                string json = JsonConvert.SerializeObject(sortedDevices.ToArray());
                System.IO.File.WriteAllText(@"./sortedMachines", json);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
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

        private void _bRight_Click(object sender, EventArgs e)
        {

            if (selectedDevices.Count != 1)
                return;

            ZombieView zm = selectedDevices.First();
            var pos = sortedDevices.IndexOf(zm.MachineName);

            if (pos == -1)
                return;

            var controlIndex = flowLayoutPanel1.Controls.IndexOf(zm);

            controlIndex++;
            if (controlIndex > flowLayoutPanel1.Controls.Count - 1)
                controlIndex = flowLayoutPanel1.Controls.Count - 1;


            sortedDevices.RemoveAt(pos);
            sortedDevices.Insert(controlIndex, zm.MachineName);

            ZombieView_MachineNameChanged(null, new OnMachineNameChangedEventArgs(zm.MachineName));
        }

        private void _bLeft_Click(object sender, EventArgs e)
        {

            if (selectedDevices.Count != 1)
                return;

            ZombieView zm = selectedDevices.First();
            var pos = sortedDevices.IndexOf(zm.MachineName);
            if (pos == -1)
                return;

            var controlIndex = flowLayoutPanel1.Controls.IndexOf(zm);

            controlIndex--;
            if (controlIndex < 0)
                controlIndex = 0;


            sortedDevices.RemoveAt(pos);
            sortedDevices.Insert(controlIndex, zm.MachineName);

            ZombieView_MachineNameChanged(null, new OnMachineNameChangedEventArgs(zm.MachineName));
        }
    }
}
