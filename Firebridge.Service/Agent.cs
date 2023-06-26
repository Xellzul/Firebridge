using Firebridge.Common;
using Firebridge.Common.Models;
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

    public Guid ownerControllerId { get; private set; }

    private string? agentName;

    private Task? errorReader;

    public Agent(IApplicationLoader applicationLoader, ILogger<Agent> logger, IServiceContext serviceContext)
    {
        this.applicationLoader = applicationLoader;
        this.logger = logger;
        this.serviceContext = serviceContext;
    }

    public async Task ExecuteAsync(StartProgramModel startProgramModel, CancellationToken token = default)
    {
        agentId = startProgramModel.AgentGuid;
        agentName = startProgramModel.Type;

        var sessionId = startProgramModel.SessionId == StartProgramModel.ActiveSessionId ? applicationLoader.GetActiveSessionId() : startProgramModel.SessionId;
        var integrityLevel = IIntegrityLevel.Medium; // TODO

        logger.LogInformation($"Starting Agent {agentId}");

        (proccess, writeStream, errorStream, readStream) = applicationLoader.StartProcess(
            AppDomain.CurrentDomain.BaseDirectory + "Firbridge.Agent.exe",
            new[] { agentId.ToString() },
            integrityLevel);

        if (proccess == null || writeStream == null || readStream == null || errorStream == null)
            throw new InvalidProgramException($"Failed to start process");

        errorReader = ReadErrorOutput(token);

        await SendData(startProgramModel, agentId, token);

        while (readStream.CanRead && !token.IsCancellationRequested)
        {
            var data = await StreamSerializer.RecieveAsync(readStream, token);

            logger.LogTrace($"Recieved packet from {agentId}({agentName}) <{data.GetType()}>");

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
            logger.LogWarning($"Recieved error from {agentId}({agentName}) <{errorLine}>");
        }
    }

    public async Task SendData<T>(T data, Guid sender, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(writeStream);

        var pkt = new Packet<T>() { Payload = data, Sender = sender };
        await StreamSerializer.SendAsync(writeStream, pkt, token);
    }
}
