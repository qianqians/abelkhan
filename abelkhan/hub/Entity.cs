using core;
using Google.Protobuf;

namespace hub;

public abstract class Entity(string entityId, string entityType, RedisHandle redis) : BaseEntity(entityId, entityType, redis)
{
    public abstract override IMessage FullInfo();
    public abstract override IMessage ClientInfo();
    
    public new async Task CreateRemoteEntity(Client client)
    {
        await base.CreateRemoteEntity(client);
    }
}