using MessagePack;

namespace Firebridge.Common.Models.Packets;

[MessagePackObject]
public sealed class StartProgramModelPacket
{
    [IgnoreMember]
    public const uint ActiveSessionId = uint.MaxValue;

    [Key(0)]
    public required string Type { get; init; }

    [Key(1)]
    public required uint SessionId { get; init; } = ActiveSessionId;

    [Key(2)]
    public required ICollection<byte[]> Assemblies { get; init; }

    [Key(3)]
    public required Guid OwnerGuid { get; init; }

    [Key(4)]
    public required Guid ControllerGuid { get; init; }

    [Key(5)]
    public required Guid AgentGuid { get; init; }

    [Key(6)]
    public bool DebugMode { get; init; } = false;

    [Key(7)]
    public bool LogToConsole { get; init; } = false;
}
