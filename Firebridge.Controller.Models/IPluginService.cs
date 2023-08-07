namespace Firebridge.Controller.Models;

public interface IPluginService
{
    public ICollection<string> GetActions();

    public ICollection<string> GetGlobalActions();
}
