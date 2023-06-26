using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Firebridge.Controller.Common.Agents;

public class Class1TODODELETE : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
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
        label.Location = new Point(0, 0);

        form.TopMost = true;
        form.FormBorderStyle = FormBorderStyle.None;
        form.WindowState = FormWindowState.Maximized;

        new Task(async () =>
        {
            await Task.Delay(3000);
            form.Invoke(form.Close);
        }).Start();

        label.Size = form.Size;
        label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        label.Dock = DockStyle.Fill;
        label.AutoSize = true;
        label.Font = new Font(label.Font.FontFamily, 64f);

        form.ShowDialog();

        return Task.CompletedTask;
    }
}