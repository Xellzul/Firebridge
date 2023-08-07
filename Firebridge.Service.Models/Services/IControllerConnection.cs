using System.Net.Sockets;

namespace Firebridge.Service.Models.Services;

public interface IControllerConnection
{
    public Task Connect(TcpClient client, CancellationToken cancellationToken = default);

    public Task ExecuteAsync(CancellationToken cancellationToken = default);

    public Task Send<T>(T data, Guid ToProgram, CancellationToken cancellationToken = default);

    Guid GetId();
}
