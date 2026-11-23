using core;
using consts;
using System.Collections.Concurrent;
using Google.Protobuf;

namespace hub
{
    internal class HubGlobalEvent(RedisHandle _redis)
    {
        private readonly ConcurrentDictionary<string, BaseEntity> entitis = new();

        public void OnEntityCreate(BaseEntity entity)
        {
            entitis[entity.EntityId] = entity;
        }

        public void OnEntityDestroy(BaseEntity entity)
        {
            entitis.TryRemove(entity.EntityId, out _);
        }

        public void OnEntityEvent(string entity, CallRpc event_)
        {
            if (entitis.TryGetValue(entity, out var _entity))
            {
                try
                {
                    _entity.PostTask(async () =>
                    {
                        await _entity.OnDoMsg(string.Empty, string.Empty, null, event_.ProtoName, event_.Content);
                    });
                }
                catch (Exception ex)
                {
                    Log.Error($"OnClientRequestHub Entity:{entity} failed! {ex}");
                }
            }
            else
            {
                Log.Error($"OnClientRequestHub Entity:{entity} not found!");
            }
        }

        public async void CallEntityEvent(string hubId, string entity, CallRpc event_)
        {
            await _redis.PushList(string.Format(Consts.HubHubMq, hubId), new HubHubNotifyEntity()
            {
                EntityId = entity,
                Event = event_,
            }.ToByteArray());
        }
    }
}
