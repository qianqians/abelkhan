using Consul;
using core;

namespace hub;

public class ConsulServiceWatcher(IConsulClient consulClient)
{
    private ulong _catalogLastIndex = 0;
    
    private readonly Dictionary<string, HashSet<string>> _knownServiceInstances = new();

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var queryOptions = new QueryOptions
                {
                    WaitIndex = _catalogLastIndex,
                    WaitTime = TimeSpan.FromMinutes(5)
                };

                var catalogResult = await consulClient.Catalog.Services(queryOptions, stoppingToken);
                _catalogLastIndex = catalogResult.LastIndex;

                var currentServices = catalogResult.Response.Keys;

                foreach (var serviceName in currentServices)
                {
                    if (serviceName.Equals("gate", StringComparison.OrdinalIgnoreCase) ||
                        serviceName.Equals("db_proxy", StringComparison.OrdinalIgnoreCase))
                    {
                        await CheckServiceInstancesAsync(serviceName, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                Log.Error($"ExecuteAsync Error:{ex}!");
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }
    }

    private async Task CheckServiceInstancesAsync(string serviceName, CancellationToken ct)
    {
        var healthResult = await consulClient.Health.Service(serviceName, tag: null, passingOnly: true, ct);
        var currentInstances = healthResult.Response;

        if (!_knownServiceInstances.TryGetValue(serviceName, out var knownIds))
        {
            knownIds = new HashSet<string>();
            _knownServiceInstances[serviceName] = knownIds;
        }

        var currentIds = new HashSet<string>();

        foreach (var entry in currentInstances)
        {
            var service = entry.Service;
            currentIds.Add(service.ID);

            if (!knownIds.Contains(service.ID))
            {
                OnNewServerRegistered(serviceName, service);
            }
        }

        _knownServiceInstances[serviceName] = currentIds;
    }

    public event Action<string, string, string, ushort>? OnNewService;
    private void OnNewServerRegistered(string serviceName, AgentService service)
    {
        OnNewService?.Invoke(serviceName, service.ID, service.Address, (ushort)service.Port);
        Log.Info($"[New Server Registered] ServiceName:{serviceName}, InstanceID:{service.ID}, Address:{service.Address}:{service.Port}");
    }
}