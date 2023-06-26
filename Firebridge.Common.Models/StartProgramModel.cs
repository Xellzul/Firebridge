using MessagePack;

namespace Firebridge.Common.Models;

[MessagePackObject]
public sealed class StartProgramModel
{
    public const uint ActiveSessionId = uint.MaxValue;

    [Key(0)]
    public required string Type { get; set; }

    [Key(1)]
    public required uint SessionId { get; set; } = ActiveSessionId;

    [Key(2)]
    public required ICollection<byte[]> Assemblies { get; set; }

    [Key(3)]
    public required Guid AgentGuid { get; set; }
}
