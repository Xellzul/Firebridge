using MessagePack;

namespace Firebridge.Common.Models.Packets;

[MessagePackObject]
public sealed class AgentStatusChangedPacket
{
    [Key(0)]
    public required AgentState State { get; init; }
}