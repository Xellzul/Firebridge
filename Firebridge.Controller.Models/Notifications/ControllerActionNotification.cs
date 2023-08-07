using MediatR;

namespace Firebridge.Controller.Models.Notifications;

public sealed class ControllerActionNotification : INotification
{
    public required string Action { get; set; }

    public required IServiceConnection Connection { get; set; }
}
