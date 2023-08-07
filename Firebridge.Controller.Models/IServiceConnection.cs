using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Firebridge.Controller.Models;

public interface IServiceConnection
{
    Guid ServiceId { get; set; }

    AsyncServiceScope Scope { get; set; }

    public Task Connect(IPEndPoint iPEndPoint, CancellationToken cancellationToken = default);

    public Task Send<T>(T data, Guid senderProgram, Guid targetProgram, CancellationToken cancellationToken = default);

    Task StartAgent(Type program, Guid ownerProgram = default, Guid agentGuid = default, CancellationToken cancellationToken = default);
}