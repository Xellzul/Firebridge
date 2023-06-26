using Firebridge.Service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Hosting;
using Firebridge.Service.Models.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Firebridge.Service;

public class ControllerService : BackgroundService
{
    private readonly IOptions<FirebridgeServiceSettings> configuration;
    private readonly ILogger<ControllerService> logger;
    private readonly IFirewallService firewallService;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public BlockingCollection<IControllerConnection> ControllerConnections { get; init; } = new BlockingCollection<IControllerConnection>();
    //    private readonly IHostApplicationLifetime hostApplicationLifetime;

    //public int Port => ((IPEndPoint)Listener.LocalEndpoint).Port;

    public ControllerService(IOptions<FirebridgeServiceSettings> configuration, ILogger<ControllerService> logger, IFirewallService firewallService, IServiceScopeFactory serviceScopeFactory)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.firewallService = firewallService;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Controller Service Starting");

        var port = configuration.Value.AgentServerPort;

        if (port > ushort.MaxValue || port < 0)
            throw new ArgumentOutOfRangeException(nameof(port));

        firewallService.AddFirewallException(configuration.Value.AppName);

        TcpListener Listener = new TcpListener(IPAddress.IPv6Any, port);
        Listener.Server.DualMode = true;
        Listener.Start();
        logger.LogInformation($"Bound to {Listener.LocalEndpoint}");

        while (!stoppingToken.IsCancellationRequested && Listener.Server.IsBound)
        {
            try
            {
                TcpClient client = await Listener.AcceptTcpClientAsync(stoppingToken);

                if (client == null)
                    continue;

                logger.LogInformation($"Client connecte: {client.Client.RemoteEndPoint}");

                new Task(async () =>
                {
                    using IServiceScope scope = serviceScopeFactory.CreateScope();
                    var controllerConnection = scope.ServiceProvider.GetRequiredService<IControllerConnection>();

                    ControllerConnections.Add(controllerConnection);

                    try
                    {
                        await controllerConnection.Connect(client);

                        await controllerConnection.ExecuteAsync();
                    }
                    catch (IOException e) when (e.InnerException is SocketException)
                    {
                        if (((SocketException)e.InnerException).SocketErrorCode == SocketError.ConnectionReset)
                        {
                            logger.LogInformation($"Forcibly dissconnected: {client.Client.RemoteEndPoint}");
                        }
                        else
                        {
                            logger.LogCritical(e, "TODO HERE");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "controllerConnection.ExecuteAsync");
                    }

                    if (!ControllerConnections.TryTake(out controllerConnection))
                        throw new Exception("Cant take out controller");

                }, TaskCreationOptions.LongRunning).Start();
            }
            catch (OperationCanceledException e)
            {
                // this happens on cancelation token trigger
                logger.LogInformation(e, "OperationCanceledException in Controller Service");
                break;
            }

        }

        logger.LogInformation("Controller Service Stopping");
        Listener.Stop();
    }
}
