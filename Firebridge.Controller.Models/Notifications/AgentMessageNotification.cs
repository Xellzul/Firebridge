using Firebridge.Common.Models.Packets;
using MediatR;

namespace Firebridge.Controller.Models.Notifications;
public sealed class AgentMessageNotification : INotification
{
    public AgentMessageNotification(Packet packet, IServiceConnection senderConnection)
    {
        Sender = packet.Sender;
        Target = packet.Target;
        SenderProgram = packet.SenderProgram;
        TargetProgram = packet.TargetProgram;
        Payload = packet.Payload;
        SenderConnection = senderConnection;
    }

    public Guid Sender { get; set; }

    public Guid Target { get; set; }

    public Guid SenderProgram { get; set; }

    public Guid TargetProgram { get; set; }

    public object Payload { get; set; }

    public IServiceConnection SenderConnection { get; set; }
}