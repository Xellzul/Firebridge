using MessagePack;
using MessagePack.Formatters;

namespace Firebridge.Common.Models;

[MessagePackObject]
public sealed record Packet<T>
{
    [Key(0)]
    //[MessagePackFormatter(typeof(TypelessFormatter))]
    public required T Payload { get; init; }

    [Key(1)]
    public required Guid Sender { get; init; }

    public override string ToString()
    {
        return $"({Sender}) -> ({Guid.Empty}) - <{Payload}>";
        //public Guid To { get; set; }
    }
}