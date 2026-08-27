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
    private readonly Nito.AsyncEx.AsyncLock _lockObject = new();

    public async Task Join(Client client)
    {
        using (await _lockObject.LockAsync())
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
    }

    public async Task Leave(Client client)
    {
        using (await _lockObject.LockAsync())
        {
            _clients.Remove(client);
        }
    }

    public async Task CreateRemoteEntity(Entity entity)
    {
        using (await _lockObject.LockAsync())
        {
            foreach (var cli in _clients)
            {
                await entity.CreateRemoteEntity(cli);
            }
            _entities.Add(entity);
        }
    }

    public async Task RemoveEntity(Entity entity)
    {
        using (await _lockObject.LockAsync())
        {
            foreach (var cli in _clients)
            {
                await entity.DeleteRemoteEntity(cli);
            }
            _entities.Remove(entity);
        }
    }

    public async Task CreateRemotePlayer(Player player)
    {
        using (await _lockObject.LockAsync())
        {
            foreach (var cli in _clients)
            {
                await player.CreateRemotePlayer(cli);
            }
            _players.Add(player);
        }
    }

    public async Task RemovePlayer(Player player)
    {
        using (await _lockObject.LockAsync())
        {
            foreach (var cli in _clients)
            {
                await player.DeleteRemoteEntity(cli);
            }
            _players.Remove(player);
        }
    }

    public async Task Notify<T>(BaseEntity entity, string method, T argv)
        where T : IMessage<T>
    {
        var callRpc = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString()
        };
        using (await _lockObject.LockAsync())
        {
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
}