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
        Connection connection;
        private bool downloadedData = false;

        public string Ip { get => _ip.Text; }

        public ZombieView(string ip)
        {
            InitializeComponent();

            connection = new Connection(new IPEndPoint(IPAddress.Parse(ip), 6969));
            _ip.Text = ip;

            UISetup();
            Init();
        }

        public void SendPacket(Packet packet)
        {
            connection.SendPacket(packet);
        }

        private void Init()
        {
            connection.MessageRecieved += Connection_MessageRecieved;
            detailView = new ZombieDetailView(this.connection);
        }

        public void AutoUpdate()
        {
            connection.SendPacket(new Packet() { Id = 3, Data = "" });
        }

        private void UISetup()
        {
            _image.BackColor = ColorTranslator.FromHtml("#34ace0");
            this.BackColor = ColorTranslator.FromHtml("#84817a");

            _image.GetType()
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(_image, true);
        }


        private void Connection_MessageRecieved(object sender, EventArgs e)
        {
            var packet = ((MessageEventArgs)e).Packet;
            if (packet.Id == 3)
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action(() =>
                    {
                        _image.Image?.Dispose();
                        _image.Image = (Bitmap)packet.Data;
                    }));
                else
                {
                    _image.Image?.Dispose();
                    _image.Image = (Bitmap)packet.Data;
                }

            }

            Console.WriteLine(((MessageEventArgs)e).Packet.Data.ToString());


            //if (this.InvokeRequired)
            //    this.Invoke(new Action(() => _textBoxConsole.Text += ((MessageEventArgs)e).Packet.Data.ToString() + Environment.NewLine));
            //else
            //    _textBoxConsole.Text += ((MessageEventArgs)e).Packet.Data.ToString() + Environment.NewLine;
        }

        private bool selected = false;

        public bool IsSelected
        {
            get => selected;
            set
            {
                selected = value;

                if (selected)
                {
                    this.BackColor = ColorTranslator.FromHtml("#ffb142");
                }
                else
                {
                    this.BackColor = ColorTranslator.FromHtml("#84817a");
                }
            }
        }


        private void _image_Click(object sender, EventArgs e)
        {
            IsSelected = !selected;

        }

        public static int Width = 199;
        public static int Height = 248;

        private void Todo_delete_this_Click(object sender, EventArgs e)
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

        private ZombieDetailView detailView;
        private void _buttonDetail_Click(object sender, EventArgs e)
        {
            detailView.Show();
        }
    }
}
