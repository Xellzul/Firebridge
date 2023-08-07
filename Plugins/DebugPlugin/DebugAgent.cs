using Firebridge.Agent;
using Firebridge.Common.Models;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace DebugPlugin;
public class DebugAgent : AgentBackgroundService
{
    public DebugAgent(IAgentContext context, IHostApplicationLifetime lifeTime) : base(context, lifeTime)
    {
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Debugger.Launch();

        return Task.CompletedTask;
    }
}