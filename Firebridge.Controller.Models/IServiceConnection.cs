using System.Net;

namespace Firebridge.Controller.Models;

public interface IServiceConnection
{
    public Task Connect(IPEndPoint iPEndPoint, CancellationToken cancellationToken = default);

    public Task Send<T>(T data, CancellationToken cancellationToken = default);
}