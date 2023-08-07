using Firebridge.Common.Models.Packets;

namespace Firebridge.Common.Tests;

public class StreamSerializerTests
{
    [Test]
    public async Task CanSendMultipleStringPacket()
    {
        var ms = new MemoryStream();
        var data = new Packet()
        {
            Payload = "a",
            Sender = Guid.NewGuid(),
            SenderProgram = Guid.NewGuid(),
            Target = Guid.NewGuid(),
            TargetProgram = Guid.NewGuid()
        };

        var data2 = new Packet()
        {
            Payload = "b",
            Sender = Guid.NewGuid(),
            SenderProgram = Guid.NewGuid(),
            Target = Guid.NewGuid(),
            TargetProgram = Guid.NewGuid()
        };

        await StreamSerializer.SendAsync(new StreamReader(ms).BaseStream, data);
        await StreamSerializer.SendAsync(new StreamReader(ms).BaseStream, data2);

        ms.Seek(0, SeekOrigin.Begin);

        var dataDeserialized = await StreamSerializer.RecieveAsync(ms);
        var dataDeserialized2 = await StreamSerializer.RecieveAsync(ms);

        Assert.That(dataDeserialized.GetType(), Is.EqualTo(data.GetType()));
        Assert.That(dataDeserialized.Payload.GetType(), Is.EqualTo(data.Payload.GetType()));
        Assert.That((string)dataDeserialized.Payload, Is.EqualTo((string)data.Payload));
        Assert.That(dataDeserialized.Sender, Is.EqualTo(data.Sender));
        Assert.That(dataDeserialized.SenderProgram, Is.EqualTo(data.SenderProgram));
        Assert.That(dataDeserialized.Target, Is.EqualTo(data.Target));
        Assert.That(dataDeserialized.TargetProgram, Is.EqualTo(data.TargetProgram));

        Assert.That(dataDeserialized2.Payload.GetType(), Is.EqualTo(data2.Payload.GetType()));
        Assert.That((string)dataDeserialized2.Payload, Is.EqualTo((string)data2.Payload));
    }

    [Test]
    public async Task CanSendStringPacket()
    {
        var ms = new MemoryStream();
        var data = new Packet() 
        { 
            Payload = "Hello", 
            Sender = Guid.NewGuid(), 
            SenderProgram = Guid.NewGuid(), 
            Target = Guid.NewGuid(), 
            TargetProgram = Guid.NewGuid() 
        };

        await StreamSerializer.SendAsync(ms, data);

        ms.Seek(0, SeekOrigin.Begin);

        var dataDeserialized = await StreamSerializer.RecieveAsync(ms);

        Assert.That(dataDeserialized.GetType(), Is.EqualTo(data.GetType()));
        Assert.That(dataDeserialized.Payload.GetType(), Is.EqualTo(data.Payload.GetType()));
        Assert.That((string)dataDeserialized.Payload, Is.EqualTo((string)data.Payload));
        Assert.That(dataDeserialized.Sender, Is.EqualTo(data.Sender));
        Assert.That(dataDeserialized.SenderProgram, Is.EqualTo(data.SenderProgram));
        Assert.That(dataDeserialized.Target, Is.EqualTo(data.Target));
        Assert.That(dataDeserialized.TargetProgram, Is.EqualTo(data.TargetProgram));
    }

    [Test]
    public async Task CanSendHandshakePacket()
    {
        var payloadOriginal = new HandshakePacket() { Guid = Guid.NewGuid() };

        var ms = new MemoryStream();
        var data = new Packet() 
        { 
            Payload = payloadOriginal, 
            Sender = Guid.NewGuid(), 
            SenderProgram = Guid.NewGuid(), 
            Target = Guid.NewGuid(),
            TargetProgram = Guid.NewGuid() 
        };

        await StreamSerializer.SendAsync(ms, data);

        ms.Seek(0, SeekOrigin.Begin);

        var dataDeserialized = await StreamSerializer.RecieveAsync(ms);

        Assert.That(dataDeserialized.GetType(), Is.EqualTo(data.GetType()));
        Assert.That(dataDeserialized.Payload.GetType(), Is.EqualTo(data.Payload.GetType()));
        var payload = (HandshakePacket)dataDeserialized.Payload;

        Assert.That(payload.Guid, Is.EqualTo(payloadOriginal.Guid));
    }

    [Test]
    public async Task CanSendSerialziedPacket()
    {
        var payloadOriginal = new HandshakePacket() { Guid = Guid.NewGuid() };

        var serialized = await StreamSerializer.SerializeDataAsync(payloadOriginal);
        Assert.NotNull(serialized);

        var deserialized1 = StreamSerializer.DeserializeData(serialized);
        Assert.That(deserialized1.GetType(), Is.EqualTo(payloadOriginal.GetType()));
        Assert.That(((HandshakePacket)deserialized1).Guid, Is.EqualTo(payloadOriginal.Guid));

        var ms = new MemoryStream();
        var data = new Packet()
        {
            Payload = serialized,
            Sender = Guid.NewGuid(),
            SenderProgram = Guid.NewGuid(),
            Target = Guid.NewGuid(),
            TargetProgram = Guid.NewGuid()
        };

        await StreamSerializer.SendAsync(ms, data);

        ms.Seek(0, SeekOrigin.Begin);

        var dataDeserialized = await StreamSerializer.RecieveAsync(ms);

        Assert.That(dataDeserialized.GetType(), Is.EqualTo(data.GetType()));
        Assert.That(dataDeserialized.Payload.GetType(), Is.EqualTo(data.Payload.GetType()));
        Assert.That(dataDeserialized.Payload.GetType(), Is.EqualTo(typeof(byte[])));

        var deserialized2 = StreamSerializer.DeserializeData((byte[])dataDeserialized.Payload);
        Assert.That(deserialized2.GetType(), Is.EqualTo(payloadOriginal.GetType()));
        Assert.That(((HandshakePacket)deserialized2).Guid, Is.EqualTo(payloadOriginal.Guid));
    }
}