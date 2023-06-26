using Firebridge.Controller.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;

namespace Firebridge.Controller.Common;

public class ControllerContext : IControllerContext
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<ControllerContext> logger;

    private readonly ConcurrentDictionary<Guid, IServiceConnection> services = new ConcurrentDictionary<Guid, IServiceConnection>();

    public ControllerContext(IServiceScopeFactory serviceScopeFactory, ILogger<ControllerContext> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
    }

    public async Task TryConnectService(IPAddress address, Guid id)
    {
        try
        {

            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var serviceConnection = scope.ServiceProvider.GetRequiredService<IServiceConnection>();

            if (services.TryAdd(id, serviceConnection))
            {
                try
                {
                    await serviceConnection.Connect(new IPEndPoint(address, 47001)); //TODO NOT HARCODED 47001
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "???1");
                }

                services.TryRemove(id, out _);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "???2");
        }
    }
}
