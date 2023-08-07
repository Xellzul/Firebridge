using Firebridge.Common;
using Firebridge.Common.Models.Services;
using Firebridge.Controller.Common;
using Firebridge.Controller.Models;
using Firebridge.Controller.WinUI;
using MessagePack.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Runtime.InteropServices;

namespace Firebridge.Controller;
public static class MauiProgram
{
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();

    public static MauiApp CreateMauiApp()
    {
        Directory.SetCurrentDirectory(Path.GetDirectoryName(AppContext.BaseDirectory));

        //TODO move this?
        AllocConsole();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.File("Logs/ControllerStartup.log")
            .WriteTo.Console()
            .CreateBootstrapLogger();

        var builder = MauiApp.CreateBuilder();


        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Logging.AddSerilog(dispose: true);

        builder.Services.AddOptions();

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();


        builder.Services.Configure<FirebridgeControllerSettings>(config);

        builder.Configuration.AddConfiguration(config);

        var pluginService = PluginService.LoadPlugins("plugins", builder.Services);

        builder.Services.AddScoped<IServiceConnection, ServiceConnection>();
        builder.Services.AddScoped<ServiceMiniView>();

        builder.Services.AddSingleton<IPluginService>(pluginService);
        builder.Services.AddSingleton<IControllerContext, ControllerContext>();
        builder.Services.AddSingleton<IFingerprintService, FingerprintService>();
        builder.Services.AddSingleton<IDiscoveryClient, DiscoveryClient>();
        builder.Services.AddSingleton<Dashboard>();


        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
                                      .CreateLogger();

        builder.Services.AddLogging();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
