using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Services;
using Firebridge.Controller.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Firebridge.Controller.Common;

public class ServiceConnection : IServiceConnection
{
    public Guid ServiceGuid { get; private set; }

    private readonly ILogger<ServiceConnection> logger;
    private readonly IFingerprintService fingerprintService;
    private readonly IServiceCallback controllerCallback;

    private TcpClient client;

    public ServiceConnection(ILogger<ServiceConnection> logger, IFingerprintService fingerprintService, IServiceCallback controllerCallback)
    {
        this.logger = logger;
        this.fingerprintService = fingerprintService;
        this.controllerCallback = controllerCallback;

        client = new TcpClient();
    }

    public async Task Connect(IPEndPoint iPEndPoint, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Connecting to serivce {iPEndPoint}");

        try
        {
            controllerCallback.Connecting(this);

            await client.ConnectAsync(iPEndPoint);

            var handshake = new Handshake() { Guid = fingerprintService.GetFingerprint() };
            await Send(handshake, cancellationToken);

            var resopnse = await ReadPacket(cancellationToken);
            var handshakeResponse = resopnse as Packet<Handshake>; //TODO

            if (handshakeResponse == null)
                throw new InvalidOperationException($"Wrong data type {resopnse.GetType()}");

            if (handshakeResponse.Payload.Guid != handshakeResponse.Sender)
                throw new InvalidOperationException($"Missmatched guid {handshakeResponse.Payload.Guid} - {handshakeResponse.Sender}");

            ServiceGuid = handshakeResponse.Payload.Guid;

            controllerCallback.Connected(this);

            logger.LogInformation($"Connected to: {ServiceGuid}");

            while (!cancellationToken.IsCancellationRequested && client.Connected)
            {
                var packet = await this.ReadPacket(cancellationToken);

                logger.LogCritical("TODO HERE" + packet.GetType());
            }
        }
        catch (IOException e) when (e.InnerException is SocketException)
        {
            if(((SocketException)e.InnerException).SocketErrorCode == SocketError.ConnectionReset)
            {
                logger.LogInformation($"Forcibly dissconnected: {ServiceGuid}");
            }
            else
            {
                logger.LogCritical(e, "TODO HERE");
            }
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "TODO HERE");
        }
        controllerCallback.Disconnected(this);

        logger.LogInformation($"Disconnected from: {ServiceGuid}");
    }

    private async Task<object> ReadPacket(CancellationToken cancellationToken = default)
    {
        var packet = await StreamSerializer.RecieveAsync(client.GetStream(), cancellationToken);
        ArgumentNullException.ThrowIfNull(packet);

        logger.LogDebug($"Recieving packet: {packet}");

        return packet;
    }

    public Task Send<T>(T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        var packet = new Packet<T> { Payload = data, Sender = fingerprintService.GetFingerprint() };
        logger.LogDebug($"Sending packet: {packet}");

        return StreamSerializer.SendAsync(client.GetStream(), packet, cancellationToken);
    }
}
