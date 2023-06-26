using Firebridge.Common.Models;

namespace Firebridge.Common.Tests;

public class StreamSerializerTests
{
    [Test]
    public async Task CanSendStringPacket()
    {
        var ms = new MemoryStream();
        var data = new Packet<string>() { Payload = "Hello", Sender = Guid.NewGuid()};

        await StreamSerializer.SendAsync(ms, data);

        //ms.Seek(0, SeekOrigin.Begin);
        //using StreamReader sr = new StreamReader(ms);
        //var text = sr.ReadToEnd();
        //Console.WriteLine(text);

        ms.Seek(0, SeekOrigin.Begin);

        var dataDeserialized = (Packet<string>) await StreamSerializer.RecieveAsync(ms);

        Assert.That(dataDeserialized.GetType(), Is.EqualTo(data.GetType()));
        Assert.That(dataDeserialized.Payload.GetType(), Is.EqualTo(data.Payload.GetType()));
        Assert.That(dataDeserialized.Payload, Is.EqualTo(data.Payload));
        Assert.That(dataDeserialized.Sender, Is.EqualTo(data.Sender));
    }

    [Test]
    public async Task CanSendHandshakePacket()
    {
        var ms = new MemoryStream();
        var data = new Packet<Handshake>() { Payload = new Handshake() { Guid = Guid.NewGuid() }, Sender = Guid.NewGuid() };

        await StreamSerializer.SendAsync(ms, data);

        ms.Seek(0, SeekOrigin.Begin);

        var dataDeserialized = (Packet<Handshake>) await StreamSerializer.RecieveAsync(ms);

        Assert.That(dataDeserialized.GetType(), Is.EqualTo(data.GetType()));
        Assert.That(dataDeserialized.Payload.GetType(), Is.EqualTo(data.Payload.GetType()));
        Assert.That(dataDeserialized.Payload.Guid, Is.EqualTo(data.Payload.Guid));
        Assert.That(dataDeserialized.Sender, Is.EqualTo(data.Sender));
    }
}