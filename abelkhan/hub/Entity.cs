using System.Collections.Concurrent;
using core;
using Google.Protobuf;
using MongoDB.Bson;

namespace hub;

public abstract class Entity(
    string entityId,
    string entityType,
    RedisHandle redis,
    ConcurrentDictionary<string, Client> clients,
    ConcurrentDictionary<string, GateNetwork> gates) :
    BaseEntity(entityId, entityType, redis, clients, gates)
{
    public abstract override BsonDocument FullInfo();
    public abstract override BsonDocument ClientInfo();

    public new async Task CreateRemoteEntity(Client client)
    {
        await base.CreateRemoteEntity(client);
    }
}