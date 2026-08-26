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
                throw new ArgumentException($"EchoQueryServiceEntity CreateEntity err {gateName}_{cliConnId}_{info}");
        }
        return e;
    }

    public virtual async Task<List<BaseEntity>?> EchoQueryServiceExt(List<(string, string, byte[])> infoData)
    {
        try
        {
            var lEntities = new List<BaseEntity>();
            foreach (var i in infoData)
            {
                var (gateName, cliConnId, info) = i;
                lEntities.Add(CreateEntity(gateName, cliConnId, info));
            }

            foreach (var e in lEntities)
            {
                switch (e)
                {
                    case Entity entity:
                        await _group.CreateRemoteEntity(entity);
                        break;
                    case Player player:
                        await _group.CreateRemotePlayer(player);
                        break;
                    default:
                        throw new ArgumentException($"EchoQueryServiceExt err:{e}");
                } 
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