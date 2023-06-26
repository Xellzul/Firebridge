using Firebridge.Common;
using Firebridge.Common.Models.Services;
using Firebridge.Controller.Common;
using Firebridge.Controller.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
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

        builder.Services.AddTransient<IServiceConnection, ServiceConnection>();

        builder.Services.AddSingleton<IControllerContext, ControllerContext>();
        builder.Services.AddSingleton<IFingerprintService, FingerprintService>();
        builder.Services.AddSingleton<IDiscoveryClient, DiscoveryClient>();
        builder.Services.AddSingleton<Dashboard>();
        builder.Services.AddSingleton<IServiceCallback, Dashboard>(x => x.GetService<Dashboard>());

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
                                      .CreateLogger();

        builder.Services.AddLogging();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
