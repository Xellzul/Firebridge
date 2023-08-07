using Firebridge.Common;
using Firebridge.Common.Models;
using Firebridge.Common.Models.Packets;
using Firebridge.Service.Models.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Firebridge.Service;

public class Agent : IAgent
{
    private readonly IApplicationLoader applicationLoader;
    private readonly IServiceContext serviceContext;
    private readonly ILogger<Agent> logger;

    //Stream to Agent
    private Stream? writeStream;
    //Incoming stream
    private Stream? readStream;
    //Incoming error stream
    private Stream? errorStream;

    private Process? proccess;

    public Guid agentId { get; private set; }

    public Guid ownerProcessId { get; private set; }

    public Guid ownerControllerId { get; private set; }

    private string? agentName;

    private Task? errorReader;

    public Agent(IApplicationLoader applicationLoader, ILogger<Agent> logger, IServiceContext serviceContext)
    {
        this.applicationLoader = applicationLoader;
        this.logger = logger;
        this.serviceContext = serviceContext;
    }

    public async Task ExecuteAsync(StartProgramModelPacket startProgramModel, CancellationToken token = default)
    {
        agentName = startProgramModel.Type.Split('.').Last();

        var sessionId = startProgramModel.SessionId == StartProgramModelPacket.ActiveSessionId ? applicationLoader.GetActiveSessionId() : startProgramModel.SessionId;
        var integrityLevel = IIntegrityLevel.Medium; // TODO

        logger.LogInformation($"Starting Agent {agentId}");

        (proccess, writeStream, errorStream, readStream) = applicationLoader.StartProcess(
            AppDomain.CurrentDomain.BaseDirectory + "Firebridge.Agent.exe",
            new[] { agentId.ToString(), startProgramModel.OwnerGuid.ToString() },
            integrityLevel);

        if (proccess == null || writeStream == null || readStream == null || errorStream == null)
            throw new InvalidProgramException($"Failed to start process");

        errorReader = ReadErrorOutput(token);

        await SendData(startProgramModel, startProgramModel.ControllerGuid, startProgramModel.OwnerGuid, token);

        while (readStream.CanRead && !token.IsCancellationRequested)
        {
            var data = await StreamSerializer.RecieveAsync(readStream, token);

            logger.LogDebug("Agent>Service: {agentId}({agentName}) <{@packet}>", agentId, agentName, data);

            if (data.Payload != null)
                await ReturnData(data.Payload, token);
            // TODo HANDLE DATA
        }
    }

    private async Task ReadErrorOutput(CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(errorStream);

        using var sr = new StreamReader(errorStream);

        while (errorStream.CanRead && !token.IsCancellationRequested)
        {
            var errorLine = await sr.ReadLineAsync(token);

            if (errorLine == null)
                continue;

            logger.LogWarning($"Recieved error from {agentId}({agentName}) <{errorLine}>");
            await ReturnData(new AgentErrorPacket { Line = errorLine });
        }
    }

    public Task SendData<T>(T data, Guid sender, Guid senderProgram, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(data);

        var packet = new Packet() { Payload = data, Sender = sender, SenderProgram = senderProgram, Target = ownerControllerId, TargetProgram = agentId };

        return sendRawInternal(packet);
    }

    public Guid GetId() => agentId;

    public void PrepareAgent(StartProgramModelPacket startProgramModel)
    {
        agentId = startProgramModel.AgentGuid;
        ownerProcessId = startProgramModel.OwnerGuid;
        ownerControllerId = startProgramModel.ControllerGuid;
        logger.LogWarning("PREP::{agentId} {ownerProcessId} {ownerControllerId}", agentId, ownerProcessId, ownerControllerId);
    }

    private async Task ReturnData<T>(T data, CancellationToken cancellationToken = default)
    {
        var owner = serviceContext.GetController(ownerControllerId);

        ArgumentNullException.ThrowIfNull(owner);

        await owner.Send(data, ownerProcessId, cancellationToken);
    }

    public Task SendRaw(Packet data, CancellationToken token = default)
    {
        return sendRawInternal(data, token);
    }

    private async Task sendRawInternal(Packet packet, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(writeStream);

        logger.LogTrace($"Sending packet to Agent: {packet}");
        await StreamSerializer.SendAsync(writeStream, packet, token);
    }
}
