using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Firebridge.Service.Tests;

internal static class TestHelpers
{    /// <summary>
     /// Check if both IpV4 and IpV6 TCP ports are avaible at the same time
     /// </summary>
    internal static bool PortIsAvaibleTcp(int port) => !Enumerable.SequenceEqual(IPGlobalProperties.GetIPGlobalProperties()
        .GetActiveTcpListeners()
        .Where(p => p.Port == port)
        .Select(fam => fam.AddressFamily).OrderBy(x => x),
        new[] { AddressFamily.InterNetworkV6, AddressFamily.InterNetwork }.OrderBy(x => x));

    /// <summary>
    /// Check if both IpV4 and IpV6 UDP ports are avaible at the same time
    /// </summary>
    internal static bool PortIsAvaibleUdp(int port) => !Enumerable.SequenceEqual(IPGlobalProperties.GetIPGlobalProperties()
    .GetActiveUdpListeners()
    .Where(p => p.Port == port)
    .Select(fam => fam.AddressFamily).OrderBy(x => x),
    new[] { AddressFamily.InterNetworkV6, AddressFamily.InterNetwork }.OrderBy(x => x));
}
