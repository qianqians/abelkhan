using client;

namespace Abelkhan.Stride.Net;

/// <summary>接入参数：连哪个 gate、用哪个 service、演示用哪个频道。</summary>
public sealed class AbelkhanClientOptions
{
    /// <summary>gate 的 WebSocket 地址</summary>
    public string Wss { get; set; } = "wss://127.0.0.1:9001/";

    /// <summary>首次进服要发 ClientRequestService 的服务名（gate 的 EnterService）</summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>重连账号：非空则连接后发 ClientRequestClientReconnect</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>进服务时带给 hub 的参数（游戏自己解析）</summary>
    public byte[] Argv { get; set; } = [];

    /// <summary>自签证书时打开</summary>
    public bool IgnoreCertificate { get; set; } = true;

    /// <summary>演示用：发言的频道 id</summary>
    public string ChatChannel { get; set; } = "world";

    /// <summary>演示用：每隔多久自动发一条频道消息（秒，0 = 不发）</summary>
    public double AutoChatIntervalSeconds { get; set; } = 8;
}

/// <summary>
/// 客户端 Player：游戏的玩家实体。这里把 SDK 的通知事件直接转成游戏能消费的回调。
/// </summary>
public sealed class SamplePlayer(string entityId, string entityType) : client.Player(entityId, entityType)
{
    /// <summary>任意一条人类可读的日志（示例里打到控制台 + 屏幕左上角）</summary>
    public event Action<string>? Log;

    /// <summary>最近 N 条聊天 / 通知，供 UI 消费</summary>
    public List<string> RecentMessages { get; } = new();

    internal void Attach()
    {
        FriendRequested += n => Add($"[好友] 来自 {n.From.UserId} 的申请");
        FriendOnline += n => Add($"[好友] {n.UserId} 上线");
        FriendOffline += n => Add($"[好友] {n.UserId} 下线");
        FriendAdded += n => Add($"[好友] 已和 {n.UserId} 成为好友");
        FriendRemoved += n => Add($"[好友] {n.UserId} 把你删了");
        FriendAcceptTimeout += n => Add($"[好友] 同意 {n.UserId} 超时，可重试");

        ChatMessage += n => Add($"[{n.Entry.ChannelId}] {n.Entry.SenderName}: {n.Entry.Content}");
        ChatPrivate += n => Add($"[私聊] {n.Entry.SenderName}: {n.Entry.Content}");

        GuildUpdated += n => Add($"[帮会] 信息变了（{n.Reason}），请刷新");
        GuildInvited += n => Add($"[帮会] {n.InviterId} 邀请你加入 {n.GuildName}");
        GuildRemoved += n => Add($"[帮会] 已离开（{n.Reason}）");
        GuildTaskUpdated += n => Add($"[帮会] 任务 {n.Task.TaskId} → {n.Task.Status}");

        QuestUpdated += n => Add($"[任务] {n.Quest.QuestId} → {n.Quest.Status}");
    }

    private void Add(string line)
    {
        RecentMessages.Add(line);
        if (RecentMessages.Count > 32)
        {
            RecentMessages.RemoveAt(0);
        }

        Log?.Invoke(line);
    }
}

/// <summary>客户端实体工厂：把服务器下发的 entity_type 映射成 <see cref="SamplePlayer"/>。</summary>
public sealed class SampleClientService : client.Service
{
    private readonly Action<SamplePlayer>? _onPlayerCreated;

    public SampleClientService(Action<SamplePlayer>? onPlayerCreated = null)
    {
        _onPlayerCreated = onPlayerCreated;
    }

    public override client.Player CreatePlayerEntity(string entityId, string entityType, byte[] argv)
    {
        var player = new SamplePlayer(entityId, entityType);
        player.Attach();
        _onPlayerCreated?.Invoke(player);
        return player;
    }
}
