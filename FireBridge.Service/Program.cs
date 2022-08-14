using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.Reflection;

namespace FireBridge.Service;

public class Program
{
    public const string ServiceName = "FireBridge Service";

    public static async Task Main()
    {
        Directory.SetCurrentDirectory(Path.GetDirectoryName(AppContext.BaseDirectory)!);

        if (Environment.UserInteractive && !Debugger.IsAttached)
        {
            var status = ServiceInstaller.GetServiceStatus(ServiceName);
            Console.WriteLine("Service status is: " + status.ToString());

            if (status != ServiceState.Running && status != ServiceState.NotFound)
            {
                Console.WriteLine("Stopping old serice");
                ServiceInstaller.StopService(ServiceName);
                status = ServiceInstaller.GetServiceStatus(ServiceName);
                Console.WriteLine("Service status is: " + status.ToString());
            }

            if (ServiceInstaller.ServiceIsInstalled(ServiceName))
            {
                Console.WriteLine("Uninstalling old serice");
                ServiceInstaller.Uninstall(ServiceName);
                status = ServiceInstaller.GetServiceStatus(ServiceName);
                Console.WriteLine("Service status is: " + status.ToString());
            }


            Console.WriteLine("Installing Service");

            ServiceInstaller.InstallAndStart(ServiceName, ServiceName, Process.GetCurrentProcess().MainModule!.FileName!);

            status = ServiceInstaller.GetServiceStatus(ServiceName);
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
                    .AddJsonFile("appsettings.json");
            }
            )
            .UseWindowsService()
            .ConfigureServices((builder, services) =>
            {
                services.Configure<AppSettings>(builder.Configuration);
                services.AddHostedService<FireBridgeService>();
                services.AddSingleton<ServiceServer>();
            })
            .UseSerilog((hostingContext, loggerConfiguration)
            =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
            })
            .Build();

        await host.RunAsync();
    }
}
