using Firebridge.Common.Models.Packets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Firbridge.Agent;

internal class AgentHelper : BackgroundService
{
    private readonly AgentContext context;
    private readonly ILogger<AgentHelper> logger;

    public AgentHelper(AgentContext context, ILogger<AgentHelper> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var line = await context.ReadOut(stoppingToken);

            if (line == null)
                continue;

            logger.LogDebug("Returning line from SIN {line}", line);
            await context.Return(new StringPacket() { Line = line }, stoppingToken);
        }
    }
}