using core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace guild;

/// <summary>
/// 帮会的 Redis 存储层（权威数据）。
///
/// 所有会改变多把 key 的操作（创建 / 邀请 / 审批 / 入帮 / 踢人 / 转让 / 解散 / 任务）
/// 都用 Lua 在 Redis 内原子执行：帮会成员来自多个 hub 进程，任何「先读后写」的
/// C# 实现都会在并发下超员或产生两个帮主。
///
/// 上限校验与写入在同一个脚本里完成（成员上限 = 全局默认 + 帮会附加）。
/// </summary>
public sealed class GuildStore
{
    private readonly RedisHandle _redis;
    private readonly GuildSettings _settings;

    public GuildStore(RedisHandle redis, GuildSettings settings)
    {
        _redis = redis;
        _settings = settings;
        Keys = new GuildKeys(settings.KeyPrefix);
    }

    public GuildSettings Settings => _settings;

    public GuildKeys Keys { get; }

    public static string NewGuildId() => "G" + Guid.NewGuid().ToString("N")[..12];

    // ================================================================
    // 创建 / 改名 / 公告
    // ================================================================

    /// <summary>创建帮会（创建者直接成为帮主）。</summary>
    public async Task<GuildResult> CreateAsync(string guildId, string name, string leaderId, int capExtra = 0)
    {
        var nameKey = GuildSettings.NameKey(name);
        var result = await _redis.Execute("guild.create", db => db.ScriptEvaluateAsync(
            CreateScript,
            new RedisKey[]
            {
                Keys.Info(guildId), Keys.Members(guildId), Keys.NameIndex(nameKey),
                Keys.PlayerGuild(leaderId), Keys.Index,
            },
            new RedisValue[]
            {
                guildId, name, leaderId, Now(), Math.Max(0, capExtra),
            }));

        return Parse(result);
    }

    /// <summary>改名（仅帮主）。</summary>
    public async Task<GuildResult> RenameAsync(string guildId, string actorId, string newName)
    {
        var info = await GetInfoAsync(guildId);
        if (info == null)
        {
            return GuildResult.Fail(GuildCodes.NotFound);
        }

        var result = await _redis.Execute("guild.rename", db => db.ScriptEvaluateAsync(
            RenameScript,
            new RedisKey[]
            {
                Keys.Members(guildId), Keys.Info(guildId),
                Keys.NameIndex(GuildSettings.NameKey(info.Name)),
                Keys.NameIndex(GuildSettings.NameKey(newName)),
            },
            new RedisValue[] { actorId, newName, guildId }));

        return Parse(result);
    }

    /// <summary>改公告（仅帮主）。</summary>
    public async Task<GuildResult> SetNoticeAsync(string guildId, string actorId, string notice)
    {
        var result = await _redis.Execute("guild.notice", db => db.ScriptEvaluateAsync(
            SetNoticeScript,
            new RedisKey[] { Keys.Members(guildId), Keys.Info(guildId) },
            new RedisValue[] { actorId, notice }));

        return Parse(result);
    }

    // ================================================================
    // 邀请 / 申请 / 入帮
    // ================================================================

    /// <summary>邀请一个玩家（帮主 / 副帮主）。</summary>
    public async Task<GuildResult> InviteAsync(string guildId, string actorId, string targetId)
    {
        var result = await _redis.Execute("guild.invite", db => db.ScriptEvaluateAsync(
            InviteScript,
            new RedisKey[]
            {
                Keys.Members(guildId), Keys.PlayerGuild(targetId),
                Keys.Invites(targetId),
            },
            new RedisValue[]
            {
                actorId, guildId, Math.Max(1, _settings.MaxInvites),
            }));

        return Parse(result);
    }

    /// <summary>申请加入某个帮会。</summary>
    public async Task<GuildResult> ApplyAsync(string guildId, string userId)
    {
        var result = await _redis.Execute("guild.apply", db => db.ScriptEvaluateAsync(
            ApplyScript,
            new RedisKey[]
            {
                Keys.Info(guildId), Keys.PlayerGuild(userId), Keys.Applies(guildId),
            },
            new RedisValue[]
            {
                userId, Now(), Math.Max(1, _settings.MaxApplications),
            }));

        return Parse(result);
    }

