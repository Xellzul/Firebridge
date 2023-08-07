using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Packets;

namespace Firbridge.Agent;

public class AgentContext : IAgentContext
{
    // From Service to Agent - Send objects
    private readonly Stream sin;
    // From Agent to Service - Send objects
    private readonly Stream sout;
    // Error From Agent to Service -- Send ?objects? //TODO: what
    private readonly Stream serr;
    // todo - do not use
    private readonly MemoryStream msin;
    // todo - do not use
    private readonly MemoryStream msout;

    private readonly Guid agentId;

    private readonly Guid ownerProgramId;

    private readonly Guid ownerControllerId;

    public AgentContext(Stream sin, Stream sout, Stream serr, MemoryStream msin, MemoryStream msout, Guid agentId, Guid ownerControllerId, Guid ownerProgramId)
    {
        this.sin = sin;
        this.sout = sout;
        this.serr = serr;
        this.msin = msin;
        this.msout = msout;
        this.agentId = agentId;
        this.ownerControllerId = ownerControllerId;
        this.ownerProgramId = ownerProgramId;
    }

    public Task<string?> ReadOut(CancellationToken cancellationToken = default)
    {
        return new StreamReader(msin).ReadLineAsync(cancellationToken).AsTask();
    }

    public async Task<object> Recieve(CancellationToken cancellationToken = default)
    {
        var packet = await StreamSerializer.RecieveAsync(sin, cancellationToken);
        ArgumentNullException.ThrowIfNull(packet);

        return packet;
    }

    public async Task Return<T>(T data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        var packet = new Packet { Payload = data, Sender = Guid.Empty, SenderProgram = agentId, TargetProgram = ownerProgramId, Target = ownerControllerId};

        await StreamSerializer.SendAsync(sout, packet, cancellationToken);
    }

    public async Task Send<T>(T data, Guid targetController, Guid targetProgram, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        var packet = new Packet { Payload = data, Sender = Guid.Empty, SenderProgram = agentId, Target = targetController, TargetProgram = targetProgram };

        await StreamSerializer.SendAsync(sout, packet, cancellationToken);
    }
}