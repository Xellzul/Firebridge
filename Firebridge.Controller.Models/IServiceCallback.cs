namespace Firebridge.Controller.Models;

public interface IServiceCallback
{
    public void Connecting(IServiceConnection serviceConnection);

    public void Connected(IServiceConnection serviceConnection);

    public void Disconnected(IServiceConnection serviceConnection);
}
