namespace client;

/// <summary>
/// 客户端连接配置（客户端只支持 WebSocket）。
/// </summary>
public class ClientConfig
{
    /// <summary>gate 的 WebSocket 地址，例如 wss://127.0.0.1:9001/</summary>
    public string Wss { get; set; } = "wss://127.0.0.1:9001/";

    /// <summary>
    /// 跳过证书校验：gate 的 WebSocket 端口是 UseHttps(pfx, password)，
    /// 测试环境用自签证书时需要打开，生产环境应该关掉。
    /// </summary>
    public bool IgnoreCertificate { get; set; } = false;

    /// <summary>连接超时（毫秒）</summary>
    public long ConnectTimeoutMs { get; set; } = 5000;

    /// <summary>心跳间隔：gate 侧 10s 收不到任何事件就断开，默认 3s 一跳</summary>
    public long HeartBeatsIntervalMs { get; set; } = 3000;

    /// <summary>request 等待回包的超时（毫秒），0 表示不超时</summary>
    public long RequestTimeoutMs { get; set; } = 10000;

    /// <summary>断线后是否自动重连</summary>
    public bool AutoReconnect { get; set; } = true;

    /// <summary>重连间隔（毫秒）</summary>
    public long ReconnectIntervalMs { get; set; } = 3000;

    /// <summary>
    /// 重连后等待服务器重新认领实体的时间（毫秒）：
    /// 这期间没被 CreatePlayerEntity/CreateRemoteEntity/RefreshEntity 碰过的实体，本地当作已经不存在删掉。
    /// 0 表示不做这个清理（服务器必须自己发 DeleteRemoteEntity）。
    /// </summary>
    public long ResyncTimeoutMs { get; set; } = 3000;

    /// <summary>
    /// 版本握手：必须落在 gate 配置的 [MinVersion, MaxVersion] 之内，否则会收到 KickOff。
    /// gate 侧两者都是 0 时用默认值即可。
    /// </summary>
    public uint MinVersion { get; set; } = 0;

    /// <summary>版本握手上报的最大版本</summary>
    public uint MaxVersion { get; set; } = 0;

    /// <summary>
    /// 可选的主动进服务（ClientRequestService.service_name）。
    /// 注意 gate 在 accept 时已经把 EnterService 的请求投给 hub 了，
    /// 所以首次进服通常留空，等 CreatePlayerEntity 即可。
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>可选的重连账号：非空则连接后发 ClientRequestReconnect.user_id</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>进服务/重连时带给 hub 的参数</summary>
    public byte[] Argv { get; set; } = [];
}
