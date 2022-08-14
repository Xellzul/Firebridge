using System.Net;
using System.Net.Sockets;

namespace Firebridge.Core.Networking;

public class TcpServer
{
    private TcpListener Listener { get; set; }

    public int Port => ((IPEndPoint)Listener.LocalEndpoint).Port;

    public TcpServer(int port)
    {
        if (port > UInt16.MaxValue || port < 0)
            throw new ArgumentOutOfRangeException(nameof(port));

        Listener = new TcpListener(IPAddress.IPv6Any, port);
        Listener.Server.DualMode = true;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Listener.Start();
        cancellationToken.Register(() => Listener.Stop());

        while (!cancellationToken.IsCancellationRequested && Listener.Server.IsBound)
        {
            TcpClient? client = null;
            try
            {
                client = await Listener.AcceptTcpClientAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                continue;
            }

            if (client == null)
                continue;

            //TODO:
        }
    }
}