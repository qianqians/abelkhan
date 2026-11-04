namespace guild;

/// <summary>
/// 帮会系统运行参数。
///
/// 帮会数据全在 Redis（Lua 原子），所以这里的上限都是「权威值」：
/// 成员上限 = <see cref="MaxMembers"/> + 帮会自身的 memberCapExtra（帮会升级时加，可扩展）。
/// </summary>
public sealed class GuildSettings
{
    /// <summary>Redis key 前缀。多游戏共用一个 Redis 时用它隔离，不能包含 '{' / '}'。</summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>全局默认成员上限（不含帮会等级附加）。</summary>
    public int MaxMembers { get; set; } = 50;

    /// <summary>副帮主上限。</summary>
    public int MaxOfficers { get; set; } = 3;

    /// <summary>待审批申请上限（防刷）。</summary>
    public int MaxApplications { get; set; } = 50;

    /// <summary>单个玩家能同时持有的帮会邀请上限。</summary>
    public int MaxInvites { get; set; } = 10;

    public int NameMinLength { get; set; } = 2;
    public int NameMaxLength { get; set; } = 16;

    /// <summary>帮会公告最大长度。</summary>
    public int NoticeMaxLength { get; set; } = 200;

    /// <summary>新建帮会时自动发布的任务 id（必须在 <see cref="GuildTaskCatalog"/> 里存在）。</summary>
    public List<string> DefaultTaskIds { get; set; } = new();

    /// <summary>
    /// 校验并归一化帮会名：去掉首尾空白，拒绝空、超长、控制字符和花括号（花括号会干扰 Redis
    /// Cluster 的 hash tag 解析）。返回 null 表示合法。
    /// </summary>
    public string? NormalizeName(string? name, out string normalized)
    {
        normalized = (name ?? string.Empty).Trim();
        if (normalized.Length < Math.Max(1, NameMinLength) ||
            normalized.Length > Math.Max(NameMinLength, NameMaxLength))
        {
            return GuildCodes.NameInvalid;
        }

        foreach (var ch in normalized)
        {
            if (char.IsControl(ch) || ch is '{' or '}' or '|')
            {
                return GuildCodes.NameInvalid;
            }
        }

        return null;
    }

    /// <summary>名字做唯一性索引时用的归一化形式（大小写不敏感 + 去首尾空白）。</summary>
    public static string NameKey(string normalizedName) => normalizedName.ToLowerInvariant();
}
