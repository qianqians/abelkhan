using Google.Protobuf;
using Google.Protobuf.Reflection;
namespace engine;

public static class WRpc
{
    public static byte[] Notify<T>(string method, T argv) where T : IMessage<T>, new()
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

    public static byte[] Request<T>(string method, string msgId, T argv) where T : IMessage<T>, new()
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

    public static event Action<Request>? OnRequest;
    public static event Action<Response>? OnResponse;
    public static event Action<Notify>? OnNotify;

    public static void OnNetworkData(byte[] data)
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
                throw new ArgumentException("message: " + msg.PayloadCase.ToString());
        }
    }
    
    public static T OnMsg<T>(byte[] data) where T : IMessage<T>, new()
    {
        return new MessageParser<T>(() => new T()).ParseFrom(data);
    }
}