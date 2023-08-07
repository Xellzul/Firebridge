using Firebridge.Agent;
using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Packets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Runtime.Loader;
using System.Text.Json;

namespace Firbridge.Agent;

public class Program
{
    public static async Task Main(string[] args)
    {
        // redirect standard IO
        var sin = Console.OpenStandardInput();
        var sout = Console.OpenStandardOutput();
        var serr = Console.OpenStandardError();

        //Create new IO
        //TODO: Actualy read this
        var msin = new MemoryStream();
        var msout = new MemoryStream();
        var mserr = new MemoryStream();

        Console.SetIn(new StreamReader(msin));
        Console.SetOut(new StreamWriter(msout));
        Console.SetError(new StreamWriter(mserr));

        var processId = Process.GetCurrentProcess().Id;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.File("Logs/AgentStartup.log", shared: true)
            .CreateBootstrapLogger();

        Log.Logger.Information($"{processId}: Starting Agent with Proccess ID: '{processId}' and args {JsonSerializer.Serialize(args)}");

        Log.Logger.Information($"{processId}: Attempting to read Init Packet");
        var initPacket = await StreamSerializer.RecieveAsync(sin);

        Log.Logger.Information($"{processId}: Got init Packet: {initPacket.GetType()}");

        var init = initPacket.Payload as StartProgramModelPacket;
        if (init == null)
        {
            throw new InvalidProgramException($"Init packet is wrong type {init?.GetType()}");
        }

        Log.Logger.Information($"{processId}: Got init Packet: {JsonSerializer.Serialize(
            new { initPacket.Sender, init.SessionId, init.Type, init.AgentGuid, init.ControllerGuid, init.OwnerGuid, assemblyCount = init.Assemblies.Count })}");

        var AgentContext = new AgentContext(sin, sout, serr, msin, msout, init.AgentGuid, init.ControllerGuid, init.OwnerGuid);

        Log.Logger.Information($"{processId}: Creating Agent Context ({init.AgentGuid})");

        // Load assemblies
        foreach (var assembly in init.Assemblies)
        {
            using var ms = new MemoryStream(assembly);
            AssemblyLoadContext.Default.LoadFromStream(ms);
        }

        Log.Logger.Information($"{processId}: Loaded all assemblies");

        var injectType = Type.GetType(init.Type);

        Log.Logger.Information($"Startup class: {injectType}");

        if(injectType == null)
        {
            Log.Logger.Error($"{processId}: {nameof(injectType)} is null");
            throw new InvalidProgramException($"{nameof(injectType)} is null");
        }

        Log.Logger.Information($"{processId}: Building host");

        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                builder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("agent.appsettings.json");
            }
        )
        .ConfigureServices((builder, services) =>
        {
            services.Configure<HostOptions>(options =>
            {
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
            });
            services.AddTransient(typeof(IHostedService), injectType);
            services.AddSingleton<IAgentContext, AgentContext>(x => AgentContext);
            services.AddSingleton(x => AgentContext);
            services.AddHostedService<AgentHelper>();
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
            loggerConfiguration.Enrich.WithProperty("AgentId", init.AgentGuid);
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