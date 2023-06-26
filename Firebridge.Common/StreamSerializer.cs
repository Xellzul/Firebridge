using Firebridge.Common.Models;
using MessagePack;
using System.Buffers;

namespace Firebridge.Common;

public static class StreamSerializer
{
    public static async Task SendAsync<T>(Stream stream, Packet<T> data, CancellationToken cancellationToken = default)
    {
        //var resolver = CompositeResolver.Create(
        //    new IMessagePackFormatter[] { TypelessFormatter.Instance },
        //    new IFormatterResolver[] { TypelessObjectResolver.Instance, StandardResolverAllowPrivate.Instance });

        //var options = MessagePackSerializerOptions.Standard
        //    .WithCompression(MessagePackCompression.Lz4BlockArray)
        //    .WithResolver(resolver);

        await MessagePackSerializer.Typeless.SerializeAsync(stream, data, cancellationToken: cancellationToken);
        await stream.FlushAsync();
    }

    public static async Task<object> RecieveAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var streamReader = new MessagePackStreamReader(stream, true);
        var rawData = await streamReader.ReadAsync(cancellationToken);

        if (rawData == null)
            throw new InvalidOperationException("Cannot read from stream");

        var data = (ReadOnlySequence<byte>)rawData;

        var packet = MessagePackSerializer.Typeless.Deserialize(data, cancellationToken: cancellationToken);

        ArgumentNullException.ThrowIfNull(packet);

        return packet;
    }
}
