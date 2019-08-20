using FirebridgeShared.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirebridgeClient
{
    public partial class Form1 : Form
    {
        Client c = new Client();
        Random r = new Random();
        int num;
        public Form1()
        {
            num = r.Next();
            InitializeComponent();
            c.MessageRecieved += C_MessageRecieved;
            c.Connect();
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
