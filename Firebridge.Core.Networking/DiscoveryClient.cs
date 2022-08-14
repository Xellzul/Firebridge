using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Firebridge.Core.Networking;

/// <summary>
/// For finging everyone with <see cref="DiscoveryServer"/>
/// </summary>
public class DiscoveryClient
{
    public string Secret { get; private set; }
    public int Port { get; private set; }
    public ReadOnlyMemory<byte> RequestData { get; private set; }

    /// <summary>
    /// In ms
    /// </summary>
    public int PingerDelay { get; set; } = 5000;

    private UdpClient PingerResponseClient;
    private Task PingerTask;
    private CancellationTokenSource PingerCancellationTokenSource;

    /// <param name="secret">App wide secret</param>
    /// <param name="port">udp port where to look for app</param>
    public DiscoveryClient(string secret, int port)
    {
        Secret = secret;
        Port = port;
        PingerCancellationTokenSource = new CancellationTokenSource();

        PingerResponseClient = new UdpClient(AddressFamily.InterNetworkV6);
        PingerResponseClient.EnableBroadcast = true;
        PingerResponseClient.Client.DualMode = true;

        PingerTask = new Task(async () => { await SendPingsAsync(PingerCancellationTokenSource.Token); }, PingerCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
    }

    private async Task SendPingsAsync(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var addresses = GetBroadcastAddresses();
                addresses.Select(x => SendPingAsync(x, cancellationToken)).ToList();
                await Task.Delay(PingerDelay);
            }
            catch(OperationCanceledException)
            {
                break;
            }
            catch(SocketException)
            {
            }
        }
    }

    private async Task SendPingAsync(IPAddress iPAddress, CancellationToken cancellationToken)
    {
        var PingerClient = new UdpClient();
        PingerClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        PingerClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);

        await PingerClient.SendAsync(RequestData, new IPEndPoint(iPAddress, Port), cancellationToken);
    }

    public async Task<(IPAddress Address, Guid Id)> DiscoverClient(CancellationToken cancellationToken)
    {
        while(true)
        {
            if (!PingerResponseClient.Client.IsBound)
            {
                PingerResponseClient.Client.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
                var recievedPort = ((IPEndPoint?)PingerResponseClient.Client.LocalEndPoint)?.Port;
                if (recievedPort == null)
                    throw new InvalidOperationException("No bound port");

                RequestData = new ReadOnlyMemory<byte>(Encoding.ASCII.GetBytes(Secret + recievedPort));
            }

            // Turn on if not running
            if (!PingerCancellationTokenSource.IsCancellationRequested && PingerTask.Status == TaskStatus.Created)
            {
                PingerTask.Start();
            }

            UdpReceiveResult response;

            try
            {
                response = await PingerResponseClient.ReceiveAsync(cancellationToken);
            }
            catch(SocketException)
            {
                continue;
            }

            //Get id and return
            try
            {
                var ASCII = Encoding.ASCII.GetString(response.Buffer);

                if (!ASCII.StartsWith(Secret))
                    continue;

                Guid id;
                if (Guid.TryParse(ASCII.Substring(Secret.Length), out id))
                {
                    return (response.RemoteEndPoint.Address, id);
                }
            }
            catch { };
        }
    }

    public void Stop()
    {
        PingerCancellationTokenSource.Cancel();
    }

    private IEnumerable<IPAddress> GetBroadcastAddresses()
    {
        //https://stackoverflow.com/questions/1096142/broadcasting-udp-message-to-all-the-available-network-cards
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up
                         && ni.SupportsMulticast
                         && ((ni.GetIPProperties().GetIPv4Properties() != null
                             && ni.GetIPProperties().GetIPv4Properties().Index != NetworkInterface.LoopbackInterfaceIndex)
                         || ni.GetIPProperties().GetIPv6Properties() != null
                             && ni.GetIPProperties().GetIPv6Properties().Index != NetworkInterface.IPv6LoopbackInterfaceIndex))
            .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
            .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork || a.Address.AddressFamily == AddressFamily.InterNetworkV6)
            .Select(a => a.Address);
    }
}
