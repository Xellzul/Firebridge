using MessagePack;
namespace Firebridge.Common.Models.Packets;

[MessagePackObject]
public sealed class PluginDataPacket
{
    [Key(0)]
    public required byte[] data { get; init; }
}