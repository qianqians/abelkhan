using core;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace quest;

/// <summary>
/// 任务系统的 Redis 存储层。
///
/// 并发前提：同一个玩家的请求在 hub 的 <c>Actor</c> 队列里串行执行，且一个玩家同时只在一个 hub 上，
/// 所以「读状态 → 算增量 → 写回」按玩家串行是安全的；多 key 的原子性用短 Lua 保证（同 <c>{quest}</c> slot）。
///
/// 四把 key 各司其职：
///   event   玩家事件状态（触发 / 完成条件读它，Count/Max/Flag 三种更新方式）
///   active  进行中的任务（含进度快照）
///   done    已完成的任务
///   progress 每个任务自己的进度计数（不同任务互不污染）
/// </summary>
public sealed class QuestStore
{
    private readonly RedisHandle _redis;
    private readonly QuestSettings _settings;

    public QuestStore(RedisHandle redis, QuestSettings settings)
    {
        _redis = redis;
        _settings = settings;
        Keys = new QuestKeys(settings.KeyPrefix);
    }

    public QuestSettings Settings => _settings;

    public QuestKeys Keys { get; }

    // ================================================================
    // 事件状态
    // ================================================================

    /// <summary>
    /// 上报一个事件，按 <paramref name="mode"/> 更新玩家的事件状态，返回更新后的值。
    /// </summary>
    public async Task<long> ApplyEventAsync(string userId, string conditionKey, QuestConditionMode mode, long amount)
    {
        var value = await _redis.Execute("quest.event", db => db.ScriptEvaluateAsync(
            ApplyEventScript,
            new RedisKey[] { Keys.Event(userId) },
            new RedisValue[] { conditionKey, (int)mode, amount }));

        return (long)value;
    }

    /// <summary>玩家的事件状态全量（<c>type|key → value</c>）。</summary>
    public async Task<Dictionary<string, long>> GetEventStateAsync(string userId)
    {
        var values = await _redis.Execute("quest.event_all", db => db.HashGetAllAsync(Keys.Event(userId)));
        var state = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            state[entry.Name.ToString()] = (long)entry.Value;
        }

