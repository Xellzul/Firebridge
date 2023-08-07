using Firebridge.Controller;
using Firebridge.Controller.Models;
using Microsoft.Extensions.Logging;
using OverridePlugin;

namespace OverridePluginMaui;

[ControllerPlugin(ScopeType = ScopeType.SingletonService, Implements = typeof(OverridePluginContext))]
public class OverridePluginContext
{
    private const string OverrideSettings = "Override Settings";

    public Guid owner = Guid.NewGuid();
    public Guid agent = Guid.NewGuid();

    public OverridePluginContext(ILogger<OverridePluginContext> logger)
    {
        logger.LogError("CTOR{0}", owner);
    }
}