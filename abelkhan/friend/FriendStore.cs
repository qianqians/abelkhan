using core;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
namespace friend;

/// <summary>
/// 好友系统的 Redis 存储层。
///
/// 关系数据全部在 Redis（权威），玩家存档只是镜像。所有会改变双方状态的操作都用 Lua
/// 在 Redis 内原子执行，因此多 hub 进程并发（两个玩家分属不同 hub、甚至都不在线）时
/// 也不会出现「各自读到未满然后双双写入」的超限问题。
///
/// 添加好友分三步，对应三条 Lua：
///   1. <see cref="SubmitRequestAsync"/>：写申请（离线消息），此时预检双方上限；
///   2. <see cref="AcceptReserveAsync"/>：同意后占位（reserved+1 + pending 截止时间），
///      防止跨服确认期间重复同意导致超限；
///   3. <see cref="ConfirmAddAsync"/>：确认时再次校验对方上限并原子双写好友列表、清占位。
/// 任何一步失败 / hub 崩溃，都由 <see cref="SweepExpiredReservesAsync"/> 超时回收占位兜底。
/// </summary>
public sealed class FriendStore
{
    private readonly RedisHandle _redis;
    private readonly FriendSettings _settings;

    public FriendStore(RedisHandle redis, FriendSettings settings)
    {
        _redis = redis;
        _settings = settings;
        Keys = new FriendKeys(settings.KeyPrefix);
    }

    public FriendSettings Settings => _settings;

    public FriendKeys Keys { get; }

    // ================================================================
    // 申请
    // ================================================================

    /// <summary>
    /// 申请加好友：<paramref name="fromUserId"/> 申请 <paramref name="toUserId"/>。
    /// 此处预检「被申请方」是否已满（已满则直接拒绝，不产生离线消息），同时预检申请方。
    /// </summary>
    public async Task<FriendResult> SubmitRequestAsync(string fromUserId, string toUserId)
    {
        var invalid = Validate(fromUserId, toUserId);
        if (invalid != null)
        {
            return invalid;
        }

        var result = await _redis.Execute("friend.submit", db => db.ScriptEvaluateAsync(
            SubmitScript,
            new RedisKey[]
            {
                Keys.Friends(toUserId), Keys.Inbox(toUserId), Keys.Reserved(toUserId), Keys.Extra(toUserId),
                Keys.Friends(fromUserId), Keys.Sent(fromUserId), Keys.Reserved(fromUserId), Keys.Extra(fromUserId),
            },
            new RedisValue[]
            {
                fromUserId, toUserId, Math.Max(0, _settings.MaxFriends),
                Math.Max(1, _settings.MaxPendingRequests),
            }));

        return ParseResult(result, "submit_request");
    }

