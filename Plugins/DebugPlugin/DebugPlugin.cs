using Firebridge.Common.Models;
using Firebridge.Common.Models.Packets;
using Firebridge.Controller.Models;
using Firebridge.Controller.Models.Notifications;
using MediatR;

namespace DebugPlugin;

[ControllerPlugin(ScopeType = ScopeType.HostedService, Implements = typeof(DebugPlugin))]
public class DebugPlugin : INotificationHandler<ControllerActionNotification>, IControllerAction
{
    public const string DebugAgentAction = "Debug Agent";
    public const string DebugServiceAction = "Debug Service";

    public static ICollection<string> LoadAction()
    {
        return new[] { DebugAgentAction, DebugServiceAction };
    }

    public static ICollection<string> LoadGlobalAction()
    {
        return Array.Empty<string>();
    }

    public async Task Handle(ControllerActionNotification notification, CancellationToken cancellationToken)
    {
        switch (notification.Action)
        {
            case DebugAgentAction:
                await notification.Connection.StartAgent(typeof(DebugAgent));
                break;

            case DebugServiceAction:
                await notification.Connection.Send(new DebugPacket(), Guid.Empty, Guid.Empty, cancellationToken);
                break;
        }
    }
}