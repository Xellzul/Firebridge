namespace Firebridge.Service.Models.Services;

public interface IServiceContext
{
    public void RegisterAgent(IAgent agent);

    public void UnregisterAgent(IAgent agent);
}
