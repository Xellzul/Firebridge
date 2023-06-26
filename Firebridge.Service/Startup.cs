using Firebridge.Service.Models;
using Firebridge.Service.Models.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Firebridge.Service;

public class Startup
{
    public static void ConfigureServices(HostBuilderContext builder, IServiceCollection services)
    {
        services.Configure<FirebridgeServiceSettings>(builder.Configuration);

        services.AddTransient<IAgent, Agent>();

        services.AddScoped<IControllerConnection,  ControllerConnection>();

        services.AddSingleton<IServiceContext, ControllersContext>();

        services.AddHostedService<DiscoveryService>();
        services.AddHostedService<ControllerService>();
    }
}
