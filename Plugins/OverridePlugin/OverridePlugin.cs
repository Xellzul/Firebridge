using Firbridge.Agent;
using Firebridge.Common;
using Firebridge.Common.Models.Packets;
using Firebridge.Controller;
using Firebridge.Controller.Models;
using Firebridge.Controller.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OverridePluginMaui;

[ControllerPlugin(ScopeType = ScopeType.HostedService, Implements = typeof(OverridePlugin))]
[ControllerPlugin(ScopeType = ScopeType.SingletonService, Implements = typeof(INotificationHandler<AgentMessageNotification>))]
public class OverridePlugin : INotificationHandler<ServiceStatusChangedNotification>, INotificationHandler<ControllerActionNotification>, INotificationHandler<AgentMessageNotification>, IControllerAction
{
    private const string OverrideSettings = "Override Settings";
    private readonly ServiceMiniView serviceView;
    private readonly OverridePluginContext overridePluginContext;

    public OverridePlugin(ServiceMiniView serviceView, OverridePluginContext overridePluginContext, ILogger<OverridePlugin> logger)
    {
        logger.LogWarning("OverridePlugin CTOOOORaaa {owner}", overridePluginContext.owner);
        this.serviceView = serviceView;
        this.overridePluginContext = overridePluginContext;
    }

    public static ICollection<string> LoadAction()
    {
        return Array.Empty<string>();
    }

    public static ICollection<string> LoadGlobalAction()
    {
        return new[] { OverrideSettings };
    }

    public async Task Handle(ServiceStatusChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.ConnectionState == ServiceConnectionState.Connected)
        {
            await notification.ServiceConnection.StartAgent(typeof(ImageCapturerAgent), overridePluginContext.owner, overridePluginContext.agent);
        }
    }

    public async Task Handle(ControllerActionNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Action == OverrideSettings)
        {

        }
    }

    public async Task Handle(AgentMessageNotification notification, CancellationToken cancellationToken)
    {
        if(notification.TargetProgram == overridePluginContext.owner)
        {
            var payload = notification.Payload as PluginDataPacket;
            if (payload == null)
                return;

            var a = StreamSerializer.Deserialize<byte[]>(payload.data);
            var b = ImageSource.FromStream(() => new MemoryStream(a));

            await serviceView.SetImage(b);
        }
    }
}