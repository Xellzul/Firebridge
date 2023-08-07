using Microsoft.Extensions.Logging;
using System.Runtime.Versioning;
using NetFwTypeLib;
using Firebridge.Service.Models.Services;

namespace Firebridge.Service.Win;

[SupportedOSPlatform("windows")]
internal class FirewallService : IFirewallService
{
    private readonly ILogger<FirewallService> logger;
    public FirewallService(ILogger<FirewallService> logger)
    {
        this.logger = logger;
    }

    public void AddFirewallException(string name)
    {
        logger.LogInformation("Adding firewall rule {0}", name);

        var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")!)!;

        foreach (INetFwRule rule in firewallPolicy.Rules)
        {
            if (rule.Name == name)
                return;
        }

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