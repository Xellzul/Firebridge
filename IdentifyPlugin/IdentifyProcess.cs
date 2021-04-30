using FireBridgeCore.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IdentifyPlugin
{
    public class IdentifyProcess : UserProgram
    {
        public override void Main(UserProgramContainer container, object args)
        {
            var label = new Label();
            var form = new Form();
            form.Controls.Add(label);



            label.Text =
                Environment.MachineName +
                Environment.NewLine +
                Environment.OSVersion +
                Environment.NewLine +
                Environment.UserName +
                Environment.NewLine;


            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (ni.GetIPProperties().GatewayAddresses?.Count > 0)
                                label.Text += ">>";
                            label.Text += ip.Address.ToString() + Environment.NewLine;
                        }
                    }
                }
            }
            label.Location = new System.Drawing.Point(0,0);

            form.TopMost = true;
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Maximized;

            new Task(async () =>
                {
                    await Task.Delay(3000);
                    form.Close();
                }).Start();

            label.Size = form.Size;
            label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            label.Dock = DockStyle.Fill;
            label.AutoSize = true;
            label.Font = new System.Drawing.Font(label.Font.FontFamily, 64f);

            form.ShowDialog();
        }
    }
}
