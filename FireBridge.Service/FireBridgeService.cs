using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetFwTypeLib;

namespace FireBridge.Service;

public class FireBridgeService : BackgroundService
{
    private readonly ILogger<FireBridgeService> _logger;
    private readonly ServiceServer _server;

    public FireBridgeService(ILogger<FireBridgeService> logger, ServiceServer server) =>
    (_logger, _server) = (logger, server);


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(StartAsync));
        _logger.LogInformation("Adding Firewall Exception");
        AddFirewallException("FireBridge");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(StopAsync));
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FireBridge Service Started");

        await _server.StartAccepting(stoppingToken);

        _logger.LogInformation("FireBridge Service Stopped");

        Environment.Exit(1);
    }

    private static void AddFirewallException(string name)
    {
        var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
            Type.GetTypeFromProgID("HNetCfg.FwPolicy2")!)!;

        if (firewallPolicy.Rules.Item(name) != null)
            return;

        var firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule")!)!;
        firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
        firewallRule.Description = name;
        firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
        firewallRule.Enabled = true;
        firewallRule.InterfaceTypes = "All";
        firewallRule.Name = name;
        firewallPolicy.Rules.Add(firewallRule);
    }
}
