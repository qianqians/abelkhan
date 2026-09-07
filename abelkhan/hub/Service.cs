using System.Collections.Concurrent;
using core;
using Google.Protobuf;
using Newtonsoft.Json.Linq;

namespace hub;

public abstract class Service(
    ConcurrentDictionary<string, Client> clients,
    ConcurrentDictionary<string, GateNetwork> gates)
{
    private readonly Group _group = new();

    protected abstract BaseEntity CreateEntity(string gateName, string cliConnId, byte[] info);

    public virtual async Task<BaseEntity> EchoQueryServiceEntity(string gateName, string cliConnId, byte[] info)
    {
        var e = CreateEntity(gateName, cliConnId, info);
        switch (e)
        {
            case Entity entity:
                await _group.CreateRemoteEntity(entity);
                break;
            case Player player:
                await _group.CreateRemotePlayer(player);
                break;
            default:
                Log.Error($"EchoQueryServiceEntity CreateEntity err {gateName}_{cliConnId}_{info}");
                break;
        }
        return e;
    }

    public virtual async Task<List<BaseEntity>?> EchoQueryServiceExt(List<(string, string, byte[])> infoData)
    {
        try
        {
            var lEntities = new List<BaseEntity>();
            foreach (var (gateName, cliConnId, info) in infoData)
            {
                lEntities.Add(await EchoQueryServiceEntity(gateName, cliConnId, info));
            }
            return lEntities;
        }
        catch (Exception ex)
        {
            Log.Error($"EchoQueryServiceExt err:{ex}");
        }
        return null;
    }
}