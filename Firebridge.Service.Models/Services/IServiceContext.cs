namespace Firebridge.Service.Models.Services;

public interface IServiceContext
{
    public void RegisterAgent(IAgent agent);

    public void UnregisterAgent(IAgent agent);

    public IAgent GetAgent(Guid agentId);

    public ICollection<IAgent> ResolveAgents(Guid agentId);

    public void RegisterController(IControllerConnection controller);

    public void UnregisterController(IControllerConnection agent);

    public IControllerConnection GetController(Guid agentId);

    public ICollection<IControllerConnection> ResolveController(Guid agentId);
}
