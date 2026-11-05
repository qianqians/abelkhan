namespace client;

/// <summary>
/// 客户端传输层抽象。<see cref="Client"/> 默认用 <see cref="WebSocketNetwork"/>（连 gate），
/// 测试 / 引擎集成可以注入自己的实现（例如进程内管道、引擎自带的网络层）。
///
/// 约定：<see cref="Start"/> 传入的 <c>onPacket</c> 用来把**收到的完整报文**交给框架
/// （框架自己做 protobuf 解析 + 分发），实现方不需要关心拆包之外的任何协议语义。
/// </summary>
public interface IClientNetwork
{
    /// <summary>连接断开（只会触发一次）</summary>
    event Action? OnClose;

    bool IsOpen { get; }

    /// <summary>开始收发；收到的整包通过 <paramref name="onPacket"/> 交给框架</summary>
    void Start(Action<byte[]> onPacket);

    /// <summary>发送一个完整报文</summary>
    Task Send(byte[] data);

    /// <summary>本地关闭</summary>
    Task Close();
}
