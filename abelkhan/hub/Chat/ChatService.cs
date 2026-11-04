using System.Collections.Concurrent;
using chat;
using consts;
using core;
using engine;
using Google.Protobuf;

namespace hub;

/// <summary>
/// 聊天系统的编排层（hub 侧）。
///
/// 「以 group 为基准群发」的落地：框架的 <see cref="Group"/> 只是「本 hub 的在线客户端」，
/// 真正的 group 是 <c>(频道, 作用域 id)</c> 的成员集合。所以本类维护一份**本 hub 的成员索引**：
/// 登录时解析玩家在场景 / 帮会 / 门派里的 id，作用域变化时增量更新；
/// 收到跨 hub 广播后，只推给索引里命中该 <c>(channelId, scopeId)</c> 的本地玩家。
///
/// 频道消息走 Redis pub/sub 跨 hub 扇出（一条消息只写一次，不需要 hub 注册表）；
/// 点对点私聊直接按 presence 找到对方连接投递（不限好友、仅在线）。
/// </summary>
public sealed class ChatService
{
    private readonly ChatStore _store;
    private readonly RedisHandle _redis;

    /// <summary>本 hub 在线玩家：userId → Player。</summary>
    private readonly ConcurrentDictionary<string, Player> _players = new();

    /// <summary>本 hub 玩家的作用域缓存：userId → (scope → scopeId)。</summary>
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<ChatScope, string>> _scopes = new();

    /// <summary>成员索引：<c>scopeId</c> 路由键 → 本 hub 的成员。</summary>
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _members = new();

    public ChatService(ChatStore store, RedisHandle redis)
    {
        _store = store;
        _redis = redis;
    }

    public ChatStore Store => _store;

    /// <summary>游戏提供的展示信息解析（发送时把昵称 / 头像冗余进消息）。</summary>
    public Func<IReadOnlyCollection<string>, Task<IReadOnlyDictionary<string, ChatDisplay>>>? DisplayResolver
    {
        get;
        set;
    }

    /// <summary>
    /// 按 userId 查在线连接上的实体 id：在线返回实体 id，离线返回 null。
    /// 由 MainClass 注入（复用好友系统的 presence）。
    /// </summary>
    public Func<string, Task<string?>>? ResolveOnlineEntityId { get; set; }

    // ================================================================
    // 上下线 / 作用域
    // ================================================================

    public Task OnOnlineAsync(Player player)
    {
        _players[player.UserId] = player;

        var scopes = new ConcurrentDictionary<ChatScope, string>();
        foreach (var scope in Enum.GetValues<ChatScope>())
        {
            var scopeId = ResolveScope(player, scope);
            if (!string.IsNullOrEmpty(scopeId))
            {
                scopes[scope] = scopeId;
            }
        }

        _scopes[player.UserId] = scopes;
        foreach (var (scope, scopeId) in scopes)
        {
            AddMembership(player.UserId, scope, scopeId);
        }

        return Task.CompletedTask;
    }

