using System.Net;

namespace Firebridge.Controller.Models;

public interface IControllerContext
{
    Task TryConnectService(IPAddress address, Guid id);
}