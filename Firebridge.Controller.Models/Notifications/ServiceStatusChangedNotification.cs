using MediatR;

namespace Firebridge.Controller.Models.Notifications;

public class ServiceStatusChangedNotification : INotification
{
    public IServiceConnection ServiceConnection { get; }

    public ServiceConnectionState ConnectionState { get; }

    public ServiceStatusChangedNotification(IServiceConnection serviceConnection, ServiceConnectionState connectionState)
    {
        ServiceConnection = serviceConnection;
        ConnectionState = connectionState;
    }
}
