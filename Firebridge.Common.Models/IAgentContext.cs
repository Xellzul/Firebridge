namespace Firebridge.Common.Models;

public interface IAgentContext
{
    public Task<object> Recieve(CancellationToken cancellationToken = default);

    public Task Send<T>(T data, Guid targetController, Guid targetProgram, CancellationToken cancellationToken = default);

    public Task Return<T>(T data, CancellationToken cancellationToken = default);
}

