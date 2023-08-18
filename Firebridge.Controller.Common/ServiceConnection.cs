using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Packets;
using Firebridge.Common.Models.Services;
using Firebridge.Controller.Models;
using Firebridge.Controller.Models.Notifications;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Loader;

namespace Firebridge.Controller.Common;

public class ServiceConnection : IServiceConnection, IDisposable
{
    public Guid ServiceGuid { get; set; }
    public Guid ServiceId { get; set; }
    public AsyncServiceScope Scope { get; set; }

    private readonly ILogger<ServiceConnection> logger;
    private readonly IFingerprintService fingerprintService;
    private readonly IMediator mediator;
    private readonly ConcurrentDictionary<Guid, Agent> agents = new ConcurrentDictionary<Guid, Agent>();

    private TcpClient client;
    private PacketStream packetStream = null!;

    public ServiceConnection(ILogger<ServiceConnection> logger, IFingerprintService fingerprintService, IMediator mediator)
    {
        this.logger = logger;
        this.fingerprintService = fingerprintService;
        this.mediator = mediator;

        client = new TcpClient();
    }

    public async Task Connect(IPEndPoint iPEndPoint, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Connecting to service {@address}:{@ip}", iPEndPoint.Address.ToString(), iPEndPoint.Port);

        try
        {
            await mediator.Publish(new ServiceStatusChangedNotification(this, ServiceConnectionState.Connecting), cancellationToken);
            await client.ConnectAsync(iPEndPoint);
            packetStream = new PacketStream(client.GetStream());

            var handshake = new HandshakePacket() { Guid = fingerprintService.GetFingerprint() };
            await Send(handshake, Guid.Empty, Guid.Empty, cancellationToken);

            var response = await packetStream.ReadAsync(cancellationToken);
            var handshakeResponse = response.Payload as HandshakePacket; //TODO

            if (handshakeResponse == null)
                throw new InvalidOperationException($"Wrong data type {response.GetType()}");

            if (handshakeResponse.Guid != response.Sender)
                throw new InvalidOperationException($"Mismatched guid {handshakeResponse.Guid} - {response.Sender}");

            ServiceGuid = handshakeResponse.Guid;
            await mediator.Publish(new ServiceStatusChangedNotification(this, ServiceConnectionState.Connected), cancellationToken);

            logger.LogInformation($"Connected to: {ServiceGuid}");

            while (!cancellationToken.IsCancellationRequested && client.Connected)
            {
                var packet = await packetStream.ReadAsync(cancellationToken);

                await mediator.Publish(new AgentMessageNotification(packet, this));
            }
        }
        catch (IOException e) when (e.InnerException is SocketException)
        {
            if(((SocketException)e.InnerException).SocketErrorCode == SocketError.ConnectionReset)
            {
                logger.LogInformation($"Forcibly disconnected: {ServiceGuid}");
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
        await mediator.Publish(new ServiceStatusChangedNotification(this, ServiceConnectionState.Disconnected), cancellationToken);

        logger.LogInformation($"Disconnected from: {ServiceGuid}");
    }

    public async Task StartAgent(Type program, Guid ownerProgramId = default, Guid agentGuid = default, CancellationToken cancellationToken = default)
    {
        if (ownerProgramId == default)
            ownerProgramId = Guid.NewGuid();

        if (agentGuid == default)
            agentGuid = Guid.NewGuid();

        var agent = new Agent(program);

        if (!agents.TryAdd(ownerProgramId, agent))
            throw new InvalidOperationException("Failed to add Agent");

        await mediator.Publish(new AgentStatusChangedNotification(agent, AgentState.Starting));

        var assembly = program.Assembly;
        var context = AssemblyLoadContext.GetLoadContext(assembly);
        ArgumentNullException.ThrowIfNull(context);

        //var locations = GetAssembliesLocations(assembly, context).Distinct();
        var locations = context.Assemblies.Select(x => x.Location);

        var loadedFiles = await Task.WhenAll(locations.Select(x => File.ReadAllBytesAsync(x)));

        var programFullName = program + ", " + program.Assembly.FullName;

        var payload = new StartProgramModelPacket() {
            AgentGuid = agentGuid, 
            SessionId = StartProgramModelPacket.ActiveSessionId, 
            Type = programFullName, 
            Assemblies = loadedFiles, 
            OwnerGuid = ownerProgramId,
            ControllerGuid = fingerprintService.GetFingerprint()
        };

        await Send(payload, ownerProgramId, agentGuid, cancellationToken);
    }

    //private IEnumerable<string> GetAssembliesLocations(Assembly assembly, AssemblyLoadContext context)
    //{
    //    var asmLoc = assembly.Location;

    //    if (string.IsNullOrWhiteSpace(asmLoc))
    //        throw new Exception("TODo");

    //    foreach (var asmName in assembly.GetReferencedAssemblies())
    //    {
    //        var asm = context.LoadFromAssemblyName(asmName);
    //        foreach (var location in GetAssembliesLocations(asm, context))
    //        {
    //            yield return location;
    //        }
    //    }

    //    yield return asmLoc;
    //}

    public Task Send<T>(T data, Guid senderProgram, Guid targetProgram, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        var packet = new Packet { Payload = data, Sender = fingerprintService.GetFingerprint(), SenderProgram = senderProgram, Target =  ServiceGuid, TargetProgram = targetProgram };
        logger.LogDebug("Sending packet: {@packet}", packet);

        return StreamSerializer.SendAsync(client.GetStream(), packet, cancellationToken);
    }

    public void Dispose()
    {
       // throw new NotImplementedException();
    }
}
