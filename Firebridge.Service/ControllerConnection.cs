using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Services;
using Firebridge.Service.Models.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace Firebridge.Service;

internal class ControllerConnection : IControllerConnection
{
    public Guid ControllerGuid { get; private set; }

    private readonly ILogger<ControllerConnection> logger;
    private readonly IFingerprintService fingerprintService;
    private readonly IServiceContext serviceContext;
    private readonly IServiceProvider provider;

    private TcpClient? client;

    public ControllerConnection(ILogger<ControllerConnection> logger, IFingerprintService fingerprintService, IServiceContext serviceContext, IServiceProvider provider)
    {
        this.logger = logger;
        this.fingerprintService = fingerprintService;
        this.serviceContext = serviceContext;
        this.provider = provider;
    }

    public async Task Connect(TcpClient tcpClient, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Controller connecting {tcpClient.Client.RemoteEndPoint}");
        client = tcpClient;

        var handshake = new Handshake() { Guid = fingerprintService.GetFingerprint() };

        await SendPacket(handshake, cancellationToken);

        var resopnse = await ReadPacket(cancellationToken);
        var handshakeResponse = resopnse as Packet<Handshake>;

        if (handshakeResponse == null)
            throw new InvalidOperationException($"Wrong data type {resopnse.GetType()}");

        if (handshakeResponse.Payload.Guid != handshakeResponse.Sender)
            throw new InvalidOperationException($"Missmatched guid {handshakeResponse.Payload.Guid} - {handshakeResponse.Sender}");

        ControllerGuid = handshakeResponse.Payload.Guid;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        while (client.Connected && !cancellationToken.IsCancellationRequested)
        {
            var packet = await ReadPacket(cancellationToken);

            switch (packet)
            {
                case Packet<object> p:
                    logger.LogWarning($"TODO5555");
                    //ROUTE
                    break;

                case Packet<StartProgramModel> p:
                    logger.LogInformation($"Starting program {p.Payload.Type}");
                    StartProgram(p.Payload);
                    break;

                default:
                    logger.LogWarning($"Recieved unknown packet type of {packet.GetType()}");
                    break;
            }

            //TODO: HERE
        }
    }

    private void StartProgram(StartProgramModel startProgramModel)
    {
        var AgentCancellationTokenSource = new CancellationTokenSource();

        var agentTask = new Task(async () =>
        {
            try
            {
                var agent = provider.GetRequiredService<IAgent>();

                serviceContext.RegisterAgent(agent);
                await agent.ExecuteAsync(startProgramModel, AgentCancellationTokenSource.Token);
            }
            catch(Exception e) 
            {
                logger.LogError(e, "error in agent");
            }
            finally
            {
                serviceContext.UnregisterAgent(agent);
                AgentCancellationTokenSource.Cancel();
            }
        }, AgentCancellationTokenSource.Token, TaskCreationOptions.LongRunning);

        agentTask.Start();
    }

    private async Task<object> ReadPacket(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        var packet = await StreamSerializer.RecieveAsync(client.GetStream(), cancellationToken);
        ArgumentNullException.ThrowIfNull(packet);

        logger.LogDebug($"Recieving packet: {packet}");

        return packet;
    }

    private Task SendPacket<T>(T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(client);

        var packet = new Packet<T> { Payload = data, Sender = fingerprintService.GetFingerprint() };
        logger.LogDebug($"Sending packet: {packet}");

        return StreamSerializer.SendAsync(client.GetStream(), packet, cancellationToken);
    }
}
