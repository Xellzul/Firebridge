using Firebridge.Service.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Firebridge.Common.Models.Services;
using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Packets;

namespace Firebridge.Service;

public class DiscoveryService : BackgroundService
{
    private readonly ILogger<DiscoveryService> logger;
    private readonly IOptions<FirebridgeServiceSettings> configuration;
    private readonly IFingerprintService fingerprintService;

    public DiscoveryService(ILogger<DiscoveryService> logger,
                            IOptions<FirebridgeServiceSettings> configuration,
                            IFingerprintService fingerprintService)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.fingerprintService = fingerprintService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Discovery Service Starting");

        var udpClient = new UdpClient(AddressFamily.InterNetworkV6);
        udpClient.Client.EnableBroadcast = true;
        udpClient.Client.DualMode = true;

        udpClient.Client.Bind(new IPEndPoint(IPAddress.IPv6Any, configuration.Value.DiscoveryServerPort));
        logger.LogInformation($"Bound to {udpClient.Client.LocalEndPoint}");

        ArgumentNullException.ThrowIfNull(udpClient);

        var secret = configuration.Value.DiscoveryServerSecret;

        var responseData = new ReadOnlyMemory<byte>(
            StreamSerializer.Serialize(new DiscoveryPacket() 
            { 
                Secret = configuration.Value.DiscoveryServerSecret, 
                Sender = fingerprintService.GetFingerprint(),
                SenderPort = configuration.Value.AgentServerPort
            }));

        while (!stoppingToken.IsCancellationRequested && udpClient.Client.IsBound)
        {
            try
            {
                var data = await udpClient.ReceiveAsync(stoppingToken);
                var ASNCIIResponse = Encoding.ASCII.GetString(data.Buffer);

                logger.LogTrace("Got {0} from {1}", ASNCIIResponse, data.RemoteEndPoint.ToString());

                if (ASNCIIResponse.StartsWith(secret))
                {
                    if (int.TryParse(ASNCIIResponse[secret.Length..], out int port))
                    {
                        //TODO: Do not await?
                        await udpClient.SendAsync(responseData, new IPEndPoint(data.RemoteEndPoint.Address, port), stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                // this happens on cancelation token trigger
                logger.LogInformation(e, "OperationCanceledException in Discovery server");
                break; 
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.OperationAborted)
            {
                logger.LogInformation(e, "SocketException - SocketError.OperationAborted in Discovery server");
                break; //TODO: Test or change to continue
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in Discovery server");

                throw;
            }
        }

        logger.LogInformation("Discovery Service Stopping");
        udpClient.Close();
    }
}