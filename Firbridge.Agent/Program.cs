using Firebridge.Common;
using Firebridge.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace Firbridge.Agent;

public class Program
{
    public static async Task Main(string[] args)
    {
        // redirect standard IO
        var sin = Console.OpenStandardInput();
        var sout = Console.OpenStandardOutput();
        var serr = Console.OpenStandardError();

        var msin = new MemoryStream();
        var msout = new MemoryStream();

        Console.SetIn(new StreamReader(msin));
        Console.SetOut(new StreamWriter(msout));
        // TODO ^^

        var initPacket = await StreamSerializer.RecieveAsync(sin);
        var init = initPacket as Packet<StartProgramModel>;
        if(init == null)
        {
            throw new InvalidProgramException($"Init packet is wrong type {init?.GetType()}");
        }

        // Load assemblies
        foreach(var assembly in init.Payload.Assemblies)
        {
            /*var asm = */AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(assembly));
        }

        var asd = Type.GetType(init.Payload.Type);

        //Assembly asm = null;
        ////todo: try catch
        //if (spm.Assemblies != null && spm.Assemblies.Length > 0)
        //else
        //    asm = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.ManifestModule.Name == "FireBridgeCore.dll").First();

        //var type = asm.GetType(spm.Type);
        //UserProgram instance = Activator.CreateInstance(type) as UserProgram;


        //object args = null;
        //if (spm.StartParameters != null && spm.StartParameters.Length > 0)
        //{
        //    var ms = new MemoryStream(spm.StartParameters, false);
        //    args = new BinaryFormatter().Deserialize(ms);
        //}

        //_userProgramContainer.StartAsync(instance, args, _connection, spm.ProcessId, e.Message.From);





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
            services.AddTransient(typeof(IHostedService), asd);
            services.AddSingleton<IAgentContext, AgentContext>();
            //ConfigureWinServices(builder, services);
            //Startup.ConfigureServices(builder, services);
        })
        .UseSerilog((hostingContext, loggerConfiguration) => { loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration); })
        .Build();

        //https://stackoverflow.com/questions/50486481/is-there-a-way-to-enrich-the-log-with-a-property-from-configuration

        await host.RunAsync();
    }
}

public interface IAgentContext
{

}

public class AgentContext : IAgentContext
{

}