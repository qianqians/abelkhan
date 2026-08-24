using core;
using Google.Protobuf;
using Newtonsoft.Json.Linq;

namespace hub;

public abstract class Service
{
    private readonly Group _group = new();

    protected abstract BaseEntity CreateEntity(string entityId, string entityType, IMessage info);

    public virtual async Task EchoQueryServiceEntity(string gateName, string cliConnId, IMessage info)
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
    }

    public virtual async Task EchoQueryServiceExt(List<(string, string, IMessage)> infoData)
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
        }
        catch (Exception ex)
        {
            Log.Error($"EchoQueryServiceExt err:{ex}");
        }
    }
}