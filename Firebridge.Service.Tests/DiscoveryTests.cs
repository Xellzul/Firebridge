using Firebridge.Common.Models.Services;
using Firebridge.Service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Sockets;

namespace Firebridge.Service.Tests;

public class DiscoveryTests
{
    private const int DiscoveryPort = 47000;

    [Test]
    public async Task Server_Start_And_Stops()
    {
        Assert.True(TestHelpers.PortIsAvaibleUdp(DiscoveryPort));

        var logger = new Mock<ILogger<DiscoveryService>>(MockBehavior.Loose);
        var config = new Mock<IOptions<FirebridgeServiceSettings>>(MockBehavior.Loose);
        var fingerprint = new Mock<IFingerprintService>(MockBehavior.Loose);

        var guid = Guid.NewGuid();

        fingerprint.Setup(f => f.GetFingerprint()).Returns(guid);
        config.Setup(f => f.Value).Returns(new FirebridgeServiceSettings() { AppName = "name", AppVersion = "ver", DiscoveryServerPort = DiscoveryPort, DiscoveryServerSecret = "secret" });

        var discoveryServer = new DiscoveryService(logger.Object, config.Object, fingerprint.Object);

        var cts = new CancellationTokenSource();
        var ct = cts.Token;

        await discoveryServer.StartAsync(ct);


        Assert.False(TestHelpers.PortIsAvaibleUdp(DiscoveryPort));

        await discoveryServer.StopAsync(ct);

        Assert.True(TestHelpers.PortIsAvaibleUdp(DiscoveryPort));
        Assert.True(discoveryServer.ExecuteTask?.IsCompletedSuccessfully);
    }

    [Test]
    public async Task Server_Crashes_On_Unavaiable_Port()
    {
        Assert.True(TestHelpers.PortIsAvaibleUdp(DiscoveryPort));

        var logger = new Mock<ILogger<DiscoveryService>>(MockBehavior.Loose);
        var config = new Mock<IOptions<FirebridgeServiceSettings>>(MockBehavior.Loose);
        var fingerprint = new Mock<IFingerprintService>(MockBehavior.Loose);

        var guid = Guid.NewGuid();

        fingerprint.Setup(f => f.GetFingerprint()).Returns(guid);
        config.Setup(f => f.Value).Returns(new FirebridgeServiceSettings() { AppName = "name", AppVersion = "ver", DiscoveryServerPort = DiscoveryPort, DiscoveryServerSecret = "secret" });

        var discoveryServer = new DiscoveryService(logger.Object, config.Object, fingerprint.Object);
        var discoveryServer2 = new DiscoveryService(logger.Object, config.Object, fingerprint.Object);

        var cts = new CancellationTokenSource();
        var ct = cts.Token;

        await discoveryServer.StartAsync(ct);
        Assert.False(TestHelpers.PortIsAvaibleUdp(DiscoveryPort));

        Assert.ThrowsAsync<SocketException>(async () => await discoveryServer2.StartAsync(ct));

        Assert.False(TestHelpers.PortIsAvaibleUdp(DiscoveryPort));

        await discoveryServer.StopAsync(ct);

        Assert.True(TestHelpers.PortIsAvaibleUdp(DiscoveryPort));
    }
}