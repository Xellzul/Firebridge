using MessagePack;

namespace Firebridge.Common.Models.Packets;

[MessagePackObject]
public sealed class StringPacket
{
    [Key(0)]
    public required string Line { get; init; }
}