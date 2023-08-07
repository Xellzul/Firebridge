using Firebridge.Common.Models;
using MediatR;

namespace Firebridge.Controller.Models.Notifications;

public sealed class AgentStatusChangedNotification : INotification
{
    public Agent Agent { get; }

    public AgentState ConnectionState { get; }

    public AgentStatusChangedNotification(Agent agent, AgentState connectionState)
    {
        Agent = agent;
        ConnectionState = connectionState;
    }
}