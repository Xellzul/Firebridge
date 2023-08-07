using Firebridge.Controller.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;
using System.Runtime.Loader;

namespace Firebridge.Controller.Common;

public class PluginService : IPluginService
{
    public List<string> Actions { get; init; } = new List<string>();
    public List<string> GlobalActions { get; init; } = new List<string>();

    public ICollection<string> GetActions()
    {
        return Actions;
    }

    public ICollection<string> GetGlobalActions()
    {
        return GlobalActions;
    }

    private void addGlobalAction(string action)
    {
        GlobalActions.Add(action);
    }

    private void addAction(string action)
    {
        Actions.Add(action);
    }

    private void AddType(IServiceCollection services, Type service, ScopeType ServiceType, Type implements)
    {
        //TODO: this
        switch (ServiceType)
        {
            case ScopeType.HostedService:
                services.AddSingleton(typeof(IHostedService), service);
                break;
            case ScopeType.TransientService:
                services.AddTransient(implements, service);
                break;
            case ScopeType.ScopedService:
                services.AddScoped(implements, service);
                break;
            case ScopeType.SingletonService:
                services.AddSingleton(implements, service);
                break;
            default:
                throw new NotImplementedException("Not implemented");
        }
    }

    public static PluginService LoadPlugins(string folder, IServiceCollection services)
    {
        var pluginService = new PluginService();
        var pluginFolderFullPath = Path.Combine(AppContext.BaseDirectory, folder);

        if (!Directory.Exists(pluginFolderFullPath))
        {
            Log.Logger.Warning($"Loading folder {pluginFolderFullPath} - Folder not exists");
            return pluginService;
        }

        Log.Logger.Information($"Loading folder {pluginFolderFullPath}");

        foreach (var pluginDirectory in Directory.GetDirectories(pluginFolderFullPath))
        {
            var pluginNameDirectory = Path.GetFileName(pluginDirectory);
            var pluginDllPath = Path.Combine(pluginDirectory, pluginNameDirectory + ".dll");

            Log.Logger.Information($"Loading {pluginNameDirectory} from {pluginDllPath}");

            try
            {
                var context = new AssemblyLoadContext(pluginDllPath);
                var resolver = new AssemblyDependencyResolver(pluginDllPath);

                ArgumentNullException.ThrowIfNull(context);
                ArgumentNullException.ThrowIfNull(resolver);

                context.Resolving += (AssemblyLoadContext context, AssemblyName assemblyName) =>
                {
                    string assemblyPath = resolver!.ResolveAssemblyToPath(assemblyName);
                    if (assemblyPath != null)
                    {
                        return context.LoadFromAssemblyPath(assemblyPath);
                    }

                    return null;
                };

                var asm = context.LoadFromAssemblyPath(pluginDllPath);

                foreach (var assembly in asm.GetReferencedAssemblies())
                {
                    LoadWithDependencies(assembly, context);
                }

                Log.Logger.Information($"{pluginNameDirectory}'s assembly loaded: {asm.FullName}");

                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(asm));

                foreach (Type type in asm.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(ControllerPluginAttribute), false);
                    foreach (var attribute in attributes)
                    {
                        var casted = attribute as ControllerPluginAttribute;
                        if (casted == null)
                        {
                            Log.Logger.Error($"Attribute is null in {pluginNameDirectory}, {attribute.GetType()}");
                            continue;
                        }

                        Log.Logger.Information($"Loading from {pluginNameDirectory} - {type}, {casted.ScopeType}, {casted.Implements}");
                        pluginService.AddType(services, type, casted.ScopeType, casted.Implements);
                    }
                    
                    if (typeof(IControllerAction).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        var method = type.GetMethod(nameof(IControllerAction.LoadAction), BindingFlags.Public | BindingFlags.Static);
                        if (method == null)
                        {
                            Log.Logger.Error($"Method is null {method}");
                        }
                        else
                        {
                            var actions = method.Invoke(null, null);

                            if(actions is not ICollection<string>)
                            {
                                Log.Logger.Error($"Method is not string {actions?.GetType()}");
                            }
                            else
                            {
                                foreach (var action in (ICollection<string>)actions)
                                {
                                    pluginService.addAction(action);
                                }
                            }
                        }

                        method = type.GetMethod(nameof(IControllerAction.LoadGlobalAction), BindingFlags.Public | BindingFlags.Static);
                        if (method == null)
                        {
                            Log.Logger.Error($"Method is null {method}");
                        }
                        else
                        {
                            var actions = method.Invoke(null, null);

                            if (actions is not ICollection<string>)
                            {
                                Log.Logger.Error($"Method is not string {actions?.GetType()}");
                            }
                            else
                            {
                                foreach (var action in (ICollection<string>)actions)
                                {
                                    pluginService.addGlobalAction(action);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Error loading {pluginNameDirectory}");
            }
        }

        return pluginService;
    }

    private static void LoadWithDependencies(AssemblyName assembly, AssemblyLoadContext context)
    {
        var asm = context.LoadFromAssemblyName(assembly);
        foreach (var refAssembly in asm.GetReferencedAssemblies())
        {
            LoadWithDependencies(refAssembly, context);
        }
    }
}