using Firebridge.Service.Models.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Firebridge.Service;

public class ControllersContext : IServiceContext
{
    private ConcurrentDictionary<Guid, IAgent> Agents { get; init; } = new ConcurrentDictionary<Guid, IAgent>();
    private ConcurrentDictionary<Guid, IControllerConnection> Controllers { get; init; } = new ConcurrentDictionary<Guid, IControllerConnection>();

    private readonly ILogger<ControllersContext> logger;

    public ControllersContext(ILogger<ControllersContext> logger)
    {
        this.logger = logger;
    }

    public IAgent GetAgent(Guid agentId)
    {
        return Agents.Single(x => x.Key == agentId).Value;
    }

    public IControllerConnection GetController(Guid controllerId)
    {
        return Controllers.Single(x => x.Key == controllerId).Value;
    }

    public void RegisterAgent(IAgent agent)
    {
        logger.LogInformation("Registering agent: {agent}", agent.GetId());

        if (!Agents.TryAdd(agent.GetId() , agent))
            throw new InvalidProgramException("Cannot register Agent");
    }

    public void RegisterController(IControllerConnection controller)
    {
        logger.LogInformation("Registering controller: {controller}", controller.GetId());

        if (!Controllers.TryAdd(controller.GetId(), controller))
            throw new InvalidProgramException("Cannot register Controller");
    }

    public ICollection<IAgent> ResolveAgents(Guid agentId)
    {
        if (agentId == Guid.Empty)
            return Agents.Values;

        return new[] { Agents.Single(x => x.Key == agentId).Value };
    }

    public ICollection<IControllerConnection> ResolveController(Guid controllerId)
    {
        if (controllerId == Guid.Empty)
            return Controllers.Values;

        return new[] { Controllers.Single(x => x.Key == controllerId).Value };
    }

    public void UnregisterAgent(IAgent agent)
    {
        logger.LogInformation("Deleting agent: {agent}", agent.GetId());
        if (!Agents.TryRemove(agent.GetId(), out _))
            throw new InvalidProgramException("Cannot unregister Agent");
    }

    public void UnregisterController(IControllerConnection controller)
    {
        logger.LogInformation("Deleting controller: {controller}", controller.GetId());
        if (!Controllers.TryRemove(controller.GetId(), out _))
            throw new InvalidProgramException("Cannot unregister Controller");
    }
}
