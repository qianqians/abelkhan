using core;
using Google.Protobuf;

namespace hub;

public abstract class Player(string entityId, string entityType, RedisHandle redis) : BaseEntity(entityId, entityType, redis)
{
    public abstract override IMessage FullInfo();
    public abstract override IMessage ClientInfo();

    public new async Task CreateRemotePlayer(Client client)
    {
        await base.CreateRemotePlayer(client);
    }
}