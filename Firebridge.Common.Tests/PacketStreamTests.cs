using Firebridge.Common.Models.Packets;

namespace Firebridge.Common.Tests;

public class PacketStreamTests
{
    [Test]
    public async Task CanSendObjects()
    {
        using var ms = new MemoryStream();
        using var packetStreamIn = new PacketStream(ms);
        using var packetStreamOut = new PacketStream(ms);

        var data = new Packet()
        {
            Payload = "b",
            Sender = Guid.NewGuid(),
            SenderProgram = Guid.NewGuid(),
            Target = Guid.NewGuid(),
            TargetProgram = Guid.NewGuid()
        };

        var data2 = new Packet()
        {
            Payload = "c",
            Sender = Guid.NewGuid(),
            SenderProgram = Guid.NewGuid(),
            Target = Guid.NewGuid(),
            TargetProgram = Guid.NewGuid()
        };

        await packetStreamIn.SendAsync(data, default);
        await packetStreamIn.SendAsync(data2, default);

        ms.Seek(0, SeekOrigin.Begin);

        var dataDeserialized = await packetStreamOut.ReadAsync(default);
        var dataDeserialized2 = await packetStreamOut.ReadAsync(default);

        Assert.That(dataDeserialized.GetType(), Is.EqualTo(data.GetType()));
        Assert.That(dataDeserialized.Payload.GetType(), Is.EqualTo(data.Payload.GetType()));
        Assert.That((string)dataDeserialized.Payload, Is.EqualTo((string)data.Payload));
        Assert.That(dataDeserialized.Sender, Is.EqualTo(data.Sender));
        Assert.That(dataDeserialized.SenderProgram, Is.EqualTo(data.SenderProgram));
        Assert.That(dataDeserialized.Target, Is.EqualTo(data.Target));
        Assert.That(dataDeserialized.TargetProgram, Is.EqualTo(data.TargetProgram));

        Assert.That(dataDeserialized2.GetType(), Is.EqualTo(data2.GetType()));
        Assert.That(dataDeserialized2.Payload.GetType(), Is.EqualTo(data2.Payload.GetType()));
        Assert.That((string)dataDeserialized2.Payload, Is.EqualTo((string)data2.Payload));
        Assert.That(dataDeserialized2.Sender, Is.EqualTo(data2.Sender));
        Assert.That(dataDeserialized2.SenderProgram, Is.EqualTo(data2.SenderProgram));
        Assert.That(dataDeserialized2.Target, Is.EqualTo(data2.Target));
        Assert.That(dataDeserialized2.TargetProgram, Is.EqualTo(data2.TargetProgram));
    }
}
