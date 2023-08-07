using MessagePack;

namespace Firebridge.Common.Models.Packets;

[MessagePackObject]
public sealed record HandshakePacket
{
    [Key(0)]
    public required Guid Guid { get; init; }

    public override string ToString()
    {
        return $"Id: {Guid}";
    }
}
