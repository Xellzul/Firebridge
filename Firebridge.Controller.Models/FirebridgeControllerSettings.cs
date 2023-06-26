namespace Firebridge.Controller.Models;

public class FirebridgeControllerSettings
{
    public int DiscoveryServerPort { get; set; }

    public int DiscoveryDelay { get; set; }

    public int AgentServerPort { get; set; }

    public string DiscoveryServerSecret { get; set; } = null!;
}
