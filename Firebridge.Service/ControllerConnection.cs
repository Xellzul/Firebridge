using Firebridge.Common;
using Firebridge.Common.Models.Packets;
using Firebridge.Common.Models.Services;
using Firebridge.Service.Models.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Sockets;

namespace Firebridge.Service;

internal class ControllerConnection : IControllerConnection
{
    public Guid ControllerGuid { get; private set; } = Guid.Empty;

    private readonly ILogger<ControllerConnection> logger;
    private readonly IFingerprintService fingerprintService;
    private readonly IServiceContext serviceContext;
    private readonly IServiceProvider provider;

    private TcpClient? client;
    private PacketStream packetStream = null!;

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
        packetStream = new PacketStream(tcpClient.GetStream());

        var handshake = new HandshakePacket() { Guid = fingerprintService.GetFingerprint() };

        await Send(handshake, Guid.Empty, cancellationToken);

        var resopnse = await ReadPacket(cancellationToken);
        var handshakeResponse = resopnse.Payload as HandshakePacket;

        if (handshakeResponse == null)
            throw new InvalidOperationException($"Wrong data type {resopnse.GetType()}");

        if (handshakeResponse.Guid != resopnse.Sender)
            throw new InvalidOperationException($"Mismatched guid {handshakeResponse.Guid} - {resopnse.Sender}");

        ControllerGuid = handshakeResponse.Guid;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        while (client.Connected && !cancellationToken.IsCancellationRequested)
        {
            var packet = await ReadPacket(cancellationToken);

            logger.LogDebug("Reading packet from {owner} of {@packet}", ControllerGuid, packet);

            switch (packet.Payload)
            {
                case StartProgramModelPacket p:
                    logger.LogInformation($"Starting program {p.Type}");
                    StartProgram(p);
                    break;

                default:
                    //Todo handle invalid address
                    var program = serviceContext.GetAgent(packet.TargetProgram);
                    await program.SendData(packet.Payload, packet.Sender, packet.SenderProgram, cancellationToken);
                    break;
            }

            //TODO: HERE
        }
    }

    private void StartProgram(StartProgramModelPacket startProgramModel)
    {
        var AgentCancellationTokenSource = new CancellationTokenSource();
        
        var agentTask = new Task(async () =>
        {
            IAgent? agent = null;
            try
            {
                agent = provider.GetRequiredService<IAgent>();
                agent.PrepareAgent(startProgramModel);
                serviceContext.RegisterAgent(agent);

                await agent.ExecuteAsync(startProgramModel, AgentCancellationTokenSource.Token);
            }
            catch(Exception e) 
            {
                logger.LogError(e, "error in agent");
            }
            finally
            {
                if(agent != null)
                    serviceContext.UnregisterAgent(agent);

                AgentCancellationTokenSource.Cancel();
            }
        }, AgentCancellationTokenSource.Token, TaskCreationOptions.LongRunning);

        agentTask.Start();
    }

    private async Task<Packet> ReadPacket(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(packetStream);

        var packet = await packetStream.ReadAsync(cancellationToken);

        logger.LogTrace($"Recieving packet: {packet} ({packet.Payload.GetType()})");
        return packet;
    }

    public Task Send<T>(T data, Guid ToProgram, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(packetStream);

        var packet = new Packet { Payload = data, SenderProgram = Guid.Empty, Target = ControllerGuid, TargetProgram = ToProgram, Sender = fingerprintService.GetFingerprint() };
        logger.LogTrace($"Sending packet: {packet}");

        return packetStream.SendAsync(packet, cancellationToken);
    }

    public Guid GetId() => ControllerGuid;
}
