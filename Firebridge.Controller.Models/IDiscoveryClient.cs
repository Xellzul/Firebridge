using System.Net;

namespace Firebridge.Controller.Models;

public interface IDiscoveryClient
{
    public void StartDiscovering();

    public void StopDiscovering();
}
