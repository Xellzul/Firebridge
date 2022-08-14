using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Firebridge.Core.Networking.Tests;

public class TcpServerTests
{
    /// <summary>
    /// Check if both IpV4 and IpV6 ports are avaible at the same time
    /// </summary>
    private static bool PortIsAvaible(int port) => !Enumerable.SequenceEqual(IPGlobalProperties.GetIPGlobalProperties()
        .GetActiveTcpListeners()
        .Where(p => p.Port == port)
        .Select(fam => fam.AddressFamily).OrderBy(x => x),
        new[] { AddressFamily.InterNetworkV6, AddressFamily.InterNetwork }.OrderBy(x => x));

    [Test]
    public async Task Server_Start_And_Stops()
    {
        var port = 64355;
        Assert.True(PortIsAvaible(port));

        TcpServer server = new TcpServer(port);
        var cancelToken = new CancellationTokenSource();

        Task acceptorTask = server.StartAsync(cancelToken.Token);
        await Task.Delay(1);

        Assert.True(!PortIsAvaible(port));

        cancelToken.Cancel();

        await acceptorTask;

        Assert.That(acceptorTask.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        Assert.True(PortIsAvaible(port));
    }

    [Test]
    public async Task Ports_Are_Handled_Correctly()
    {
        //Bounds
        Assert.Throws<ArgumentOutOfRangeException>(() => new TcpServer(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TcpServer(UInt16.MaxValue + 1));

        TcpServer server = new TcpServer(UInt16.MaxValue);
        server = new TcpServer(1);

        var cancelToken = new CancellationTokenSource();
        server = new TcpServer(0);
        var task = server.StartAsync(cancelToken.Token);
        await Task.Delay(1);
        Assert.That(server.Port, Is.GreaterThan(0));
        cancelToken.Cancel();
        await task;
    }
}