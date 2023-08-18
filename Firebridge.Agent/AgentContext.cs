using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Packets;
using System.Diagnostics;
using System.Runtime.Loader;

namespace Firbridge.Agent;

public class AgentContext : IAgentContext
{
    // From Service to Agent - Send objects
    internal readonly PacketStream sin;
    // From Agent to Service - Send objects
    internal readonly PacketStream sout;
    // Error From Agent to Service -- Send ?objects? //TODO: what
    internal readonly Stream serr;
    // todo - do not use
    internal readonly MemoryStream msin;
    // todo - do not use
    internal readonly MemoryStream msout;

    internal readonly Guid agentId;

    internal readonly Guid ownerProgramId;

    internal readonly Guid ownerControllerId;

    internal readonly Type injectType;


    private AgentContext(PacketStream sin, PacketStream sout, Stream serr, MemoryStream msin, MemoryStream msout, Guid agentId, Guid ownerControllerId, Guid ownerProgramId, Type injectType)
    {
        this.sin = sin;
        this.sout = sout;
        this.serr = serr;
        this.msin = msin;
        this.msout = msout;
        this.agentId = agentId;
        this.ownerControllerId = ownerControllerId;
        this.ownerProgramId = ownerProgramId;
        this.injectType = injectType;
    }

    internal static async Task<AgentContext> LoadContext(CancellationToken cancellationToken)
    {
        // redirect standard IO
        var sin = new PacketStream(Console.OpenStandardInput());
        var sout = new PacketStream(Console.OpenStandardOutput());
        var serr = Console.OpenStandardError();

        //Create new IO
        //TODO: Actually read this
        var msin = new MemoryStream();
        var msout = new MemoryStream();
        var mserr = new MemoryStream();

        //Redirect buffers to IO
        Console.SetIn(new StreamReader(msin));
        Console.SetOut(new StreamWriter(msout));
        Console.SetError(new StreamWriter(mserr));

        var initPacket = await sin.ReadAsync(cancellationToken) ?? throw new InvalidProgramException("Didn't receive Init packet");
        if (initPacket.Payload is not StartProgramModelPacket)
        {
            throw new InvalidProgramException($"Init packet is wrong type {initPacket.Payload?.GetType()}");
        }

        var payload = (StartProgramModelPacket)initPacket.Payload;

        // Load assemblies
        foreach (var assembly in payload.Assemblies)
        {
            using var ms = new MemoryStream(assembly);
            AssemblyLoadContext.Default.LoadFromStream(ms);
        }

        var injectType = Type.GetType(payload.Type);
        if (injectType == null)
        {
            throw new InvalidProgramException($"{nameof(injectType)} is null");
        }

        var agentContext = new AgentContext(sin, sout, serr, msin, msout, payload.AgentGuid, payload.ControllerGuid, payload.OwnerGuid, injectType);

        return agentContext;
    }

    public Task<string?> ReadOut(CancellationToken cancellationToken = default)
    {
        return new StreamReader(msin).ReadLineAsync(cancellationToken).AsTask();
    }

    public async Task<Packet> Receive(CancellationToken cancellationToken = default)
    {
        var packet = await sin.ReadAsync(cancellationToken);
        ArgumentNullException.ThrowIfNull(packet);

        return packet;
    }

    public async Task Return<T>(T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        var packet = new Packet { Payload = data, Sender = Guid.Empty, SenderProgram = agentId, TargetProgram = ownerProgramId, Target = ownerControllerId};

        await sout.SendAsync(packet, cancellationToken);
    }

    public async Task Send<T>(T data, Guid targetController, Guid targetProgram, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        var packet = new Packet { Payload = data, Sender = Guid.Empty, SenderProgram = agentId, Target = targetController, TargetProgram = targetProgram };

        await sout.SendAsync(packet, cancellationToken);
    }
}