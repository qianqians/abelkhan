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
    private readonly object _lockObject = new();

    public async Task Join(Client client)
    {
        Entity[] entityCopy;
        Player[] playerCopy;
        lock (_lockObject)
        {
            entityCopy = _entities.ToArray();
            playerCopy = _players.ToArray();
            _clients.Add(client);
        }
        
        foreach (var e in entityCopy)
        {
            await e.CreateRemoteEntity(client);
        }

        foreach (var p in playerCopy)
        {
            await p.CreateRemotePlayer(client);
        }
    }

    public void Leave(Client client)
    {
        lock (_lockObject)
        {
            _clients.Remove(client);
        }
    }

    public async Task CreateRemoteEntity(Entity entity)
    {
        Client[] cliCopy;
        lock (_lockObject)
        {
            cliCopy = _clients.ToArray();
            _entities.Add(entity);
        }
        foreach (var cli in cliCopy)
        {
            await entity.CreateRemoteEntity(cli);
        }
    }

    public async Task RemoveEntity(Entity entity)
    {
        Client[] cliCopy;
        lock (_lockObject)
        {
            cliCopy = _clients.ToArray();
            _entities.Remove(entity);
        }
        foreach (var cli in cliCopy)
        {
            await entity.DeleteRemoteEntity(cli);
        }
    }

    public async Task CreateRemotePlayer(Player player)
    {
        Client[] cliCopy;
        lock (_lockObject)
        {
            cliCopy = _clients.ToArray();
            _players.Add(player);
        }
        foreach (var cli in cliCopy)
        {
            await player.CreateRemotePlayer(cli);
        }
    }

    public async Task RemovePlayer(Player player)
    {
        Client[] cliCopy;
        lock (_lockObject)
        {
            cliCopy = _clients.ToArray();
            _players.Remove(player);
        }
        foreach (var cli in cliCopy)
        {
            await player.DeleteRemoteEntity(cli);
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
        Client[] cliCopy;
        lock (_lockObject)
        {
            cliCopy = _clients.ToArray();
        }
        foreach (var cli in cliCopy)
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