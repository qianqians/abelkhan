namespace quest;

/// <summary>
/// 任务系统用到的 Redis key。
///
/// 全部带 <c>{quest}</c> hash tag：一次事件可能同时改「事件状态 + 多个任务进度」，
/// Redis Cluster 下必须落同一 slot。
/// </summary>
public sealed class QuestKeys
{
    private readonly string _prefix;

    public QuestKeys(string? prefix = null)
    {
        prefix ??= string.Empty;
        if (prefix.Contains('{') || prefix.Contains('}'))
        {
            throw new ArgumentException("QuestSettings.KeyPrefix 不能包含 '{' 或 '}'（会破坏 hash tag）", nameof(prefix));
        }

        if (prefix.Length > 0 && !prefix.EndsWith(':'))
        {
            prefix += ":";
        }

        _prefix = prefix;
    }

    public string Prefix => _prefix;

    public string Base => _prefix + "quest:{quest}:";

    /// <summary>玩家事件状态：<c>type|key → value</c>（触发 / 完成条件的数据源）。</summary>
    public string Event(string userId) => Base + "event:" + userId;

    /// <summary>进行中的任务：<c>questId → JSON</c>。</summary>
    public string Active(string userId) => Base + "active:" + userId;

    /// <summary>已完成的任务：<c>questId → 完成时间</c>。</summary>
    public string Done(string userId) => Base + "done:" + userId;

    /// <summary>任务进度：<c>questId|type|key → 进度</c>。</summary>
    public string Progress(string userId) => Base + "progress:" + userId;
}
