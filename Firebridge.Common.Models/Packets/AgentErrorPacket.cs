using MessagePack;

namespace Firebridge.Common.Models.Packets;

[MessagePackObject]
public sealed class AgentErrorPacket
{
    [Key(0)]
    public required string Line{ get; init; }
}