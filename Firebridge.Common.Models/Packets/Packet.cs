using MessagePack;

namespace Firebridge.Common.Models.Packets;

[MessagePackObject]
public sealed class Packet
{
    [Key(0)]
    public required object Payload { get; init; }

    [Key(1)]
    public required Guid SenderProgram { get; init; }

    [Key(2)]
    public required Guid Sender { get; init; }

    [Key(3)]
    public required Guid Target { get; init; }

    [Key(4)]
    public required Guid TargetProgram { get; init; }

    public override string ToString()
    {
        return $"({shorten(Sender)} ({shorten(SenderProgram)})) -> ({shorten(Target)} ({shorten(TargetProgram)})) - <{Payload?.GetType()}>";
    }

    private string shorten(Guid guid)
    {
        if (guid == Guid.Empty)
            return "0";
        return guid.ToString();
    }
}