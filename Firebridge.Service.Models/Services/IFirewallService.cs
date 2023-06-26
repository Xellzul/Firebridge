namespace Firebridge.Service.Models.Services;

public interface IFirewallService
{
    /// <summary>
    /// Creates firewall exception for this app
    /// </summary>
    public void AddFirewallException(string name);
}
