using Microsoft.Extensions.Hosting;

namespace Firbridge.Agent;

internal class Agent : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    internal async Task Start()
    {
        await Task.Delay(10000);
    }
}
