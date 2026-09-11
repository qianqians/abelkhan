using System.Collections.Concurrent;
using core;
using Google.Protobuf;
using MongoDB.Bson;

namespace hub;

public abstract class Player(
    string entityId,
    string entityType,
    string userId,
    RedisHandle redis,
    MongoProxy mongo,
    ConcurrentDictionary<string, Client> clients,
    ConcurrentDictionary<string, GateNetwork> gates) : BaseEntity(entityId, entityType, redis, clients, gates)
{
    public string UserId { get; } = userId;

    private readonly RedisHandle _redis = redis;

    private ConcurrentDictionary<string, Client> _clients = clients;
    private ConcurrentDictionary<string, GateNetwork> _gates = gates;

    protected abstract string MongoDbName();
    protected abstract string MongoCollectionName();
    
    public abstract override BsonDocument FullInfo();
    public abstract override BsonDocument ClientInfo();

    public async void Save()
    {
        try
        {
            var query = new BsonDocument()
            {
                { "EntityType", EntityType },
                { "EntityId", EntityId },
            };
            if (!await mongo.Update(MongoDbName(), MongoCollectionName(), query.ToBson(), FullInfo().ToBson(), true))
            {
                Log.Error($"Player entity:{EntityType} id:{EntityId} user:{UserId} save failed!");
            }
        }
        catch (Exception e)
        {
            Log.Error($"Player entity:{EntityType} id:{EntityId} user:{UserId} save error!", e);
        }
    }

    public new async Task CreateRemotePlayer(Client client)
    {
        await base.CreateRemotePlayer(client);
    }
}