using consts;
using engine;
using Google.Protobuf;

namespace hub;

public class Group()
{
    private readonly WRpc _rpc = new();
    private readonly List<Client> _clients = new();
    private readonly List<Entity> _entities = new();
    private readonly List<Player> _players = new();

    public async Task Join(Client client)
    {
        foreach (var e in _entities)
        {
            await e.CreateRemoteEntity(client);
        }

        foreach (var p in _players)
        {
            await p.CreateRemotePlayer(client);
        }
        
        _clients.Add(client);
    }

    public void Leave(Client client)
    {
        _clients.Remove(client);
    }

    public async Task CreateRemoteEntity(Entity entity)
    {
        foreach (var cli in _clients)
        {
            await entity.CreateRemoteEntity(cli);
        }
        _entities.Add(entity);
    }

    public async Task RemoveEntity(Entity entity)
    {
        foreach (var cli in _clients)
        {
            await entity.DeleteRemoteEntity(cli);
        }
        _entities.Remove(entity);
    }

    public async Task CreateRemotePlayer(Player player)
    {
        foreach (var cli in _clients)
        {
            await player.CreateRemotePlayer(cli);
        }
        _players.Add(player);
    }

    public async Task RemovePlayer(Player player)
    {
        foreach (var cli in _clients)
        {
            await player.DeleteRemoteEntity(cli);
        }
        _players.Remove(player);
    }

    public async Task Notify<T>(BaseEntity entity, string method, T argv)
        where T : IMessage<T>
    {
        var callRpc = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString()
        };
        foreach (var cli in _clients)
        {
            var msg = new GateForwardHubNotifyClient()
            {
                ConnId = cli.ConnId,
                EntityId = entity.EntityId,
                Event = callRpc,
            };
            await entity.SendToGate(cli.UserId, _rpc.Notify(Consts.GateForwardHubNotifyClient, msg));
        }
    }
}