    public Task OnOfflineAsync(string userId)
    {
        _players.TryRemove(userId, out _);
        if (_scopes.TryRemove(userId, out var scopes))
        {
            foreach (var (scope, scopeId) in scopes)
            {
                RemoveMembership(userId, scope, scopeId);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 玩家某个作用域发生变化（切场景 / 加入或退出帮会 / 转门派）时调用，
    /// 增量更新本地成员索引。<paramref name="scopeId"/> 为空 = 离开该作用域。
    /// </summary>
    public void SetScope(string userId, ChatScope scope, string? scopeId)
    {
        var scopes = _scopes.GetOrAdd(userId, _ => new ConcurrentDictionary<ChatScope, string>());
        if (scopes.TryGetValue(scope, out var previous))
        {
            RemoveMembership(userId, scope, previous);
            scopes.TryRemove(scope, out _);
        }

        if (string.IsNullOrEmpty(scopeId))
        {
            return;
        }

        scopes[scope] = scopeId;
        AddMembership(userId, scope, scopeId);
    }

    /// <summary>重新解析某个玩家的全部作用域（游戏改完数据后调一次即可）。</summary>
    public void RefreshScopes(Player player)
    {
        foreach (var scope in Enum.GetValues<ChatScope>())
        {
            SetScope(player.UserId, scope, ResolveScope(player, scope));
        }
    }

    private string? ResolveScope(Player player, ChatScope scope) => scope switch
    {
        // 全区 / 全服是部署级固定值，不需要游戏提供
        ChatScope.Zone => _store.Settings.ZoneId,
        ChatScope.Server => _store.Settings.ServerId,
        _ => player.GetChatScope(scope),
    };

    private static string RoutingKey(ChatScope scope, string scopeId) => (int)scope + ":" + scopeId;

    private void AddMembership(string userId, ChatScope scope, string scopeId)
    {
        _members.GetOrAdd(RoutingKey(scope, scopeId), _ => new ConcurrentDictionary<string, byte>())[userId] = 0;
    }

    private void RemoveMembership(string userId, ChatScope scope, string scopeId)
    {
        if (_members.TryGetValue(RoutingKey(scope, scopeId), out var set))
        {
            set.TryRemove(userId, out _);
        }
    }

    // ================================================================
    // 频道
    // ================================================================

    /// <summary>可用频道列表（带冷却 / 保留条数，客户端可以直接展示）。</summary>
    public List<ChatChannel> Channels => _store.Settings.Channels;

    /// <summary>该玩家在这个频道的作用域 id；为空表示他不在这个作用域里（例如没有帮会）。</summary>
    public string? ScopeOf(Player player, ChatChannel channel)
    {
        if (_scopes.TryGetValue(player.UserId, out var scopes) &&
            scopes.TryGetValue(channel.Scope, out var cached))
        {
            return cached;
        }

        return ResolveScope(player, channel.Scope);
    }

    /// <summary>
    /// 频道发言：校验 → 冷却 → 写历史 → pub/sub 广播。
    /// 本 hub 也会收到自己的广播（统一一条分发路径），再把消息推给本地成员。
    /// </summary>
    public async Task<(ChatResult Result, ChatEntry? Entry)> SendChannelAsync(Player player, string channelId,
        string rawContent)
    {
        var channel = _store.Settings.FindChannel(channelId);
        if (channel == null)
        {
            return (ChatResult.Fail(ChatCodes.UnknownChannel), null);
        }

        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return (ChatResult.Fail(ChatCodes.EmptyContent), null);
        }

        if (rawContent.Trim().Length > Math.Max(1, _store.Settings.MaxLength))
        {
            return (ChatResult.Fail(ChatCodes.TooLong), null);
        }

        var content = _store.Settings.Sanitize(rawContent);
        if (content.Length == 0)
        {
            return (ChatResult.Fail(ChatCodes.EmptyContent), null);
        }

        // 内容过滤（敏感词 / 禁言）：游戏可重写，返回空表示拒发
        if (player.FilterChatContent(channel.Id, content) is not { Length: > 0 })
        {
            return (ChatResult.Fail(ChatCodes.Blocked), null);
        }

        var scopeId = ScopeOf(player, channel);
        if (string.IsNullOrEmpty(scopeId))
        {
            return (ChatResult.Fail(ChatCodes.NotInScope), null);
        }

        if (!await _store.TryAcquireCooldownAsync(player.UserId, channel.Id, channel.CooldownMs))
        {
            return (ChatResult.Fail(ChatCodes.Cooldown), null);
        }

        var entry = await BuildEntryAsync(player.UserId, channel.Id, content);
        await _store.AppendHistoryAsync(channel.Id, scopeId, entry, channel.HistoryCount);

        var broadcast = new ChatBroadcast
        {
            ChannelId = channel.Id,
            ScopeId = scopeId,
            Entry = ToProto(entry),
        };
        await _store.PublishAsync(broadcast.ToByteArray());

        return (ChatResult.Success(), entry);
    }

    /// <summary>拉频道历史（服务端自己解析该玩家在该频道的作用域）。</summary>
    public async Task<(ChatResult Result, List<ChatEntry> Entries)> HistoryAsync(Player player, string channelId,
        int count)
    {
        var channel = _store.Settings.FindChannel(channelId);
        if (channel == null)
        {
            return (ChatResult.Fail(ChatCodes.UnknownChannel), new List<ChatEntry>());
        }

        if (channel.HistoryCount <= 0)
        {
            return (ChatResult.Success(), new List<ChatEntry>());
        }

        var scopeId = ScopeOf(player, channel);
        if (string.IsNullOrEmpty(scopeId))
        {
            return (ChatResult.Fail(ChatCodes.NotInScope), new List<ChatEntry>());
        }

        var take = count <= 0 ? Math.Min(channel.HistoryCount, _store.Settings.MaxHistoryFetch) : count;
        var entries = await _store.ReadHistoryAsync(channel.Id, scopeId, take);
        return (ChatResult.Success(), entries);
    }

    /// <summary>收到一条跨 hub 广播：推给本 hub 命中该作用域的玩家。</summary>
    public async Task OnBroadcastAsync(byte[] payload)
    {
        ChatBroadcast broadcast;
        try
        {
            broadcast = ChatBroadcast.Parser.ParseFrom(payload);
        }
        catch (Exception e)
        {
            Log.Error("chat: decode broadcast failed: {0}", e.Message);
            return;
        }

        var channel = _store.Settings.FindChannel(broadcast.ChannelId);
        if (channel == null || broadcast.Entry == null)
        {
            return;
        }

        IEnumerable<string> targets;
        if (channel.Scope is ChatScope.Zone or ChatScope.Server)
        {
            // 全区 / 全服：每个在线玩家都是成员，不用建索引
            targets = _players.Keys.ToList();
        }
        else if (_members.TryGetValue(RoutingKey(channel.Scope, broadcast.ScopeId), out var set))
        {
            targets = set.Keys.ToList();
        }
        else
        {
            return;
        }

        var notify = new ClientChatMessageNotify { Entry = broadcast.Entry };
        foreach (var userId in targets)
        {
            await NotifyAsync(userId, Consts.ClientChatMessageNotify, notify);
        }
    }

    // ================================================================
    // 点对点私聊
    // ================================================================

    /// <summary>私聊：不限好友、仅在线。</summary>
    public async Task<(ChatResult Result, ChatEntry? Entry)> SendPrivateAsync(Player player, string toUserId,
        string rawContent)
    {
        if (string.IsNullOrWhiteSpace(toUserId))
        {
            return (ChatResult.Fail(ChatCodes.Offline), null);
        }

        if (string.Equals(toUserId, player.UserId, StringComparison.Ordinal))
        {
            return (ChatResult.Fail(ChatCodes.SelfTarget), null);
        }

        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return (ChatResult.Fail(ChatCodes.EmptyContent), null);
        }

        if (rawContent.Trim().Length > Math.Max(1, _store.Settings.MaxLength))
        {
            return (ChatResult.Fail(ChatCodes.TooLong), null);
        }

        var content = _store.Settings.Sanitize(rawContent);
        if (content.Length == 0)
        {
            return (ChatResult.Fail(ChatCodes.EmptyContent), null);
        }

        if (player.FilterChatContent(string.Empty, content) is not { Length: > 0 })
        {
            return (ChatResult.Fail(ChatCodes.Blocked), null);
        }

        if (!await _store.TryAcquirePrivateCooldownAsync(player.UserId))
        {
            return (ChatResult.Fail(ChatCodes.Cooldown), null);
        }

        // 仅在线：对方不在线直接报错，不存离线消息
        var entityId = ResolveOnlineEntityId == null ? null : await ResolveOnlineEntityId(toUserId);
        if (string.IsNullOrEmpty(entityId))
        {
            return (ChatResult.Fail(ChatCodes.Offline), null);
        }

        var entry = await BuildEntryAsync(player.UserId, string.Empty, content);
        await NotifyDirectAsync(toUserId, entityId, Consts.ClientChatPrivateNotify,
            new ClientChatPrivateNotify { Entry = ToProto(entry) });

        return (ChatResult.Success(), entry);
    }

    // ================================================================
    // 通知 / 组装
    // ================================================================

    /// <summary>推给本 hub 在线的玩家（不在本 hub 直接跳过）。</summary>
    private async Task NotifyAsync<T>(string userId, string method, T msg) where T : IMessage<T>, new()
    {
        if (_players.TryGetValue(userId, out var player))
        {
            await NotifyDirectAsync(userId, player.EntityId, method, msg);
        }
    }

    /// <summary>按 userId + entityId 投递：走 gate 的客户端队列，跨 hub 也能到。</summary>
    private async Task NotifyDirectAsync<T>(string userId, string entityId, string method, T msg)
        where T : IMessage<T>, new()
    {
        var forward = new GateForwardHubNotifyClientMq
        {
            UserId = userId,
            EntityId = entityId,
            Event = new CallRpc
            {
                ProtoName = method,
                Content = msg.ToByteString(),
            },
        };

        var rpc = new WRpc();
        await _redis.PushList(string.Format(Consts.EntityClientMq, userId),
            rpc.Notify(Consts.GateForwardHubNotifyClientMq, forward));
    }

    private async Task<ChatEntry> BuildEntryAsync(string senderId, string channelId, string content)
    {
        var display = await ResolveDisplayAsync(new[] { senderId });
        display.TryGetValue(senderId, out var sender);
        return new ChatEntry(
            MessageId: Guid.NewGuid().ToString("N"),
            ChannelId: channelId,
            SenderId: senderId,
            SenderName: sender?.Name ?? senderId,
            SenderAvatar: sender?.Avatar ?? string.Empty,
            Content: content,
            SendTime: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    private async Task<IReadOnlyDictionary<string, ChatDisplay>> ResolveDisplayAsync(
        IReadOnlyCollection<string> userIds)
    {
        var resolver = DisplayResolver;
        if (resolver == null || userIds.Count == 0)
        {
            return new Dictionary<string, ChatDisplay>();
        }

        try
        {
            return await resolver(userIds);
        }
        catch (Exception e)
        {
            Log.Error("chat: resolve display err:{0}", e);
            return new Dictionary<string, ChatDisplay>();
        }
    }

    /// <summary>把内部条目转成协议消息。</summary>
    public static ClientChatEntry ToProto(ChatEntry entry) => new()
    {
        MessageId = entry.MessageId,
        ChannelId = entry.ChannelId,
        SenderId = entry.SenderId,
        SenderName = entry.SenderName,
        SenderAvatar = entry.SenderAvatar,
        Content = entry.Content,
        SendTime = (ulong)Math.Max(0, entry.SendTime),
    };
}
