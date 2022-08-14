using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Firebridge.Core.Networking;

/// <summary>
/// Used by Service that wants to be discovered
/// </summary>
public class DiscoveryServer
{
    public string Secret { get; private set; }
    public int Port { get; private set; }
    public Guid Id { get; private set; }

    private readonly CancellationTokenSource CancellationTokenSource;

    private readonly CancellationToken CancellationToken;
    private readonly UdpClient Server;

    /// <param name="id">conrete id for app instance</param>
    /// <param name="secret">Common app secret code</param>
    /// <param name="port">udp port</param>
    public DiscoveryServer(Guid id, string secret, int port)
    {
        Id = id;
        Secret = secret;
        Port = port;
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;

        Server = new UdpClient(AddressFamily.InterNetworkV6);
        Server.Client.EnableBroadcast = true;
        Server.Client.DualMode = true;
    }

    public async Task RunAsync()
    {
        Server.Client.Bind(new IPEndPoint(IPAddress.IPv6Any, Port));
        var ResponseData = new ReadOnlyMemory<byte>(Encoding.ASCII.GetBytes(Secret + Id.ToString()));

        while (!CancellationToken.IsCancellationRequested && Server.Client.IsBound)
        {
            try
            {
                var data = await Server.ReceiveAsync(CancellationToken);

                var ASNCIIResponse = Encoding.ASCII.GetString(data.Buffer);
                if (ASNCIIResponse.StartsWith(Secret))
                {
                    if(int.TryParse(ASNCIIResponse.Substring(Secret.Length), out int port))
                    {
                        await Server.SendAsync(ResponseData, new IPEndPoint(data.RemoteEndPoint.Address, port), CancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.OperationAborted)
            {
                break;
            }
        }
    }

    public void Stop()
    {
        CancellationTokenSource.Cancel();
        Server.Close();
    }
}
