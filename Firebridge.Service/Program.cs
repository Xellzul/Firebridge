using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Serilog;
using Firebridge.Service.Models.Services;
using Microsoft.Extensions.DependencyInjection;
using Firebridge.Common.Models.Services;
using Firebridge.Common;
using Firebridge.Service.Models;

namespace Firebridge.Service.Win;

internal class Program
{
    public static async Task Main()
    {
        Directory.SetCurrentDirectory(Path.GetDirectoryName(AppContext.BaseDirectory)!);

        if (Environment.UserInteractive && !Debugger.IsAttached)
        {
            var status = ServiceInstaller.GetServiceStatus(Constants.ServiceName);
            Console.WriteLine("Service status is: " + status.ToString());

            if (status != ServiceState.Running && status != ServiceState.NotFound)
            {
                Console.WriteLine("Stopping old serice");
                ServiceInstaller.StopService(Constants.ServiceName);
                status = ServiceInstaller.GetServiceStatus(Constants.ServiceName);
                Console.WriteLine("Service status is: " + status.ToString());
            }

            if (ServiceInstaller.ServiceIsInstalled(Constants.ServiceName))
            {
                Console.WriteLine("Uninstalling old serice");
                ServiceInstaller.Uninstall(Constants.ServiceName);
                status = ServiceInstaller.GetServiceStatus(Constants.ServiceName);
                Console.WriteLine("Service status is: " + status.ToString());
            }


            Console.WriteLine("Installing Service");

            ServiceInstaller.InstallAndStart(Constants.ServiceName, Constants.ServiceName, Process.GetCurrentProcess().MainModule!.FileName!);

            status = ServiceInstaller.GetServiceStatus(Constants.ServiceName);
            Console.WriteLine("Service status is: " + status.ToString());

            Console.WriteLine("Installed, press any key to continue");
            Console.ReadKey(true);
            return;
        }

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                builder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("service.appsettings.json");
            }
        )
        .UseWindowsService()
        .ConfigureServices(ConfigureServices)
        .UseSerilog((hostingContext, loggerConfiguration) => { loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration); })
        .Build();

        await host.RunAsync();
    }

    private static void ConfigureServices(HostBuilderContext builder, IServiceCollection services)
    {
        services.AddSingleton<IFingerprintService, FingerprintService>();
        services.AddSingleton<IFirewallService, FirewallService>();
        services.AddSingleton<IApplicationLoader, ApplicationLoader>();

        services.Configure<FirebridgeServiceSettings>(builder.Configuration);

        services.AddTransient<IAgent, Agent>();

        services.AddScoped<IControllerConnection, ControllerConnection>();

        services.AddSingleton<IServiceContext, ControllersContext>();

        services.AddHostedService<DiscoveryService>();
        services.AddHostedService<ControllerService>();
    }
}