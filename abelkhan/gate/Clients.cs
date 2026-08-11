using core;
using engine;
using consts;

namespace gate;

public class Client(string connId, INetwork clientNetwork, RedisHandle redis)
{
    public string ConnId => connId;

    private readonly Dictionary<string, INetwork> _dictEntityNetwork = new();

    public void RegisterNetwork(string entity, INetwork network)
    {
        _dictEntityNetwork.Add(entity, network);
    }
    
    public async Task SendToClient(byte[] message)
    {
        await clientNetwork.Send(message);
    }

    public async Task SendToServer(string entity, byte[] message)
    {
        if (_dictEntityNetwork.TryGetValue(entity, out var network))
        {
            await network.Send(message);
        }
        else
        {
            await redis.PushList(string.Format(Consts.EntityServerMq, entity), message);
        }
    }
}