    /// <summary>拒绝（或撤回）一条申请：从收件箱与对方的已发列表里移除。</summary>
    public async Task<bool> RejectRequestAsync(string userId, string fromUserId)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(fromUserId))
        {
            return false;
        }

        var removed = await _redis.Execute("friend.reject", db => db.ScriptEvaluateAsync(
            RejectScript,
            new RedisKey[] { Keys.Inbox(userId), Keys.Sent(fromUserId) },
            new RedisValue[] { fromUserId, userId }));

        return (long)removed > 0;
    }

    // ================================================================
    // 同意 → 占位 → 确认
    // ================================================================

    /// <summary>
    /// 同意好友请求的第一步：在「同意方」身上占一个好友位。
    /// 返回的 <c>Deadline</c> 写进占位明细，供超时回收。
    /// </summary>
    public async Task<(FriendResult Result, long Deadline)> AcceptReserveAsync(string userId, string fromUserId)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(fromUserId))
        {
            return (FriendResult.Fail(FriendCodes.Invalid), 0);
        }

        var deadline = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() +
                       Math.Max(1000, _settings.ReserveTimeoutMs);

        var result = await _redis.Execute("friend.accept", db => db.ScriptEvaluateAsync(
            AcceptReserveScript,
            new RedisKey[]
            {
                Keys.Friends(userId), Keys.Inbox(userId), Keys.Reserved(userId), Keys.Extra(userId),
                Keys.Pending(userId), PendingIndex,
            },
            new RedisValue[]
            {
                userId, fromUserId, Math.Max(0, _settings.MaxFriends), deadline,
            }));

        var parsed = ParseResult(result, "accept_reserve");
        return (parsed, parsed.Ok ? deadline : 0);
    }

    /// <summary>
    /// 同意好友请求的第二步（在「被申请方」侧执行）：再次校验对方上限，原子写入双方好友列表，
    /// 并清除同意方的占位（成功 / 对方已满都会清，失败不留占位）。
    /// </summary>
    public async Task<FriendResult> ConfirmAddAsync(string userId, string requesterId)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(requesterId))
        {
            return FriendResult.Fail(FriendCodes.Invalid);
        }

        var result = await _redis.Execute("friend.confirm", db => db.ScriptEvaluateAsync(
            ConfirmAddScript,
            new RedisKey[]
            {
                Keys.Friends(userId), Keys.Reserved(userId), Keys.Extra(userId),
                Keys.Friends(requesterId), Keys.Inbox(requesterId), Keys.Sent(requesterId),
                Keys.Reserved(requesterId), Keys.Pending(requesterId), PendingIndex,
            },
            new RedisValue[]
            {
                userId, requesterId, Math.Max(0, _settings.MaxFriends),
            }));

        return ParseResult(result, "confirm_add");
    }

    /// <summary>回滚一次占位（跨服确认失败、路由找不到、离线实例加载失败时调用）。</summary>
    public async Task<bool> CancelReserveAsync(string userId, string otherUserId)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(otherUserId))
        {
            return false;
        }

        var removed = await _redis.Execute("friend.cancel_reserve", db => db.ScriptEvaluateAsync(
            CancelReserveScript,
            new RedisKey[] { Keys.Reserved(userId), Keys.Pending(userId), PendingIndex },
            new RedisValue[] { userId, otherUserId }));

        return (long)removed > 0;
    }

    /// <summary>
    /// 回收所有超期占位（hub 定时调用）。返回被回收的 (同意方, 对方) 列表，
    /// 调用方可以据此给同意方推「加好友超时」通知。
    /// </summary>
    public async Task<List<(string UserId, string OtherId)>> SweepExpiredReservesAsync(int limit)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var users = await _redis.Execute("friend.sweep_index", db => db.SortedSetRangeByScoreAsync(
            PendingIndex, double.NegativeInfinity, now, Exclude.None, Order.Ascending, 0,
            Math.Max(1, limit)));

        var expired = new List<(string, string)>();
        foreach (var user in users)
        {
            var userId = user.ToString();
            var raw = await _redis.Execute("friend.sweep_user", db => db.ScriptEvaluateAsync(
                SweepUserScript,
                new RedisKey[] { Keys.Pending(userId), Keys.Reserved(userId), PendingIndex },
                new RedisValue[] { userId, now, Math.Max(1, limit) }));

            var obj = SafeParse(raw);
            if (obj == null)
            {
                continue;
            }

            var joined = obj.Value<string>("expired") ?? string.Empty;
            foreach (var other in joined.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                expired.Add((userId, other));
            }
        }

        return expired;
    }

    // ================================================================
    // 查询 / 删除
    // ================================================================

    /// <summary>好友 id 列表（顺序不保证，展示信息由调用方补齐）。</summary>
    public async Task<List<string>> ListFriendsAsync(string userId)
    {
        return await ReadSet(Keys.Friends(userId), "friend.list");
    }

    /// <summary>待处理申请（离线消息）列表。</summary>
    public async Task<List<string>> ListInboxAsync(string userId)
    {
        return await ReadSet(Keys.Inbox(userId), "friend.inbox");
    }

    /// <summary>我已发出的待处理申请。</summary>
    public async Task<List<string>> ListSentAsync(string userId)
    {
        return await ReadSet(Keys.Sent(userId), "friend.sent");
    }

    /// <summary>好友上限信息。</summary>
    public async Task<FriendLimit> GetLimitAsync(string userId)
    {
        var (friends, reserved, extra) = await _redis.Execute("friend.limit", async db =>
        {
            var friends = await db.SetLengthAsync(Keys.Friends(userId));
            var reserved = await db.StringGetAsync(Keys.Reserved(userId));
            var extra = await db.StringGetAsync(Keys.Extra(userId));
            return (friends, (long)(reserved.HasValue ? (long)reserved : 0),
                (long)(extra.HasValue ? (long)extra : 0));
        });

        return new FriendLimit(friends, reserved, extra, Math.Max(0, _settings.MaxFriends));
    }

    /// <summary>设置某个玩家的额外好友位（游戏发放 VIP / 道具好友位时调用）。</summary>
    public async Task SetExtraLimitAsync(string userId, long extra)
    {
        if (extra <= 0)
        {
            _redis.DelData(Keys.Extra(userId));
            return;
        }

        await _redis.SetStrData(Keys.Extra(userId), extra.ToString(), 0);
    }

    /// <summary>删除好友（双向），并清理双方残留的申请 / 占位。</summary>
    public async Task<bool> RemoveFriendAsync(string userId, string otherUserId)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(otherUserId))
        {
            return false;
        }

        var removed = await _redis.Execute("friend.remove", db => db.ScriptEvaluateAsync(
            RemoveFriendScript,
            new RedisKey[]
            {
                Keys.Friends(userId), Keys.Friends(otherUserId),
                Keys.Inbox(userId), Keys.Inbox(otherUserId),
                Keys.Sent(userId), Keys.Sent(otherUserId),
            },
            new RedisValue[] { userId, otherUserId }));

        return (long)removed > 0;
    }

    /// <summary>两人是否已经是好友。</summary>
    public async Task<bool> AreFriendsAsync(string userId, string otherUserId)
    {
        return await _redis.Execute("friend.are_friends",
            db => db.SetContainsAsync(Keys.Friends(userId), otherUserId));
    }

    // ================================================================
    // presence（在线标记，gate 维护）/ 路由（hub 维护）
    // ================================================================

    /// <summary>刷新在线标记（gate 每 3s 调用一次）。</summary>
    public async Task SetPresenceAsync(string userId, string gateId, string entityId)
    {
        await _redis.SetStrData(Keys.Presence(userId), EncodePresence(gateId, entityId),
            Math.Max(1000, _settings.PresenceTtlMs));
    }

    /// <summary>释放在线标记；只有当标记仍属于本连接时才删除，避免误删同一玩家的新连接。</summary>
    public async Task ReleasePresenceAsync(string userId, string gateId, string entityId)
    {
        await _redis.Execute("friend.presence_release", db => db.ScriptEvaluateAsync(
            ReleasePresenceScript,
            new RedisKey[] { Keys.Presence(userId) },
            new RedisValue[] { EncodePresence(gateId, entityId) }));
    }

    /// <summary>
    /// 批量查在线状态（一次 MGET，避免逐 key 往返）。
    /// 返回的字典只含在线的玩家。
    /// </summary>
    public async Task<Dictionary<string, FriendPresence>> GetPresenceAsync(IReadOnlyCollection<string> userIds)
    {
        var online = new Dictionary<string, FriendPresence>();
        if (userIds.Count == 0)
        {
            return online;
        }

        var ids = userIds.Distinct().ToList();
        var keys = ids.Select(id => (RedisKey)Keys.Presence(id)).ToArray();
        var values = await _redis.Execute("friend.presence_mget", db => db.StringGetAsync(keys));

        for (var i = 0; i < ids.Count && i < values.Length; i++)
        {
            var raw = values[i];
            if (!raw.HasValue || raw.IsNullOrEmpty)
            {
                continue;
            }

            online[ids[i]] = DecodePresence(raw.ToString());
        }

        return online;
    }

    private static string EncodePresence(string gateId, string entityId) => gateId + "|" + entityId;

    private static FriendPresence DecodePresence(string raw)
    {
        var index = raw.IndexOf('|');
        return index < 0
            ? new FriendPresence(raw, string.Empty)
            : new FriendPresence(raw[..index], raw[(index + 1)..]);
    }

    /// <summary>写入 / 刷新玩家所在 hub 的路由（TTL 到期即视为离线）。</summary>
    public async Task SetRouteAsync(string userId, string hubId)
    {
        await _redis.SetStrData(Keys.Route(userId), hubId, Math.Max(1000, _settings.RouteTtlMs));
    }

    /// <summary>读取玩家所在 hub；null / 空表示不在线。</summary>
    public async Task<string?> GetRouteAsync(string userId)
    {
        var value = await _redis.GetStrData(Keys.Route(userId));
        return value.HasValue && !value.IsNullOrEmpty ? value.ToString() : null;
    }

    // ================================================================
    // 跨 hub 指令队列
    // ================================================================

    public async Task PushHubCommandAsync(string hubId, byte[] payload)
    {
        await _redis.PushList(Keys.HubMq(hubId), payload);
    }

    public async Task<List<byte[]>> PopHubCommandsAsync(string hubId, int count)
    {
        // PopList 的返回类型本身可空（底层 Redis 未连接时返回 null Task）
        var task = _redis.PopList(Keys.HubMq(hubId), Math.Max(1, count));
        if (task == null)
        {
            return new List<byte[]>();
        }

        var data = await task;
        return data ?? new List<byte[]>();
    }

    // ================================================================
    // 内部
    // ================================================================

    private string PendingIndex => Keys.Prefix + "friend:{friend}:pending_index";

    private static FriendResult? Validate(string? fromUserId, string? toUserId)
    {
        if (string.IsNullOrWhiteSpace(fromUserId) || string.IsNullOrWhiteSpace(toUserId))
        {
            return FriendResult.Fail(FriendCodes.Invalid, "user id 不能为空");
        }

        return null;
    }

    private async Task<List<string>> ReadSet(string key, string operation)
    {
        var members = await _redis.Execute(operation, db => db.SetMembersAsync(key));
        return members.Select(m => m.ToString()).ToList();
    }

    private static FriendResult ParseResult(RedisResult raw, string operation)
    {
        var obj = SafeParse(raw);
        if (obj == null)
        {
            Log.Error("friend: {0} 返回无法解析: {1}", operation, raw);
            return FriendResult.Fail("internal", "redis result invalid");
        }

        var ok = obj.Value<bool?>("ok") ?? false;
        var code = obj.Value<string>("code") ?? (ok ? FriendCodes.Ok : "failed");
        return ok ? FriendResult.Success(code) : FriendResult.Fail(code);
    }

    private static JObject? SafeParse(RedisResult raw)
    {
        try
        {
            var text = raw.ToString();
            return string.IsNullOrWhiteSpace(text) ? null : JObject.Parse(text);
        }
        catch (Exception e)
        {
            Log.Error("friend: parse redis result failed: {0}", e.Message);
            return null;
        }
    }

    // ================================================================
    // Lua：所有脚本返回 cjson 编码的单个字符串，避免依赖 Redis 多值返回的绑定细节
    // ================================================================

    private const string SubmitScript = """
        local toFriends = KEYS[1]
        local toInbox = KEYS[2]
        local toReserved = KEYS[3]
        local toExtra = KEYS[4]
        local fromFriends = KEYS[5]
        local fromSent = KEYS[6]
        local fromReserved = KEYS[7]
        local fromExtra = KEYS[8]

        local from = ARGV[1]
        local to = ARGV[2]
        local maxFriends = tonumber(ARGV[3])
        local maxPending = tonumber(ARGV[4])

        if from == to then
          return cjson.encode({ ok = false, code = 'self' })
        end

        -- 已是好友：清掉可能残留的申请，幂等返回
        if redis.call('SISMEMBER', toFriends, from) == 1 then
          redis.call('SREM', toInbox, from)
          redis.call('SREM', fromSent, to)
          return cjson.encode({ ok = false, code = 'already_friend' })
        end

        -- 已申请过：幂等（不重复产生离线消息）
        if redis.call('SISMEMBER', toInbox, from) == 1 then
          return cjson.encode({ ok = false, code = 'duplicate' })
        end

        if redis.call('SCARD', toInbox) >= maxPending then
          return cjson.encode({ ok = false, code = 'inbox_full' })
        end

        -- 此处判断「被申请方」好友数是否已达上限（含临时占位）
        local toUsed = redis.call('SCARD', toFriends) + tonumber(redis.call('GET', toReserved) or '0')
        local toCap = maxFriends + tonumber(redis.call('GET', toExtra) or '0')
        if toUsed >= toCap then
          return cjson.encode({ ok = false, code = 'target_full' })
        end

        -- 申请方也提前校验，避免对方同意后才发现加不上
        local fromUsed = redis.call('SCARD', fromFriends) + tonumber(redis.call('GET', fromReserved) or '0')
        local fromCap = maxFriends + tonumber(redis.call('GET', fromExtra) or '0')
        if fromUsed >= fromCap then
          return cjson.encode({ ok = false, code = 'requester_full' })
        end

        redis.call('SADD', toInbox, from)
        redis.call('SADD', fromSent, to)
        return cjson.encode({ ok = true })
        """;

    private const string RejectScript = """
        local inbox = KEYS[1]
        local sent = KEYS[2]
        local from = ARGV[1]
        local owner = ARGV[2]

        local removed = redis.call('SREM', inbox, from)
        redis.call('SREM', sent, owner)
        return removed
        """;

    private const string AcceptReserveScript = """
        local friends = KEYS[1]
        local inbox = KEYS[2]
        local reserved = KEYS[3]
        local extra = KEYS[4]
        local pending = KEYS[5]
        local pendingIndex = KEYS[6]

        local user = ARGV[1]
        local from = ARGV[2]
        local maxFriends = tonumber(ARGV[3])
        local deadline = tonumber(ARGV[4])

        if redis.call('SISMEMBER', inbox, from) == 0 then
          return cjson.encode({ ok = false, code = 'no_request' })
        end
        if redis.call('SISMEMBER', friends, from) == 1 then
          return cjson.encode({ ok = false, code = 'already_friend' })
        end

        local used = redis.call('SCARD', friends) + tonumber(redis.call('GET', reserved) or '0')
        local cap = maxFriends + tonumber(redis.call('GET', extra) or '0')
        if used >= cap then
          return cjson.encode({ ok = false, code = 'self_full' })
        end

        redis.call('INCR', reserved)
        redis.call('ZADD', pending, deadline, from)

        -- 索引里记「最早到期的占位时间」，超时扫描从这里取人
        local cur = redis.call('ZSCORE', pendingIndex, user)
        if (not cur) or deadline < tonumber(cur) then
          redis.call('ZADD', pendingIndex, deadline, user)
        end

        return cjson.encode({ ok = true })
        """;

    private const string ConfirmAddScript = """
        local bFriends = KEYS[1]
        local bReserved = KEYS[2]
        local bExtra = KEYS[3]
        local aFriends = KEYS[4]
        local aInbox = KEYS[5]
        local aSent = KEYS[6]
        local aReserved = KEYS[7]
        local aPending = KEYS[8]
        local pendingIndex = KEYS[9]

        local b = ARGV[1]
        local a = ARGV[2]
        local maxFriends = tonumber(ARGV[3])

        -- 清一位占位（只有真的存在占位时才 -1，避免把计数减成负数）
        local function clear_reserve(member)
          local removed = redis.call('ZREM', aPending, member)
          if removed > 0 then
            local r = tonumber(redis.call('GET', aReserved) or '0')
            if r > 0 then
              redis.call('DECR', aReserved)
            end
          end
          if redis.call('ZCARD', aPending) == 0 then
            redis.call('ZREM', pendingIndex, a)
          else
            local first = redis.call('ZRANGE', aPending, 0, 0, 'WITHSCORES')
            redis.call('ZADD', pendingIndex, first[2], a)
          end
        end

        if redis.call('SISMEMBER', bFriends, a) == 1 then
          redis.call('SREM', aInbox, b)
          redis.call('SREM', aSent, b)
          clear_reserve(b)
          return cjson.encode({ ok = true, code = 'already_friend' })
        end

        -- 申请必须还在，否则说明已经被拒绝 / 撤回
        if redis.call('SISMEMBER', aInbox, b) == 0 then
          clear_reserve(b)
          return cjson.encode({ ok = false, code = 'no_request' })
        end

        -- 此处判断「被申请方」(b) 好友数是否在上限内
        local bUsed = redis.call('SCARD', bFriends) + tonumber(redis.call('GET', bReserved) or '0')
        local bCap = maxFriends + tonumber(redis.call('GET', bExtra) or '0')
        if bUsed >= bCap then
          clear_reserve(b)
          return cjson.encode({ ok = false, code = 'target_full' })
        end

        redis.call('SADD', bFriends, a)
        redis.call('SADD', aFriends, b)
        redis.call('SREM', aInbox, b)
        redis.call('SREM', aSent, b)
        clear_reserve(b)
        return cjson.encode({ ok = true })
        """;

    private const string CancelReserveScript = """
        local reserved = KEYS[1]
        local pending = KEYS[2]
        local pendingIndex = KEYS[3]

        local user = ARGV[1]
        local other = ARGV[2]

        local removed = redis.call('ZREM', pending, other)
        if removed > 0 then
          local r = tonumber(redis.call('GET', reserved) or '0')
          if r > 0 then
            redis.call('DECR', reserved)
          end
        end

        if redis.call('ZCARD', pending) == 0 then
          redis.call('ZREM', pendingIndex, user)
        else
          local first = redis.call('ZRANGE', pending, 0, 0, 'WITHSCORES')
          redis.call('ZADD', pendingIndex, first[2], user)
        end

        return removed
        """;

    private const string SweepUserScript = """
        local pending = KEYS[1]
        local reserved = KEYS[2]
        local pendingIndex = KEYS[3]

        local user = ARGV[1]
        local now = tonumber(ARGV[2])
        local limit = tonumber(ARGV[3])

        local expired = redis.call('ZRANGEBYSCORE', pending, '-inf', now, 'LIMIT', 0, limit)
        local n = #expired
        for i = 1, n do
          redis.call('ZREM', pending, expired[i])
        end

        if n > 0 then
          local r = tonumber(redis.call('GET', reserved) or '0')
          local nr = r - n
          if nr < 0 then nr = 0 end
          redis.call('SET', reserved, nr)
        end

        if redis.call('ZCARD', pending) == 0 then
          redis.call('ZREM', pendingIndex, user)
        else
          local first = redis.call('ZRANGE', pending, 0, 0, 'WITHSCORES')
          redis.call('ZADD', pendingIndex, first[2], user)
        end

        -- 用 '|' 拼接而不是 cjson 数组：空表会被编码成 {} 而不是 []，C# 侧不好区分
        return cjson.encode({ expired = table.concat(expired, '|') })
        """;

    private const string RemoveFriendScript = """
        local aFriends = KEYS[1]
        local bFriends = KEYS[2]
        local aInbox = KEYS[3]
        local bInbox = KEYS[4]
        local aSent = KEYS[5]
        local bSent = KEYS[6]

        local a = ARGV[1]
        local b = ARGV[2]

        local removed = redis.call('SREM', aFriends, b)
        redis.call('SREM', bFriends, a)
        redis.call('SREM', aInbox, b)
        redis.call('SREM', bInbox, a)
        redis.call('SREM', aSent, b)
        redis.call('SREM', bSent, a)
        return removed
        """;

    private const string ReleasePresenceScript = """
        if redis.call('GET', KEYS[1]) == ARGV[1] then
          return redis.call('DEL', KEYS[1])
        end
        return 0
        """;
}
