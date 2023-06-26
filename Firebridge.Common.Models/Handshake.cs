using MessagePack;

namespace Firebridge.Common.Models;

[MessagePackObject]
public sealed record Handshake
{
    [Key(0)]
    public required Guid Guid { get; init; }

    public override string ToString()
    {
        return $"Id: {Guid}";
    }
}
