using Firebridge.Core.Networking;
using FireBridgeCore;
using Microsoft.Extensions.Options;

namespace FireBridge.Service;

public class ServiceServer
{
    private readonly AppSettings _settings;
    private readonly DiscoveryServer _discoveryServer;
    private readonly Guid MachineId = MachineFingerprintGenerator.GetFingerprint();

    public ServiceServer(DiscoveryServer DiscoveryServer, IOptions<AppSettings> Configuration)
    => (_settings, _discoveryServer) = (Configuration.Value, DiscoveryServer);

    internal async Task StartAccepting(CancellationToken stoppingToken)
    {


        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10);
        }
    }
}



/*
 
     public Guid Guid { get; private set; }
    private DiscoveryServer discoveryServer;

    public ConnectionsManager2(Guid id, int discoveryPort, int mainPort, string secret)
    {
        Guid = id;
        discoveryServer = new DiscoveryServer(id, secret, discoveryPort);
    }

    public async Task StartAccepting(CancellationToken cancelationToken)
    {
        _ = discoveryServer.RunAsync();

        while (!cancelationToken.IsCancellationRequested)
        {
            try
            {
            }
            catch (Exception)
            {

            }
        }
    }
 */