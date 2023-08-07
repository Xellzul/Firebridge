using MediatR;

namespace Firebridge.Controller.Models.Notifications;

public sealed class ControllerGlobalActionNotification : INotification
{
    public required string Action { get; set; }
}