using Firebridge.Common.Models.Packets;

namespace Firebridge.Service.Models.Services;

public interface IAgent
{
    public Task ExecuteAsync(StartProgramModelPacket startProgramModel, CancellationToken token = default);

    public Guid GetId();

    void PrepareAgent(StartProgramModelPacket startProgramModel);

    public Task SendData<T>(T data, Guid sender, Guid senderProgram, CancellationToken token = default);

    public Task SendRaw(Packet data, CancellationToken token = default);
}

