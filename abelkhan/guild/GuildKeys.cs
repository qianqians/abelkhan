namespace guild;

/// <summary>
/// 帮会系统用到的 Redis key。
///
/// 所有 key 共用 <c>{guild}</c> hash tag：一次操作要原子地读写「帮会信息 + 成员表 +
/// 玩家身上的帮会索引 + 名字索引」等多个 key，Redis Cluster 下必须落同一 slot。
/// </summary>
public sealed class GuildKeys
{
    private readonly string _prefix;

    public GuildKeys(string? prefix = null)
    {
        prefix ??= string.Empty;
        if (prefix.Contains('{') || prefix.Contains('}'))
        {
            throw new ArgumentException("GuildSettings.KeyPrefix 不能包含 '{' 或 '}'（会破坏 hash tag）", nameof(prefix));
        }

        if (prefix.Length > 0 && !prefix.EndsWith(':'))
        {
            prefix += ":";
        }

        _prefix = prefix;
    }

    public string Prefix => _prefix;

    /// <summary>Lua 里按 guildId / userId 拼 key 用的公共前缀。</summary>
    public string Base => _prefix + "guild:{guild}:";

    public string Info(string guildId) => Base + "info:" + guildId;

    public string Members(string guildId) => Base + "members:" + guildId;

    /// <summary>帮会名 → guildId（唯一性索引）。</summary>
    public string NameIndex(string nameKey) => Base + "name:" + nameKey;

    /// <summary>userId → guildId（一人一会）。</summary>
    public string PlayerGuild(string userId) => Base + "player:" + userId;

    /// <summary>玩家收到的邀请：guildId → inviterId。</summary>
    public string Invites(string userId) => Base + "invites:" + userId;

    /// <summary>帮会的待审批申请：userId → 申请时间。</summary>
    public string Applies(string guildId) => Base + "applies:" + guildId;

    /// <summary>已发布任务：taskId → JSON。</summary>
    public string Tasks(string guildId) => Base + "tasks:" + guildId;

    /// <summary>任务领取名单。</summary>
    public string TaskMembers(string guildId, string taskId) => Base + "taskMembers:" + guildId + ":" + taskId;

    /// <summary>任务进度：memberId → 进度（共享任务用保留键 <c>_</c>）。</summary>
    public string TaskProgress(string guildId, string taskId) => Base + "taskProgress:" + guildId + ":" + taskId;

    /// <summary>全部帮会 id（列表 / 后台用）。</summary>
    public string Index => Base + "index";

    /// <summary>Lua 里拼「玩家帮会索引」key 用的前缀。</summary>
    public string PlayerGuildPrefix => Base + "player:";

    /// <summary>Lua 里拼任务 key 用的前缀。</summary>
    public string TaskMembersPrefix(string guildId) => Base + "taskMembers:" + guildId + ":";

    public string TaskProgressPrefix(string guildId) => Base + "taskProgress:" + guildId + ":";
}
