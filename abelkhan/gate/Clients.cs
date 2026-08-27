using core;
using engine;
using consts;

namespace gate;

public class Client(string connId, INetwork clientNetwork, RedisHandle redis)
{
    public string ConnId => connId;
    public string? UserId { set; get; }
    public long LastEventTime { set; get; } = TimerService.Tick;
    private readonly Dictionary<string, INetwork> _dictEntityNetwork = new();

    public void RegisterNetwork(string entity, INetwork network)
    {
        lock (_dictEntityNetwork)
        {
            _dictEntityNetwork[entity] = network;
        }
    }
    
    public async Task SendToClient(byte[] message)
    {
        await clientNetwork.Send(message);
    }

    public Task Close()
    {
        return clientNetwork.Close();
    }

    public async Task SendToServer(string entity, byte[] message)
    {
        INetwork? network;
        lock (_dictEntityNetwork)
        {
            _dictEntityNetwork.TryGetValue(entity, out network);
        }
        if (network != null)
        {
            await network.Send(message);
        }
        else
        {
            await redis.PushList(string.Format(Consts.EntityServerMq, entity), message);
        }
    }
}