using Firebridge.Controller.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Firebridge.Common;
using Firebridge.Common.Models.Packets;

namespace Firebridge.Controller.Common;

public class DiscoveryClient : IDiscoveryClient
{
    private readonly ILogger<DiscoveryClient> logger;
    private readonly IOptions<FirebridgeControllerSettings> configuration;
    private readonly IControllerContext context;

    private Task PingerTask;
    private Task ReaderTask;
    private CancellationTokenSource PingerCancellationTokenSource;
    private UdpClient PingerResponseClient;
    private ReadOnlyMemory<byte> RequestData { get; set; }

    public DiscoveryClient(ILogger<DiscoveryClient> logger, IOptions<FirebridgeControllerSettings> configuration, IControllerContext context)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.context = context;

        PingerCancellationTokenSource = new CancellationTokenSource();
        PingerTask = new Task(async () =>
        {
            await SendPingsAsync(PingerCancellationTokenSource.Token);
        }, PingerCancellationTokenSource.Token, TaskCreationOptions.LongRunning);


        ReaderTask = new Task(async () =>
        {
            await DiscoverClients(PingerCancellationTokenSource.Token);
        }, PingerCancellationTokenSource.Token, TaskCreationOptions.LongRunning);

        PingerResponseClient = new UdpClient(AddressFamily.InterNetworkV6);
        PingerResponseClient.EnableBroadcast = true;
        PingerResponseClient.Client.DualMode = true;
    }

    public void StartDiscovering()
    {
        logger.LogInformation("StartDiscovering");
        ReaderTask.Start();
        PingerTask.Start();
    }

    public void StopDiscovering()
    {
        logger.LogInformation("StopDiscovering");
        PingerCancellationTokenSource.Cancel();
    }

    private async Task DiscoverClients(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!PingerResponseClient.Client.IsBound)
            {
                PingerResponseClient.Client.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
                var recievedPort = ((IPEndPoint?)PingerResponseClient.Client.LocalEndPoint)?.Port;
                if (recievedPort == null)
                    throw new InvalidOperationException("No bound port");

                RequestData = new ReadOnlyMemory<byte>(Encoding.ASCII.GetBytes(configuration.Value.DiscoveryServerSecret + recievedPort));
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
            catch (SocketException e)
            {
                logger.LogDebug(e, "Error in discovery data recieving");
                continue;
            }

            //Get id and return
            try
            {

                var packet = StreamSerializer.Deserialize<DiscoveryPacket>(response.Buffer);

                if (configuration.Value.DiscoveryServerSecret != packet.Secret)
                {
                    logger.LogDebug($"Discovery secret doesnt match got:{packet.Secret}, should have {configuration.Value.DiscoveryServerSecret}, from {response.RemoteEndPoint}");
                    continue;
                }

                _ = context.TryConnectService(response.RemoteEndPoint.Address, packet.SenderPort, packet.Sender);
            }
            catch (Exception e)
            {
                logger.LogError(e, "error");
            };
        }
    }

    private async Task SendPingsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var addresses = GetBroadcastAddresses(); 
                //TODO: Allow IPv6
                addresses.Where(x=>x.AddressFamily == AddressFamily.InterNetwork).Select(x => SendPingAsync(x, cancellationToken)).ToList();
                await Task.Delay(configuration.Value.DiscoveryDelay);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException)
            {
            }
        }
    }

    private async Task SendPingAsync(IPAddress iPAddress, CancellationToken cancellationToken)
    {
        var PingerClient = new UdpClient();
        PingerClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        PingerClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);

        await PingerClient.SendAsync(RequestData, new IPEndPoint(iPAddress, configuration.Value.DiscoveryServerPort), cancellationToken);
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
