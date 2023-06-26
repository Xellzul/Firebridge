using Firebridge.Common.Models;

namespace Firebridge.Service.Models.Services;

public interface IAgent
{
    public Task ExecuteAsync(StartProgramModel startProgramModel, CancellationToken token = default);

    public Task SendData<T>(T data, Guid sender, CancellationToken token = default);
}
