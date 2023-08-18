using Firebridge.Common.Models.Packets;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System.Buffers;
using System.Net.Sockets;

namespace Firebridge.Common;

public static class StreamSerializer
{
    static StreamSerializer()
    {
        var resolver = CompositeResolver.Create(
            new IMessagePackFormatter[] { TypelessFormatter.Instance },
            new IFormatterResolver[] { TypelessContractlessStandardResolver.Instance });

        var options = MessagePackSerializerOptions.Standard
            .WithResolver(resolver);

        MessagePackSerializer.DefaultOptions = options;
    }

    public static async Task SendAsync(Stream stream, Packet packet, CancellationToken cancellationToken)
    {
        await MessagePackSerializer.SerializeAsync(stream, packet, cancellationToken: cancellationToken);
        
        await stream.FlushAsync(cancellationToken);
    }

    public static async Task<Packet> ReceiveAsync(MessagePackStreamReader messagePackStreamReader, CancellationToken cancellationToken = default)
    {
        var rawData = await messagePackStreamReader.ReadAsync(cancellationToken);
        ArgumentNullException.ThrowIfNull(rawData);

        return MessagePackSerializer.Deserialize<Packet>((ReadOnlySequence<byte>)rawData, cancellationToken: cancellationToken);

        //using (var messagePackStreamReader = new MessagePackStreamReader(stream, true))
        //{

        //}
        //var streamReader = new MessagePackStreamReader(stream, true, new SequencePool(1));
        //var rawData = await streamReader.ReadAsync(cancellationToken);

        //if (rawData == null)
        //    throw new InvalidOperationException("Cannot read from stream");

        //var data = (ReadOnlySequence<byte>)rawData;

        //var packet = MessagePackSerializer.Typeless.Deserialize(data, cancellationToken: cancellationToken);

        //ArgumentNullException.ThrowIfNull(packet);
        //return (Packet)packet;
    }

    public static async Task<byte[]> SerializeDataAsync(object data, CancellationToken cancellationToken = default)
    {
        var ms = new MemoryStream();

        await MessagePackSerializer.SerializeAsync(ms, data, cancellationToken: cancellationToken);

        return ms.ToArray();
    }

    public static object DeserializeData(byte[] data, CancellationToken cancellationToken = default)
    {        
        var packet = MessagePackSerializer.Typeless.Deserialize(data, cancellationToken: cancellationToken);

        ArgumentNullException.ThrowIfNull(packet);
        return packet;
    }

    public static T Deserialize<T>(byte[] data)
    {
        var packet = MessagePackSerializer.Deserialize<T>(data);

        ArgumentNullException.ThrowIfNull(packet);
        return packet;
    }

    public static byte[] Serialize<T>(T payload)
    {
        return MessagePackSerializer.Serialize(payload);
    }
}