    /// <summary>审批申请（<paramref name="accept"/> = false 时只从申请列表里删掉）。</summary>
    public async Task<GuildResult> ApproveAsync(string guildId, string actorId, string applicantId, bool accept)
    {
        if (!accept)
        {
            return await RejectApplyAsync(guildId, actorId, applicantId);
        }

        var result = await _redis.Execute("guild.approve", db => db.ScriptEvaluateAsync(
            ApproveScript,
            new RedisKey[]
            {
                Keys.Members(guildId), Keys.Applies(guildId),
                Keys.PlayerGuild(applicantId), Keys.Info(guildId),
            },
            new RedisValue[]
            {
                actorId, applicantId, guildId, Math.Max(1, _settings.MaxMembers),
            }));

        return Parse(result);
    }

    /// <summary>拒绝（或撤回）一条申请：有权限的人可以批，玩家自己也可以撤回。</summary>
    public async Task<GuildResult> RejectApplyAsync(string guildId, string actorId, string applicantId)
    {
        var result = await _redis.Execute("guild.reject_apply", db => db.ScriptEvaluateAsync(
            RejectApplyScript,
            new RedisKey[] { Keys.Members(guildId), Keys.Applies(guildId) },
            new RedisValue[] { actorId, applicantId }));

        return Parse(result);
    }

    /// <summary>接受某个帮会的邀请。</summary>
    public async Task<GuildResult> AcceptInviteAsync(string guildId, string userId)
    {
        var result = await _redis.Execute("guild.accept_invite", db => db.ScriptEvaluateAsync(
            AcceptInviteScript,
            new RedisKey[]
            {
                Keys.Invites(userId), Keys.Members(guildId), Keys.Info(guildId),
                Keys.PlayerGuild(userId),
            },
            new RedisValue[] { userId, guildId, Math.Max(1, _settings.MaxMembers) }));

        return Parse(result);
    }

    /// <summary>拒绝一个邀请（只从自己的邀请列表里删掉）。</summary>
    public async Task<GuildResult> RejectInviteAsync(string userId, string guildId)
    {
        var removed = await _redis.Execute("guild.reject_invite", db => db.ScriptEvaluateAsync(
            RejectInviteScript,
            new RedisKey[] { Keys.Invites(userId) },
            new RedisValue[] { guildId }));

        return (long)removed > 0 ? GuildResult.Success() : GuildResult.Fail(GuildCodes.NoRequest);
    }

    // ================================================================
    // 退出 / 踢人 / 职位 / 转让 / 解散
    // ================================================================

    public async Task<GuildResult> LeaveAsync(string guildId, string userId)
    {
        var result = await _redis.Execute("guild.leave", db => db.ScriptEvaluateAsync(
            LeaveScript,
            new RedisKey[] { Keys.Members(guildId), Keys.PlayerGuild(userId) },
            new RedisValue[] { userId }));

        return Parse(result);
    }

    public async Task<GuildResult> KickAsync(string guildId, string actorId, string targetId)
    {
        var result = await _redis.Execute("guild.kick", db => db.ScriptEvaluateAsync(
            KickScript,
            new RedisKey[] { Keys.Members(guildId), Keys.PlayerGuild(targetId) },
            new RedisValue[] { actorId, targetId }));

        return Parse(result);
    }

    /// <summary>升 / 降副帮主（帮主操作）。<paramref name="role"/> 只能是 Member 或 Officer。</summary>
    public async Task<GuildResult> SetRoleAsync(string guildId, string actorId, string targetId, GuildRole role)
    {
        var result = await _redis.Execute("guild.set_role", db => db.ScriptEvaluateAsync(
            SetRoleScript,
            new RedisKey[] { Keys.Members(guildId) },
            new RedisValue[]
            {
                actorId, targetId, (int)role, Math.Max(1, _settings.MaxOfficers),
            }));

        return Parse(result);
    }

    /// <summary>转让帮主：老帮主降为副帮主。</summary>
    public async Task<GuildResult> TransferAsync(string guildId, string actorId, string targetId)
    {
        var result = await _redis.Execute("guild.transfer", db => db.ScriptEvaluateAsync(
            TransferScript,
            new RedisKey[] { Keys.Members(guildId), Keys.Info(guildId) },
            new RedisValue[] { actorId, targetId }));

        return Parse(result);
    }

