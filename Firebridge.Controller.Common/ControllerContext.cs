using Firebridge.Controller.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;

namespace Firebridge.Controller.Common;

public class ControllerContext : IControllerContext
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IPluginService pluginService;
    private readonly ILogger<ControllerContext> logger;

    private readonly ConcurrentDictionary<Guid, IServiceConnection> services = new ConcurrentDictionary<Guid, IServiceConnection>();

    public ControllerContext(IServiceScopeFactory serviceScopeFactory, ILogger<ControllerContext> logger, IPluginService pluginService)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
        this.pluginService = pluginService;
    }

    public ICollection<string> GetActions()
    {
        return pluginService.GetActions();
    }

    public ICollection<string> GetGlobalActions()
    {
        return pluginService.GetGlobalActions();
    }

    public ICollection<IServiceConnection> GetServices()
    {
        return services.Values;
    }

    public async Task TryConnectService(IPAddress address, int port, Guid id)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceConnection = scope.ServiceProvider.GetRequiredService<IServiceConnection>();
            serviceConnection.ServiceId = id;
            serviceConnection.Scope = scope;

            if (services.TryAdd(id, serviceConnection))
            {
                try
                {
                    await serviceConnection.Connect(new IPEndPoint(address, port));
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
