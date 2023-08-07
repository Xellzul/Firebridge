using Firebridge.Controller.Models.Notifications;
using MediatR;

namespace Firebridge.Controller;
public class ServiceCallback : INotificationHandler<ServiceStatusChangedNotification>
{
    private readonly Dashboard dashboard;
    public ServiceCallback(Dashboard dashboard)
    {
        this.dashboard = dashboard;
    }

    public Task Handle(ServiceStatusChangedNotification notification, CancellationToken cancellationToken)
    {
        return dashboard.ConnectionChanged(notification, cancellationToken);
    }
}
