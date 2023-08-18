using Firebridge.Common.Models.Packets;
using MessagePack;

namespace Firebridge.Common;

public class PacketStream : IDisposable, IAsyncDisposable
{
    private readonly MessagePackStreamReader messagePackStreamReader;
    private readonly Stream stream;

    public bool CanRead => stream.CanRead;

    public PacketStream(Stream stream)
    {
        this.stream = stream;

        messagePackStreamReader = new MessagePackStreamReader(stream, true);
    }

    public async Task<Packet> ReadAsync(CancellationToken cancellationToken)
    {
        return await StreamSerializer.ReceiveAsync(messagePackStreamReader, cancellationToken);
    }

    public async Task SendAsync(Packet packet, CancellationToken cancellationToken)
    {
        if (!stream.CanWrite)
            throw new InvalidOperationException("Cant write into stream");

        await StreamSerializer.SendAsync(stream, packet, cancellationToken);
    }
    public void Dispose()
    {
        messagePackStreamReader.Dispose();
        stream.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        messagePackStreamReader.Dispose();
        return stream.DisposeAsync();
    }
}
