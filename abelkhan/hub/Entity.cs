using core;
using Google.Protobuf;

namespace hub;

public class Entity : Actor
{
    private readonly Dictionary<string, Func<string, ByteString, Task>> _onMsg = new();
    
    public Entity()
    {
    }

    private void Response(string connId, byte[] data)
    {
        
    }

    public void Register<T>(string method, Action<T> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onMsg.Add(method, (string connId, ByteString data) =>
        {
            try
            {
                var t = parser.ParseFrom(data);
                callback(t);
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
            return Task.CompletedTask;
        });
    }
    
    public void Register<T>(string method, Func<T, Task> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onMsg.Add(method, async (string connId, ByteString data) =>
        {
            try
            {
                var t = parser.ParseFrom(data);
                await callback(t);
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
        });
    }

    public void Register<T0, T1>(string method, Func<T0, T1> callback)
        where T0 : IMessage<T0>, new()
        where T1 : IMessage<T1>, new()
    {
        var parser = new MessageParser<T0>(() => new T0());
        _onMsg.Add(method, (string connId, ByteString data) =>
        {
            try
            {
                var t = parser.ParseFrom(data);
                var ret = callback(t);
                Response(connId, ret.ToByteArray());
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
            return Task.CompletedTask;
        });
    }
    
    public void Register<T0, T1>(string method, Func<T0, Task<T1>> callback)
        where T0 : IMessage<T0>, new()
        where T1 : IMessage<T1>, new()
    {
        var parser = new MessageParser<T0>(() => new T0());
        _onMsg.Add(method, async (string connId, ByteString data) =>
        {
            try
            {
                var t = parser.ParseFrom(data);
                var ret = await callback(t);
                Response(connId, ret.ToByteArray());
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
        });
    }
    
    public void OnDoMsg(string connId, string method, ByteString message)
    {
        if (_onMsg.TryGetValue(method, out var action))
        {
            try
            {
                action(connId, message);
            }
            catch(Exception ex)
            {
                Log.Error($"OnRequest method:{method} ex:{ex}");
            }
        }
        else
        {
            Log.Error($"OnRequest method:{method} not exist");
        }
    }
}