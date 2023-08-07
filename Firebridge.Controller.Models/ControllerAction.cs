namespace Firebridge.Controller.Models;

public interface IControllerAction
{
    public abstract static ICollection<string> LoadAction();

    public abstract static ICollection<string> LoadGlobalAction();
}
