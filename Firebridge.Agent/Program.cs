using Firebridge.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Text.Json;

namespace Firbridge.Agent;

public class Program
{
    public static async Task Main(string[] args)
    {
        var processId = Environment.ProcessId;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.File("Logs/AgentStartup.log", shared: true)
            .CreateBootstrapLogger();

        Log.Logger.Information($"{processId}: Starting Agent with Process ID: '{processId}' and args {JsonSerializer.Serialize(args)}");

        Log.Logger.Information($"{processId}: Attempting to load context");

        var context = await AgentContext.LoadContext(default);

        Log.Logger.Information($"{processId}: Building host");

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                builder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@"agent.appsettings.json");
            }
        )
        .ConfigureServices((builder, services) =>
        {
            services.Configure<HostOptions>(options =>
            {
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
            });
            services.AddTransient(typeof(IHostedService), context.injectType);
            services.AddSingleton<IAgentContext, AgentContext>(x => context);
            services.AddSingleton(x => context);
            services.AddHostedService<AgentHelper>();
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
            loggerConfiguration.Enrich.WithProperty("AgentId", context.agentId);
        })
        .Build();

        //todo:
        //https://stackoverflow.com/questions/50486481/is-there-a-way-to-enrich-the-log-with-a-property-from-configuration

        try
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Failed running host.RunAsync");
            throw;
        }
    }
}