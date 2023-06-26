namespace Firebridge.Service.Models;

public class FirebridgeServiceSettings
{
    public string AppName { get; set; } = null!;

    public string AppVersion { get; set; } = null!;

    public int DiscoveryServerPort { get; set; }

    public int AgentServerPort { get; set; }

    public string DiscoveryServerSecret { get; set; } = null!;
}
