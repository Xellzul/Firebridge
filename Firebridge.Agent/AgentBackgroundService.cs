using Firebridge.Common.Models.Packets;
using Firebridge.Common.Models;
using Microsoft.Extensions.Hosting;

namespace Firebridge.Agent;

public abstract class AgentBackgroundService : BackgroundService
{
    private readonly IAgentContext context;
    private readonly IHostApplicationLifetime lifeTime;

    public AgentBackgroundService(IAgentContext context, IHostApplicationLifetime lifeTime) 
    {
        this.context = context;
        this.lifeTime = lifeTime;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        await context.Return(new AgentStatusChangedPacket { State = AgentState.Running }, cancellationToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
        await context.Return(new AgentStatusChangedPacket { State = AgentState.Stopped }, stoppingToken);
        lifeTime.StopApplication();
    }
}
