using MessagePack;

namespace Firebridge.Common.Models.Packets;

[MessagePackObject]
public sealed class DiscoveryPacket
{
    [Key(0)]
    public required string Secret { get; init; }

    [Key(1)]
    public required Guid Sender { get; init; }

    [Key(2)]
    public required int SenderPort { get; init; }

    public override string ToString()
    {
        return $"({Secret}) from <{Sender}:{SenderPort}>";
    }
}