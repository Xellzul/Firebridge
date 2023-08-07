using Firebridge.Controller.Models;
using Firebridge.Controller.Models.Notifications;
using MediatR;

namespace IdentifyPlugin;

[ControllerPlugin(ScopeType = ScopeType.HostedService, Implements = typeof(IdentifyPlugin))]
public class IdentifyPlugin : INotificationHandler<ControllerActionNotification>, INotificationHandler<ControllerGlobalActionNotification>, IControllerAction
{
    private readonly IControllerContext controllerContext;

    public IdentifyPlugin(IControllerContext controllerContext)
    {
        this.controllerContext = controllerContext;
    }

    public static ICollection<string> LoadAction()
    {
        return new[] { ActionName };
    }

    public static ICollection<string> LoadGlobalAction()
    {
        return new[] { ActionName };
    }

    public static string ActionName { get; } = "Identify";

    public async Task Handle(ControllerActionNotification notification, CancellationToken cancellationToken = default)
    {
        if (notification.Action != ActionName)
            return;

        //sc.StartProgram(typeof(IdentifyProcess), IIntegrityLevel.Medium, UInt32.MaxValue, AssemblyData, null, null);
        await notification.Connection.StartAgent(typeof(IdentifyAgent));
    }

    public async Task Handle(ControllerGlobalActionNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Action != ActionName)
            return;

        var services = controllerContext.GetServices();

        foreach (var service in services)
        {
            await service.StartAgent(typeof(IdentifyAgent));
        }
    }
}