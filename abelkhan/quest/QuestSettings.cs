namespace quest;

/// <summary>任务系统运行参数。</summary>
public sealed class QuestSettings
{
    /// <summary>Redis key 前缀。多游戏共用一个 Redis 时用它隔离，不能包含 '{' / '}'。</summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>同时进行中的任务上限（防配置错误把玩家刷满）。</summary>
    public int MaxActiveQuests { get; set; } = 50;

    /// <summary>单个任务最多允许多少条完成记录（超出后旧的会被裁剪，避免 hash 无限增长）。</summary>
    public int MaxDoneQuests { get; set; } = 500;

    /// <summary>
    /// 进度全满 + 完成门槛都满足时是否自动完成。
    /// 默认 false：留给玩家点「提交」（很多游戏需要回去找 NPC 交任务的仪式感）。
    /// </summary>
    public bool AutoCompleteWhenReady { get; set; }
}
