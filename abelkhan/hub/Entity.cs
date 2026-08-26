using System.Collections.Concurrent;
using core;
using Google.Protobuf;

namespace hub;

public abstract class Entity(
    string entityId,
    string entityType,
    RedisHandle redis,
    ConcurrentDictionary<string, Client> clients,
    ConcurrentDictionary<string, GateNetwork> gates) :
    BaseEntity(entityId, entityType, redis, clients, gates)
{
    public abstract override IMessage FullInfo();
    public abstract override IMessage ClientInfo();

    public new async Task CreateRemoteEntity(Client client)
    {
        await base.CreateRemoteEntity(client);
    }
}