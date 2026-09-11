using System.Collections.Concurrent;
using core;
using MongoDB.Bson;

namespace hub;

public abstract class GlobalEntity(
    string entityId,
    string entityType,
    RedisHandle redis,
    MongoProxy mongo,
    ConcurrentDictionary<string, Client> clients,
    ConcurrentDictionary<string, GateNetwork> gates) : BaseEntity(entityId, entityType, redis, clients, gates)
{
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
                Log.Error($"GlobalEntity entity:{EntityType} id:{EntityId} save failed!");
            }
        }
        catch (Exception e)
        {
            Log.Error($"GlobalEntity entity:{EntityType} id:{EntityId} save error!", e);
        }
    }

    public new async Task CreateRemoteEntity(Client client)
    {
        await base.CreateRemoteEntity(client);
    }
}