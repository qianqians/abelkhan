using Google.Protobuf;
namespace engine;

public class WRpc
{
    public byte[] Notify<T>(string method, T argv) where T : IMessage<T>, new()
    {
        var call = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString(),
        };
        var ntf = new Notify()
        {
            Event = call,
        };
        var msg = new Msg()
        {
            Notify = ntf,
        };
        return msg.ToByteArray();
    }

    public byte[] Request<T>(string method, string msgId, T argv) where T : IMessage<T>, new()
    {
        var call = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString(),
        };
        var req = new Request()
        {
            MsgId = msgId,
            Event = call,
        };
        var msg = new Msg()
        {
            Req = req,
        };
        return msg.ToByteArray();
    }

    public byte[] Response<T>(string method, string msgId, T argv) where T : IMessage<T>, new()
    {
        var call = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString(),
        };
        var rsp = new Response()
        {
            MsgId = msgId,
            Event = call,
        };
        var msg = new Msg()
        {
            Rsp = rsp,
        };
        return msg.ToByteArray();
    }

    public event Action<Request>? OnRequest;
    public event Action<Response>? OnResponse;
    public event Action<Notify>? OnNotify;

    public void OnNetworkData(byte[] data)
    {
        var parser = new MessageParser<Msg>(() => new Msg());
        var msg = parser.ParseFrom(data);
        switch (msg.PayloadCase)
        {
            case Msg.PayloadOneofCase.Req:
                OnRequest?.Invoke(msg.Req);
                break;
            case Msg.PayloadOneofCase.Rsp:
                OnResponse?.Invoke(msg.Rsp);
                break;
            case Msg.PayloadOneofCase.Notify:
                OnNotify?.Invoke(msg.Notify);
                break;
            default:
                throw new ArgumentException($"message:{msg.PayloadCase.ToString()}");
        }
    }
    
    public T OnMsg<T>(byte[] data) where T : IMessage<T>, new()
    {
        return new MessageParser<T>(() => new T()).ParseFrom(data);
    }
}