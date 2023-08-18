using Firebridge.Controller.Models.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using OverridePluginMaui;

namespace OverridePlugin;
internal class ServiceStatusChangedNotificationHandler : INotificationHandler<AgentMessageNotification>
{
    private readonly OverridePluginContext overridePluginContext;
    private readonly ILogger<ServiceStatusChangedNotificationHandler> logger;

    public ServiceStatusChangedNotificationHandler(OverridePluginContext overridePluginContext, ILogger<ServiceStatusChangedNotificationHandler> logger)
    {
        this.overridePluginContext = overridePluginContext;
        this.logger = logger;
    }

    public async Task Handle(AgentMessageNotification notification, CancellationToken cancellationToken)
    {
    }
}
