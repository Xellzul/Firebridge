using System.Net;

namespace Firebridge.Controller.Models;

public interface IControllerContext
{
    ICollection<string> GetActions();

    ICollection<string> GetGlobalActions();

    ICollection<IServiceConnection> GetServices();

    Task TryConnectService(IPAddress address, int port, Guid id);
}