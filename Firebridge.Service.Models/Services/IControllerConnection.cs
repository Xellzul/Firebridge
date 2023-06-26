using System.Net.Sockets;

namespace Firebridge.Service.Models.Services;

public interface IControllerConnection
{
    public Task Connect(TcpClient client, CancellationToken cancellationToken = default);

    public Task ExecuteAsync(CancellationToken cancellationToken = default);
}