    /// <summary>
    /// 解散帮会（仅帮主）。返回被解散帮会的成员名单（用于通知 / 清理）。
    /// </summary>
    public async Task<(GuildResult Result, List<string> Members)> DisbandAsync(string guildId, string actorId)
    {
        var info = await GetInfoAsync(guildId);
        if (info == null)
        {
            return (GuildResult.Fail(GuildCodes.NotFound), new List<string>());
        }

        var result = await _redis.Execute("guild.disband", db => db.ScriptEvaluateAsync(
            DisbandScript,
            new RedisKey[]
            {
                Keys.Info(guildId), Keys.Members(guildId), Keys.Applies(guildId), Keys.Tasks(guildId),
                Keys.NameIndex(GuildSettings.NameKey(info.Name)), Keys.Index,
            },
            new RedisValue[]
            {
                actorId, guildId, Keys.PlayerGuildPrefix,
                Keys.TaskMembersPrefix(guildId), Keys.TaskProgressPrefix(guildId),
            }));

        var (parsed, data) = ParseWithData(result);
        var members = string.IsNullOrEmpty(data)
            ? new List<string>()
            : data.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        return (parsed, members);
    }

    // ================================================================
    // 任务
    // ================================================================

    public async Task<GuildResult> PublishTaskAsync(string guildId, string actorId, GuildTaskState state,
        bool officerAllowed)
    {
        var result = await _redis.Execute("guild.task_publish", db => db.ScriptEvaluateAsync(
            PublishTaskScript,
            new RedisKey[] { Keys.Members(guildId), Keys.Tasks(guildId) },
            new RedisValue[]
            {
                actorId, state.TaskId, JsonConvert.SerializeObject(state),
                officerAllowed ? 1 : 0,
            }));

        return Parse(result);
    }

    public async Task<GuildResult> CancelTaskAsync(string guildId, string actorId, string taskId)
    {
        var result = await _redis.Execute("guild.task_cancel", db => db.ScriptEvaluateAsync(
            CancelTaskScript,
            new RedisKey[]
            {
                Keys.Members(guildId), Keys.Tasks(guildId),
                Keys.TaskMembers(guildId, taskId), Keys.TaskProgress(guildId, taskId),
            },
            new RedisValue[] { actorId, taskId }));

        return Parse(result);
    }

    /// <summary>成员领取一条已发布的任务（写入领取名单 + 进度 0）。</summary>
    public async Task<GuildResult> AcceptTaskAsync(string guildId, string userId, string taskId)
    {
        var result = await _redis.Execute("guild.task_accept", db => db.ScriptEvaluateAsync(
            AcceptTaskScript,
            new RedisKey[]
            {
                Keys.Members(guildId), Keys.Tasks(guildId),
                Keys.TaskMembers(guildId, taskId), Keys.TaskProgress(guildId, taskId),
            },
            new RedisValue[] { userId, taskId }));

        return Parse(result);
    }

    /// <summary>
    /// 累加任务进度（由游戏逻辑调用）。返回 (结果, 当前进度, 是否刚好完成)。
    /// 共享任务的进度记在保留键 <c>_</c> 上。
    /// </summary>
    public async Task<(GuildResult Result, long Progress, bool Completed)> AddTaskProgressAsync(
        string guildId, string taskId, string memberId, long delta, long target, bool shared)
    {
        var result = await _redis.Execute("guild.task_progress", db => db.ScriptEvaluateAsync(
            AddTaskProgressScript,
            new RedisKey[]
            {
                Keys.Tasks(guildId), Keys.TaskMembers(guildId, taskId),
                Keys.TaskProgress(guildId, taskId),
            },
            new RedisValue[]
            {
                memberId, Math.Max(1, delta), Math.Max(1, target), shared ? 1 : 0, Now(),
            }));

        var obj = SafeParse(result);
        if (obj == null)
        {
            return (GuildResult.Fail("internal"), 0, false);
        }

        var ok = obj.Value<bool?>("ok") ?? false;
        var progress = obj.Value<long?>("progress") ?? 0;
        var completed = (obj.Value<long?>("completed") ?? 0) != 0;
        return ok
            ? (GuildResult.Success(), progress, completed)
            : (GuildResult.Fail(obj.Value<string>("code") ?? "failed"), progress, completed);
    }

    // ================================================================
    // 查询（非原子读，只在展示 / 校验时用）
    // ================================================================

    public async Task<GuildInfo?> GetInfoAsync(string guildId)
    {
        if (string.IsNullOrWhiteSpace(guildId))
        {
            return null;
        }

        var values = await _redis.Execute("guild.info",
            db => db.HashGetAllAsync(Keys.Info(guildId)));
        return ToInfo(guildId, values);
    }

