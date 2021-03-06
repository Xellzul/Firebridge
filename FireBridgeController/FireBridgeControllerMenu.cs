using FireBridgeCore.Controller;
using FireBridgeCore.Kernel;
using FireBridgeCore.Kernel.UserPrograms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireBridgeController
{
    public partial class FireBridgeControllerMenu : Form
    {
        private ContextMenuStrip cm = new ContextMenuStrip();
        public OverridePorgramSettings OverridePorgramSettings { get; set; } = new OverridePorgramSettings();
        private ScreenshotSettings screenshotSettings;
        public ConnectionManger ConnectionManger { get; set; } = new ConnectionManger();
        public List<FireBridgePlugin> Plugins = new List<FireBridgePlugin>();
        public FireBridgeControllerMenu()
        {
            /*
            foreach(var file in Directory.GetFiles(Directory.GetCurrentDirectory() + "/plugins"))
            {
                if (!file.EndsWith(".dll"))
                    continue;

                try
                {
                    Assembly asm = Assembly.LoadFrom(file);

                    foreach (Type type in asm.GetTypes())
                    {
                        if (type.GetCustomAttributes(typeof(FireBridgePluginAttribute), true).Length > 0)
                        {
                            Plugins.Add((FireBridgePlugin)Activator.CreateInstance(type));
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("hello");
                }
            }
            */
            screenshotSettings = new ScreenshotSettings(OverridePorgramSettings);
            screenshotSettings.SettingsChanged += ScreenshotSettings_SettingsChanged;
            var selectAll = new ToolStripMenuItem("Select All");
            selectAll.Click += SelectAll_Click;
            var invert = new ToolStripMenuItem("Invert Selection");
            invert.Click += Invert_Click;
            var deselect = new ToolStripMenuItem("Deselect All");
            deselect.Click += Deselect_Click;

            cm.Items.AddRange(new []{ selectAll, invert, deselect});

            InitializeComponent();

            this.ContextMenuStrip = cm;
            this.ContextMenuStrip.PerformLayout();

            screenshotSettingsMenuItem.Click += ScreenshotSettingsMenuItem_Click;

            ConnectionManger.ClientConnected += ConnectionManger_ClientConnected;
            ConnectionManger.Start();

            foreach (var plugin in Plugins)
            {
                var plugin2 = plugin.Register();
                foreach(var b in plugin2)
                {
                    var menu = new ToolStripMenuItem() { Text = b.Key, Tag = b.Value };
                    menu.Click += Menu_Click;
                    this.settingsMenu.DropDownItems.Add(menu);
                }
            }
        }

        private void Menu_Click(object sender, EventArgs e)
        {
            if (sender == null || !(sender is ToolStripMenuItem))
                return;

            ((Action<ServiceConnection[]>)(((ToolStripMenuItem)sender).Tag)).Invoke(ConnectionManger.GetConnectedServices().ToArray());
        }

        private void ScreenshotSettings_SettingsChanged(object sender, EventArgs e)
        {
            foreach (var item in mainView.Controls)
                if (item is OverViewControl ocv)
                    if (ocv.Selected)
                        ocv?.opc.ChangeSettings(OverridePorgramSettings);
        }

        private void ScreenshotSettingsMenuItem_Click(object sender, EventArgs e)
        {
            if(screenshotSettings.Visible)
                screenshotSettings.BringToFront();
            else
                screenshotSettings.Show();
        }

        private void Deselect_Click(object sender, EventArgs e)
        {
            foreach (var item in mainView.Controls)
                if (item is OverViewControl ocv)
                    ocv.Selected = false;
        }

        private void Invert_Click(object sender, EventArgs e)
        {
            foreach (var item in mainView.Controls)
                if (item is OverViewControl ocv)
                    ocv.Selected = !ocv.Selected;
        }

        private void SelectAll_Click(object sender, EventArgs e)
        {
            foreach (var item in mainView.Controls)
                if (item is OverViewControl ocv)
                    ocv.Selected = true;
        }

        private void ConnectionManger_ClientConnected(object sender, ServiceConnectionConnectedEventArgs e)
        {
            if (this.InvokeRequired) {
                this.Invoke(new MethodInvoker(() => { ConnectionManger_ClientConnected(sender, e); }));
                return;
            }


            var toadd = new OverViewControl();
            toadd.DragOver += mainView_DragOver;
            toadd.MouseDown += Toadd_MouseDown;
            mainView.Controls.Add(toadd);
            toadd.Init(e.ServiceConnection);


            //for (int i = 0; i < 5; i++)
            //{
            //    toadd = new OverViewControl();
            //    toadd.MouseDown += Toadd_MouseDown;
            //    toadd.DragOver += mainView_DragOver;
            //    mainView.Controls.Add(toadd);
            //    toadd.Init(e.ServiceConnection);
            //    toadd.BackColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)); ;
            //}
        }


        private void Toadd_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
            DoDragDrop(sender, DragDropEffects.Move);
        }

        private void mainView_DragOver(object sender, DragEventArgs e)
        {
            OverViewControl realSender = e.Data.GetData(typeof(OverViewControl)) as OverViewControl;
            if (mainView.Controls.Count < 1 || realSender == null)
                return;

            Point screenCoords = Cursor.Position;
            Point controlRelatedCoords = this.mainView.PointToClient(screenCoords);

            Console.WriteLine(controlRelatedCoords);

            Control nearest = mainView.Controls[0];
            double distance = double.MaxValue;

            foreach (Control control in mainView.Controls)
            {
                Point middle = control.Location + (control.Size / 2);
                double newDist = GetDistance(middle, controlRelatedCoords);

                if(newDist < distance)
                {
                    distance = newDist;
                    nearest = control;
                }
            }

            if (realSender == nearest)
                return;

            this.mainView.Controls.SetChildIndex(realSender, this.mainView.Controls.GetChildIndex(nearest));
        }

        private static double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2));
        }

        private void UnlockMenuItem_Click(object sender, EventArgs e)
        {
            PasswordForm frm = new PasswordForm();
            if (frm.ShowDialog() != DialogResult.OK)
                this.Close();

            throw new NotImplementedException();
            /*
            foreach (var item in ConnectionManger.GetConnectedServices())
                item.StartProgram(item.GetAgent(IIntegrityLevel.System, 0), new UnlockPcProcess() { 
                Password = frm.tb_password.Text, Username = frm.tb_username.Text });*/
        }
    }
}
