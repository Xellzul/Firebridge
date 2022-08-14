namespace Firebridge.Core.Networking.Tests;

public class DiscoveryTests
{
    [Test]
    public async Task Server_Start_And_Stops()
    {
        DiscoveryServer discoveryServer = new DiscoveryServer(new Guid(), "secret", 47000);
        _ = discoveryServer.RunAsync();
        await Task.Delay(10);
        discoveryServer.Stop();
    }

    [Test]
    public async Task Server_Accepts_Client_And_Sends_Guid()
    {
        var port = 47000;
        var secret = "someSecret";
        var guid = Guid.NewGuid();

        var discoveryServer = new DiscoveryServer(guid, secret, port);
        var discoveryClient = new DiscoveryClient(secret, port);

        _ = discoveryServer.RunAsync();
        await Task.Delay(1);

        var cts = new CancellationTokenSource();

        var client = await discoveryClient.DiscoverClient(cts.Token);

        Assert.That(client.Id, Is.EqualTo(guid));

        discoveryServer.Stop();
    }

    [Test]
    public async Task Server_Rejects_Wrong_Secret()
    {
        var port = 47000;
        var secret = "someSecret";
        var wrongSecret = "someSecrets";
        var guid = Guid.NewGuid();

        var discoveryServer = new DiscoveryServer(guid, secret, port);
        var discoveryClient = new DiscoveryClient(wrongSecret, port);

        _ = discoveryServer.RunAsync();
        await Task.Delay(1);

        var cts = new CancellationTokenSource(100);

        var test= Assert.ThrowsAsync<OperationCanceledException>(async () => await discoveryClient.DiscoverClient(cts.Token));

        discoveryServer.Stop();
    }

    [Test]
    public async Task Server_Accepts_Client_Multiple_Times()
    {
        var port = 47000;
        var secret = "someSecret";
        var guid = Guid.NewGuid();

        var discoveryServer = new DiscoveryServer(guid, secret, port);
        var discoveryClient = new DiscoveryClient(secret, port);

        _ = discoveryServer.RunAsync();
        await Task.Delay(1);

        var cts = new CancellationTokenSource();

        var client = await discoveryClient.DiscoverClient(cts.Token);
        Assert.That(client.Id, Is.EqualTo(guid));
        client = await discoveryClient.DiscoverClient(cts.Token);
        Assert.That(client.Id, Is.EqualTo(guid));
        client = await discoveryClient.DiscoverClient(cts.Token);
        Assert.That(client.Id, Is.EqualTo(guid));

        discoveryServer.Stop();
    }
}
