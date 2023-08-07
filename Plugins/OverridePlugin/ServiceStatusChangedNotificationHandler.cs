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

        logger.LogError("CTOR {owner}", overridePluginContext.agent);
    }

    public async Task Handle(AgentMessageNotification notification, CancellationToken cancellationToken)
    {
        logger.LogError("Handle {owner}", overridePluginContext.agent);
    }
}
