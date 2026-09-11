using System.Collections.Concurrent;
using core;

namespace hub;

public abstract class Service(ConcurrentDictionary<string, Client> clients)
{
    private readonly Group _group = new();

    protected abstract Player CreateEntity(string gateName, string cliConnId, byte[] info);

    public virtual async Task<Player> EchoQueryServiceEntity(string gateName, string cliConnId, byte[] info)
    {
        var player = CreateEntity(gateName, cliConnId, info);
        var cli = new Client(player.UserId, gateName, cliConnId);
        cli = clients.AddOrUpdate(player.UserId, cli, (key, old) => old);
        await player.CreateRemotePlayer(cli);
        await _group.CreateRemotePlayer(player);
        return player;
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