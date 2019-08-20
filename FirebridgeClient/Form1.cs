using FirebridgeShared.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirebridgeClient
{
    public partial class Form1 : Form
    {
        Connection c;
        Random r = new Random();
        int num;
        public Form1()
        {
            num = r.Next();
            InitializeComponent();
            c = new Connection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969));
            c.MessageRecieved += C_MessageRecieved;
            this.timer1.Enabled = true;
        }

        private void C_MessageRecieved(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.Text = this.Text = ((MessageEventArgs)e).Packet.Data.ToString()));
            else
                this.Text = this.Text = ((MessageEventArgs)e).Packet.Data.ToString();

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            c.SendPacket(new Packet() { Id = 0, Data = "Ahoj" + num + r.Next(500).ToString()});
        }
    }
}
