using Firebridge.Common.Models.Packets;
using Microsoft.Extensions.Hosting;

namespace Firbridge.Agent;

internal class AgentHelper : BackgroundService
{
    private readonly AgentContext context;

    public AgentHelper(AgentContext context)
    {
        this.context = context;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var line = await context.ReadOut(stoppingToken);

            if (line == null)
                continue;

            await context.Return(new StringPacket() { Line = line }, stoppingToken);
        }
    }
}