        return state;
    }

    // ================================================================
    // 进行中 / 已完成
    // ================================================================

    public async Task<Dictionary<string, QuestState>> GetActiveStatesAsync(string userId)
    {
        var values = await _redis.Execute("quest.active_all", db => db.HashGetAllAsync(Keys.Active(userId)));
        var states = new Dictionary<string, QuestState>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            try
            {
                var state = JsonConvert.DeserializeObject<QuestState>(entry.Value.ToString());
                if (state != null)
                {
                    states[state.QuestId] = state;
                }
            }
            catch (Exception e)
            {
                Log.Error("quest: decode active state failed: {0}", e.Message);
            }
        }

        return states;
    }

    public async Task<Dictionary<string, long>> GetDoneAsync(string userId)
    {
        var values = await _redis.Execute("quest.done_all", db => db.HashGetAllAsync(Keys.Done(userId)));
        var done = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            done[entry.Name.ToString()] = (long)entry.Value;
        }

        return done;
    }

    /// <summary>接取任务：写入进行中状态，并把进度条件全部初始化为 0。</summary>
    public async Task<QuestResult> StartQuestAsync(string userId, QuestDefinition definition, long now,
        int maxActive)
    {
        var state = QuestState.Create(definition.QuestId, now);
        foreach (var condition in definition.Progress)
        {
            state.Progress[condition.ConditionKey] = 0;
        }

        var result = await _redis.Execute("quest.start", db => db.ScriptEvaluateAsync(
            StartQuestScript,
            new RedisKey[] { Keys.Active(userId), Keys.Done(userId), Keys.Progress(userId) },
            new RedisValue[]
            {
                definition.QuestId, JsonConvert.SerializeObject(state), now,
                Math.Max(1, maxActive), definition.Progress.Count,
            }.Concat(definition.Progress.Select(c => (RedisValue)c.ConditionKey)).ToArray()));

        return Parse(result);
    }

    /// <summary>
    /// 推进某条进度条件。Count 用 <paramref name="delta"/> 累加，
    /// Max 取 <paramref name="delta"/> 与当前值的较大者，Flag 直接置 1。返回新进度。
    /// </summary>
    public async Task<long> BumpProgressAsync(string userId, string questId, QuestCondition condition,
        long delta)
    {
        var field = questId + "|" + condition.ConditionKey;
        var value = await _redis.Execute("quest.progress", db => db.ScriptEvaluateAsync(
            BumpProgressScript,
            new RedisKey[] { Keys.Progress(userId) },
            new RedisValue[] { field, (int)condition.Mode, Math.Max(1, delta) }));

        return (long)value;
    }

    /// <summary>某个任务当前的进度（<c>type|key → 进度</c>）。</summary>
    public async Task<Dictionary<string, long>> GetQuestProgressAsync(string userId, string questId)
    {
        var values = await _redis.Execute("quest.progress_all",
            db => db.HashGetAllAsync(Keys.Progress(userId)));
        var prefix = questId + "|";
        var progress = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            var field = entry.Name.ToString();
            if (field.StartsWith(prefix, StringComparison.Ordinal))
            {
                progress[field[prefix.Length..]] = (long)entry.Value;
            }
        }

        return progress;
    }

    /// <summary>完成任务：移出进行中、记完成时间、清掉该任务的进度行。</summary>
    public async Task<QuestResult> CompleteQuestAsync(string userId, QuestDefinition definition, long now)
    {
        var result = await _redis.Execute("quest.complete", db => db.ScriptEvaluateAsync(
            CompleteQuestScript,
            new RedisKey[] { Keys.Active(userId), Keys.Done(userId), Keys.Progress(userId) },
            new RedisValue[]
            {
                definition.QuestId, now, definition.Progress.Count,
            }.Concat(definition.Progress.Select(c => (RedisValue)(definition.QuestId + "|" + c.ConditionKey)))
                .ToArray()));

        return Parse(result);
    }

    /// <summary>清空一个玩家的全部任务数据（GM / 重置用）。</summary>
    public async Task ResetPlayerAsync(string userId)
    {
        await _redis.Execute("quest.reset", db => db.ScriptEvaluateAsync(
            ResetPlayerScript,
            new RedisKey[]
            {
                Keys.Event(userId), Keys.Active(userId), Keys.Done(userId), Keys.Progress(userId),
            },
            Array.Empty<RedisValue>()));
    }

    // ================================================================
    // 内部
    // ================================================================

    private static QuestResult Parse(RedisResult raw)
    {
        try
        {
            var text = raw.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                return QuestResult.Fail("internal");
            }

            var obj = Newtonsoft.Json.Linq.JObject.Parse(text);
            var ok = obj.Value<bool?>("ok") ?? false;
            return ok ? QuestResult.Success() : QuestResult.Fail(obj.Value<string>("code") ?? "failed");
        }
        catch (Exception e)
        {
            Log.Error("quest: parse redis result failed: {0}", e.Message);
            return QuestResult.Fail("internal");
        }
    }

    // ================================================================
    // Lua：所有脚本返回 cjson 编码的单个字符串（或数字）
    // ================================================================

    private const string ApplyEventScript = """
        local key = KEYS[1]
        local field = ARGV[1]
        local mode = tonumber(ARGV[2])
        local amount = tonumber(ARGV[3])

        local cur = tonumber(redis.call('HGET', key, field) or '0')
        local next
        if mode == 2 then
          next = math.max(cur, amount)   -- Max：绝对值（等级 / 战力）
        elseif mode == 3 then
          next = 1                       -- Flag：发生过就为真
        else
          next = cur + amount            -- Count：累加
        end

        redis.call('HSET', key, field, next)
        return next
        """;

    private const string StartQuestScript = """
        local activeKey = KEYS[1]
        local doneKey = KEYS[2]
        local progressKey = KEYS[3]

        local questId = ARGV[1]
        local payload = ARGV[2]
        local now = ARGV[3]
        local maxActive = tonumber(ARGV[4])
        local conditionCount = tonumber(ARGV[5])

        if redis.call('HEXISTS', doneKey, questId) == 1 then
          return cjson.encode({ ok = false, code = 'already_done' })
        end
        if redis.call('HEXISTS', activeKey, questId) == 1 then
          return cjson.encode({ ok = false, code = 'already_active' })
        end
        if redis.call('HLEN', activeKey) >= maxActive then
          return cjson.encode({ ok = false, code = 'too_many_active' })
        end

        redis.call('HSET', activeKey, questId, payload)
        for i = 1, conditionCount do
          redis.call('HSET', progressKey, questId .. '|' .. ARGV[5 + i], 0)
        end

        return cjson.encode({ ok = true })
        """;

    private const string BumpProgressScript = """
        local key = KEYS[1]
        local field = ARGV[1]
        local mode = tonumber(ARGV[2])
        local delta = tonumber(ARGV[3])

        local cur = tonumber(redis.call('HGET', key, field) or '0')
        local next
        if mode == 2 then
          next = math.max(cur, delta)
        elseif mode == 3 then
          next = 1
        else
          next = cur + delta
        end

        redis.call('HSET', key, field, next)
        return next
        """;

    private const string CompleteQuestScript = """
        local activeKey = KEYS[1]
        local doneKey = KEYS[2]
        local progressKey = KEYS[3]

        local questId = ARGV[1]
        local now = ARGV[2]
        local conditionCount = tonumber(ARGV[3])

        if redis.call('HEXISTS', activeKey, questId) == 0 then
          if redis.call('HEXISTS', doneKey, questId) == 1 then
            return cjson.encode({ ok = false, code = 'already_done' })
          end
          return cjson.encode({ ok = false, code = 'not_active' })
        end

        redis.call('HDEL', activeKey, questId)
        redis.call('HSET', doneKey, questId, now)
        for i = 1, conditionCount do
          redis.call('HDEL', progressKey, ARGV[3 + i])
        end

        return cjson.encode({ ok = true })
        """;

    private const string ResetPlayerScript = """
        redis.call('DEL', KEYS[1], KEYS[2], KEYS[3], KEYS[4])
        return 1
        """;
}