    public async Task<GuildInfo?> GetPlayerGuildAsync(string userId)
    {
        var value = await _redis.GetStrData(Keys.PlayerGuild(userId));
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return null;
        }

        return await GetInfoAsync(value.ToString());
    }

    /// <summary>玩家当前所在帮会 id（不在帮会返回 null）。</summary>
    public async Task<string?> GetPlayerGuildIdAsync(string userId)
    {
        var value = await _redis.GetStrData(Keys.PlayerGuild(userId));
        return value.HasValue && !value.IsNullOrEmpty ? value.ToString() : null;
    }

    public async Task<Dictionary<string, GuildRole>> GetMembersAsync(string guildId)
    {
        var values = await _redis.Execute("guild.members",
            db => db.HashGetAllAsync(Keys.Members(guildId)));
        var members = new Dictionary<string, GuildRole>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            members[entry.Name.ToString()] = (GuildRole)(int)entry.Value;
        }

        return members;
    }

    public async Task<GuildRole> GetRoleAsync(string guildId, string userId)
    {
        var value = await _redis.Execute("guild.role",
            db => db.HashGetAsync(Keys.Members(guildId), userId));
        return value.HasValue ? (GuildRole)(int)value : GuildRole.None;
    }

    /// <summary>玩家收到的邀请：guildId → 邀请人。</summary>
    public async Task<Dictionary<string, string>> GetInvitesAsync(string userId)
    {
        var values = await _redis.Execute("guild.invites",
            db => db.HashGetAllAsync(Keys.Invites(userId)));
        return values.ToDictionary(e => e.Name.ToString(), e => e.Value.ToString(), StringComparer.Ordinal);
    }

    /// <summary>帮会的待审批申请：userId → 申请时间。</summary>
    public async Task<Dictionary<string, long>> GetAppliesAsync(string guildId)
    {
        var values = await _redis.Execute("guild.applies",
            db => db.HashGetAllAsync(Keys.Applies(guildId)));
        var applies = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            applies[entry.Name.ToString()] = (long)entry.Value;
        }

        return applies;
    }

    /// <summary>已发布任务：taskId → 状态。</summary>
    public async Task<Dictionary<string, GuildTaskState>> GetTasksAsync(string guildId)
    {
        var values = await _redis.Execute("guild.tasks",
            db => db.HashGetAllAsync(Keys.Tasks(guildId)));
        var tasks = new Dictionary<string, GuildTaskState>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            try
            {
                var state = JsonConvert.DeserializeObject<GuildTaskState>(entry.Value.ToString());
                if (state != null)
                {
                    tasks[state.TaskId] = state;
                }
            }
            catch (Exception e)
            {
                Log.Error("guild: decode task failed: {0}", e.Message);
            }
        }

        return tasks;
    }

    public async Task<GuildTaskState?> GetTaskAsync(string guildId, string taskId)
    {
        var value = await _redis.Execute("guild.task",
            db => db.HashGetAsync(Keys.Tasks(guildId), taskId));
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<GuildTaskState>(value.ToString());
        }
        catch (Exception e)
        {
            Log.Error("guild: decode task failed: {0}", e.Message);
            return null;
        }
    }

    /// <summary>把任务标记为已完成（状态幂等，重复写没副作用）。</summary>
    public Task MarkTaskFinishedAsync(string guildId, string taskId, GuildTaskState state)
    {
        return _redis.Execute("guild.task_finish", db => db.HashSetAsync(
            Keys.Tasks(guildId), taskId, JsonConvert.SerializeObject(state))).AsTask();
    }

    /// <summary>某个任务的全部进度：memberId → 进度（共享任务的键是 <c>_</c>）。</summary>
    public async Task<Dictionary<string, long>> GetTaskProgressAsync(string guildId, string taskId)
    {
        var values = await _redis.Execute("guild.task_progress_all",
            db => db.HashGetAllAsync(Keys.TaskProgress(guildId, taskId)));
        var progress = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            progress[entry.Name.ToString()] = (long)entry.Value;
        }

        return progress;
    }

    /// <summary>该帮会的成员数 + 上限。</summary>
    public async Task<(long Count, long Capacity)> GetCapacityAsync(string guildId)
    {
        var info = await GetInfoAsync(guildId);
        if (info == null)
        {
            return (0, 0);
        }

        var count = await _redis.Execute("guild.member_count",
            db => db.HashLengthAsync(Keys.Members(guildId)));
        return (count, Math.Max(1, _settings.MaxMembers) + Math.Max(0, info.MemberCapExtra));
    }

    /// <summary>修改帮会的成员上限附加位（帮会升级 / 运营发放时调用）。</summary>
    public async Task<bool> SetMemberCapExtraAsync(string guildId, long extra)
    {
        var info = await GetInfoAsync(guildId);
        if (info == null)
        {
            return false;
        }

        await _redis.Execute("guild.cap_extra",
            db => db.HashSetAsync(Keys.Info(guildId), "memberCapExtra", Math.Max(0, extra)));
        return true;
    }

    /// <summary>全部帮会 id（后台 / 列表用）。</summary>
    public async Task<List<string>> ListGuildIdsAsync()
    {
        var values = await _redis.Execute("guild.index", db => db.SetMembersAsync(Keys.Index));
        return values.Select(v => v.ToString()).ToList();
    }

    // ================================================================
    // 内部
    // ================================================================

    private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private static GuildInfo? ToInfo(string guildId, HashEntry[] values)
    {
        if (values.Length == 0)
        {
            return null;
        }

        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in values)
        {
            map[entry.Name.ToString()] = entry.Value.ToString();
        }

        return new GuildInfo(
            GuildId: guildId,
            Name: map.GetValueOrDefault("name", string.Empty),
            LeaderId: map.GetValueOrDefault("leaderId", string.Empty),
            Notice: map.GetValueOrDefault("notice", string.Empty),
            Level: int.TryParse(map.GetValueOrDefault("level"), out var level) ? level : 1,
            MemberCapExtra: long.TryParse(map.GetValueOrDefault("memberCapExtra"), out var extra) ? extra : 0,
            CreateTime: long.TryParse(map.GetValueOrDefault("createTime"), out var create) ? create : 0);
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
            Log.Error("guild: parse redis result failed: {0}", e.Message);
            return null;
        }
    }

    private static GuildResult Parse(RedisResult raw)
    {
        var obj = SafeParse(raw);
        if (obj == null)
        {
            Log.Error("guild: 无法解析的脚本返回值:{0}", raw);
            return GuildResult.Fail("internal");
        }

        var ok = obj.Value<bool?>("ok") ?? false;
        return ok ? GuildResult.Success() : GuildResult.Fail(obj.Value<string>("code") ?? "failed");
    }

    private static (GuildResult Result, string Data) ParseWithData(RedisResult raw)
    {
        var obj = SafeParse(raw);
        if (obj == null)
        {
            return (GuildResult.Fail("internal"), string.Empty);
        }

        var ok = obj.Value<bool?>("ok") ?? false;
        var data = obj.Value<string>("data") ?? string.Empty;
        return (ok ? GuildResult.Success() : GuildResult.Fail(obj.Value<string>("code") ?? "failed"), data);
    }

    // ================================================================
    // Lua：所有脚本返回 cjson 编码的单个字符串
    // ================================================================

    private const string CreateScript = """
        local infoKey = KEYS[1]
        local membersKey = KEYS[2]
        local nameKey = KEYS[3]
        local playerKey = KEYS[4]
        local indexKey = KEYS[5]

        local guildId = ARGV[1]
        local name = ARGV[2]
        local leaderId = ARGV[3]
        local now = ARGV[4]
        local capExtra = ARGV[5]

        if redis.call('EXISTS', playerKey) == 1 then
          return cjson.encode({ ok = false, code = 'already_in_guild' })
        end
        if redis.call('EXISTS', nameKey) == 1 then
          return cjson.encode({ ok = false, code = 'name_taken' })
        end

        redis.call('SET', nameKey, guildId)
        redis.call('HSET', infoKey,
          'guildId', guildId, 'name', name, 'leaderId', leaderId,
          'notice', '', 'level', 1, 'memberCapExtra', capExtra, 'createTime', now)
        redis.call('HSET', membersKey, leaderId, 3)
        redis.call('SET', playerKey, guildId)
        redis.call('SADD', indexKey, guildId)
        return cjson.encode({ ok = true })
        """;

    private const string RenameScript = """
        local membersKey = KEYS[1]
        local infoKey = KEYS[2]
        local oldNameKey = KEYS[3]
        local newNameKey = KEYS[4]

        local actorId = ARGV[1]
        local newName = ARGV[2]
        local guildId = ARGV[3]

        if tonumber(redis.call('HGET', membersKey, actorId) or '0') ~= 3 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        if newNameKey == oldNameKey then
          return cjson.encode({ ok = true })
        end
        if redis.call('EXISTS', newNameKey) == 1 then
          return cjson.encode({ ok = false, code = 'name_taken' })
        end

        redis.call('SET', newNameKey, guildId)
        redis.call('DEL', oldNameKey)
        redis.call('HSET', infoKey, 'name', newName)
        return cjson.encode({ ok = true })
        """;

    private const string SetNoticeScript = """
        local membersKey = KEYS[1]
        local infoKey = KEYS[2]

        if tonumber(redis.call('HGET', membersKey, ARGV[1]) or '0') ~= 3 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end

        redis.call('HSET', infoKey, 'notice', ARGV[2])
        return cjson.encode({ ok = true })
        """;

    private const string InviteScript = """
        local membersKey = KEYS[1]
        local targetPlayerKey = KEYS[2]
        local invitesKey = KEYS[3]

        local actorId = ARGV[1]
        local guildId = ARGV[2]
        local maxInvites = tonumber(ARGV[3])

        if tonumber(redis.call('HGET', membersKey, actorId) or '0') < 2 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        if redis.call('EXISTS', targetPlayerKey) == 1 then
          return cjson.encode({ ok = false, code = 'target_in_guild' })
        end
        if redis.call('HLEN', invitesKey) >= maxInvites then
          return cjson.encode({ ok = false, code = 'invite_full' })
        end

        redis.call('HSET', invitesKey, guildId, actorId)
        return cjson.encode({ ok = true })
        """;

    private const string ApplyScript = """
        local infoKey = KEYS[1]
        local playerKey = KEYS[2]
        local appliesKey = KEYS[3]

        local userId = ARGV[1]
        local now = ARGV[2]
        local maxApplies = tonumber(ARGV[3])

        if redis.call('EXISTS', infoKey) == 0 then
          return cjson.encode({ ok = false, code = 'not_found' })
        end
        if redis.call('EXISTS', playerKey) == 1 then
          return cjson.encode({ ok = false, code = 'already_in_guild' })
        end
        if redis.call('HLEN', appliesKey) >= maxApplies then
          return cjson.encode({ ok = false, code = 'apply_full' })
        end

        redis.call('HSET', appliesKey, userId, now)
        return cjson.encode({ ok = true })
        """;

    // 审批：校验权限 + 申请人不在别的帮会 + 成员上限（全局默认 + 帮会附加）后入帮
    private const string ApproveScript = """
        local membersKey = KEYS[1]
        local appliesKey = KEYS[2]
        local applicantPlayerKey = KEYS[3]
        local infoKey = KEYS[4]

        local actorId = ARGV[1]
        local applicantId = ARGV[2]
        local guildId = ARGV[3]
        local maxMembers = tonumber(ARGV[4])

        if tonumber(redis.call('HGET', membersKey, actorId) or '0') < 2 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        if redis.call('HEXISTS', appliesKey, applicantId) == 0 then
          return cjson.encode({ ok = false, code = 'no_request' })
        end
        if redis.call('EXISTS', applicantPlayerKey) == 1 then
          redis.call('HDEL', appliesKey, applicantId)
          return cjson.encode({ ok = false, code = 'target_in_guild' })
        end

        local extra = tonumber(redis.call('HGET', infoKey, 'memberCapExtra') or '0')
        if redis.call('HLEN', membersKey) >= maxMembers + extra then
          return cjson.encode({ ok = false, code = 'member_full' })
        end

        redis.call('HSET', membersKey, applicantId, 1)
        redis.call('SET', applicantPlayerKey, guildId)
        redis.call('HDEL', appliesKey, applicantId)
        return cjson.encode({ ok = true })
        """;

    private const string RejectApplyScript = """
        local membersKey = KEYS[1]
        local appliesKey = KEYS[2]

        local actorId = ARGV[1]
        local applicantId = ARGV[2]

        -- 申请人自己撤回（actorId == applicantId）也允许
        if actorId ~= applicantId and tonumber(redis.call('HGET', membersKey, actorId) or '0') < 2 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end

        local removed = redis.call('HDEL', appliesKey, applicantId)
        if removed == 0 then
          return cjson.encode({ ok = false, code = 'no_request' })
        end
        return cjson.encode({ ok = true })
        """;

    private const string AcceptInviteScript = """
        local invitesKey = KEYS[1]
        local membersKey = KEYS[2]
        local infoKey = KEYS[3]
        local playerKey = KEYS[4]

        local userId = ARGV[1]
        local guildId = ARGV[2]
        local maxMembers = tonumber(ARGV[3])

        if redis.call('HEXISTS', invitesKey, guildId) == 0 then
          return cjson.encode({ ok = false, code = 'no_request' })
        end
        -- 帮会可能已解散：顺手清掉这条过期邀请（邀请没有 TTL，靠这里自愈）
        if redis.call('EXISTS', infoKey) == 0 then
          redis.call('HDEL', invitesKey, guildId)
          return cjson.encode({ ok = false, code = 'not_found' })
        end
        if redis.call('EXISTS', playerKey) == 1 then
          redis.call('HDEL', invitesKey, guildId)
          return cjson.encode({ ok = false, code = 'already_in_guild' })
        end

        local extra = tonumber(redis.call('HGET', infoKey, 'memberCapExtra') or '0')
        if redis.call('HLEN', membersKey) >= maxMembers + extra then
          return cjson.encode({ ok = false, code = 'member_full' })
        end

        redis.call('HSET', membersKey, userId, 1)
        redis.call('SET', playerKey, guildId)
        redis.call('HDEL', invitesKey, guildId)
        return cjson.encode({ ok = true })
        """;

    private const string RejectInviteScript = """
        return redis.call('HDEL', KEYS[1], ARGV[1])
        """;

    private const string LeaveScript = """
        local membersKey = KEYS[1]
        local playerKey = KEYS[2]

        local userId = ARGV[1]
        local role = tonumber(redis.call('HGET', membersKey, userId) or '0')

        if role == 0 then
          return cjson.encode({ ok = false, code = 'not_in_guild' })
        end
        if role >= 3 then
          return cjson.encode({ ok = false, code = 'leader_must_transfer' })
        end

        redis.call('HDEL', membersKey, userId)
        redis.call('DEL', playerKey)
        return cjson.encode({ ok = true })
        """;

    private const string KickScript = """
        local membersKey = KEYS[1]
        local targetPlayerKey = KEYS[2]

        local actorId = ARGV[1]
        local targetId = ARGV[2]

        local actor = tonumber(redis.call('HGET', membersKey, actorId) or '0')
        local target = tonumber(redis.call('HGET', membersKey, targetId) or '0')

        if actor < 2 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        if target == 0 then
          return cjson.encode({ ok = false, code = 'target_not_in_guild' })
        end
        -- 不能踢职位不低于自己的人（副帮主踢不了副帮主 / 帮主）
        if actor <= target then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end

        redis.call('HDEL', membersKey, targetId)
        redis.call('DEL', targetPlayerKey)
        return cjson.encode({ ok = true })
        """;

    private const string SetRoleScript = """
        local membersKey = KEYS[1]

        local actorId = ARGV[1]
        local targetId = ARGV[2]
        local role = tonumber(ARGV[3])
        local maxOfficers = tonumber(ARGV[4])

        if tonumber(redis.call('HGET', membersKey, actorId) or '0') ~= 3 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        local target = tonumber(redis.call('HGET', membersKey, targetId) or '0')
        if target == 0 then
          return cjson.encode({ ok = false, code = 'target_not_in_guild' })
        end
        if target >= 3 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        if role < 1 or role >= 3 then
          return cjson.encode({ ok = false, code = 'invalid' })
        end

        if role == 2 then
          local all = redis.call('HGETALL', membersKey)
          local officers = 0
          for i = 2, #all, 2 do
            if tonumber(all[i]) == 2 then
              officers = officers + 1
            end
          end
          if officers >= maxOfficers then
            return cjson.encode({ ok = false, code = 'officer_full' })
          end
        end

        redis.call('HSET', membersKey, targetId, role)
        return cjson.encode({ ok = true })
        """;

    private const string TransferScript = """
        local membersKey = KEYS[1]
        local infoKey = KEYS[2]

        local actorId = ARGV[1]
        local targetId = ARGV[2]

        if tonumber(redis.call('HGET', membersKey, actorId) or '0') ~= 3 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        if tonumber(redis.call('HGET', membersKey, targetId) or '0') == 0 then
          return cjson.encode({ ok = false, code = 'target_not_in_guild' })
        end

        redis.call('HSET', membersKey, actorId, 2)
        redis.call('HSET', membersKey, targetId, 3)
        redis.call('HSET', infoKey, 'leaderId', targetId)
        return cjson.encode({ ok = true })
        """;

    private const string DisbandScript = """
        local infoKey = KEYS[1]
        local membersKey = KEYS[2]
        local appliesKey = KEYS[3]
        local tasksKey = KEYS[4]
        local nameKey = KEYS[5]
        local indexKey = KEYS[6]

        local actorId = ARGV[1]
        local guildId = ARGV[2]
        local playerPrefix = ARGV[3]
        local taskMemberPrefix = ARGV[4]
        local taskProgressPrefix = ARGV[5]

        if tonumber(redis.call('HGET', membersKey, actorId) or '0') ~= 3 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end

        local members = redis.call('HKEYS', membersKey)
        local tasks = redis.call('HKEYS', tasksKey)

        for i = 1, #tasks do
          redis.call('DEL', taskMemberPrefix .. tasks[i])
          redis.call('DEL', taskProgressPrefix .. tasks[i])
        end
        for i = 1, #members do
          redis.call('DEL', playerPrefix .. members[i])
        end

        redis.call('DEL', infoKey, membersKey, appliesKey, tasksKey, nameKey)
        redis.call('SREM', indexKey, guildId)

        return cjson.encode({ ok = true, data = table.concat(members, '|') })
        """;

    private const string PublishTaskScript = """
        local membersKey = KEYS[1]
        local tasksKey = KEYS[2]

        local actorId = ARGV[1]
        local taskId = ARGV[2]
        local payload = ARGV[3]
        local officerAllowed = tonumber(ARGV[4])

        local role = tonumber(redis.call('HGET', membersKey, actorId) or '0')
        if role < 3 and not (officerAllowed == 1 and role >= 2) then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        if redis.call('HEXISTS', tasksKey, taskId) == 1 then
          return cjson.encode({ ok = false, code = 'task_published' })
        end

        redis.call('HSET', tasksKey, taskId, payload)
        return cjson.encode({ ok = true })
        """;

    private const string CancelTaskScript = """
        local membersKey = KEYS[1]
        local tasksKey = KEYS[2]
        local taskMembersKey = KEYS[3]
        local taskProgressKey = KEYS[4]

        local actorId = ARGV[1]
        local taskId = ARGV[2]

        if tonumber(redis.call('HGET', membersKey, actorId) or '0') ~= 3 then
          return cjson.encode({ ok = false, code = 'no_permission' })
        end
        if redis.call('HEXISTS', tasksKey, taskId) == 0 then
          return cjson.encode({ ok = false, code = 'task_not_published' })
        end

        redis.call('HDEL', tasksKey, taskId)
        redis.call('DEL', taskMembersKey)
        redis.call('DEL', taskProgressKey)
        return cjson.encode({ ok = true })
        """;

    private const string AcceptTaskScript = """
        local membersKey = KEYS[1]
        local tasksKey = KEYS[2]
        local taskMembersKey = KEYS[3]
        local taskProgressKey = KEYS[4]

        local userId = ARGV[1]
        local taskId = ARGV[2]

        if tonumber(redis.call('HGET', membersKey, userId) or '0') == 0 then
          return cjson.encode({ ok = false, code = 'not_in_guild' })
        end
        if redis.call('HEXISTS', tasksKey, taskId) == 0 then
          return cjson.encode({ ok = false, code = 'task_not_published' })
        end
        if redis.call('SISMEMBER', taskMembersKey, userId) == 1 then
          return cjson.encode({ ok = false, code = 'task_already_accepted' })
        end

        redis.call('SADD', taskMembersKey, userId)
        redis.call('HSET', taskProgressKey, userId, 0)
        return cjson.encode({ ok = true })
        """;

    private const string AddTaskProgressScript = """
        local tasksKey = KEYS[1]
        local taskMembersKey = KEYS[2]
        local taskProgressKey = KEYS[3]

        local memberId = ARGV[1]
        local delta = tonumber(ARGV[2])
        local target = tonumber(ARGV[3])
        local shared = tonumber(ARGV[4])

        if redis.call('SISMEMBER', taskMembersKey, memberId) == 0 then
          return cjson.encode({ ok = false, code = 'task_not_accepted', progress = 0, completed = 0 })
        end

        -- 共享任务把进度记在保留键 '_' 上（全帮一份）
        local field = memberId
        if shared == 1 then
          field = '_'
        end

        local progress = redis.call('HINCRBY', taskProgressKey, field, delta)
        local completed = 0
        if progress >= target then
          completed = 1
        end
        return cjson.encode({ ok = true, progress = progress, completed = completed })
        """;
}
