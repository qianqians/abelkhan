namespace quest;

/// <summary>
/// 条件在「事件状态」上的更新方式。这是「纯自定义事件」能不能落地的关键：
/// 等级 / 战力这类是**绝对值**，不是 +1，没有 <see cref="Max"/> 就只能让游戏自己算差值。
/// </summary>
public enum QuestConditionMode
{
    /// <summary>累加：value += amount（击杀 10 只）。</summary>
    Count = 1,

    /// <summary>取最大值：value = max(value, amount)（等级达到 10，上报的是等级本身）。</summary>
    Max = 2,

    /// <summary>布尔：只要发生过就为真（和 NPC 对过话）。</summary>
    Flag = 3,
}

/// <summary>
/// 一个任务条件：框架只认 (type, key, mode, target)，语义完全由游戏定义。
/// </summary>
/// <param name="Type">事件类型，如 <c>kill</c> / <c>level</c> / <c>talk</c>。</param>
/// <param name="Key">事件 key，如 <c>goblin</c> / <c>npc_7</c>（<c>level</c> 这种可以用空串）。</param>
/// <param name="Mode">更新方式。</param>
/// <param name="Target">目标值：进度条件达到它就满；触发 / 完成条件看它是否满足。</param>
public sealed record QuestCondition(
    string Type,
    string Key,
    QuestConditionMode Mode = QuestConditionMode.Count,
    long Target = 1)
{
    /// <summary>存储 / 比较用的键：<c>type|key</c>。</summary>
    public string ConditionKey => StoreKey(Type, Key);

    public static string StoreKey(string type, string key) => type + "|" + key;
}

/// <summary>任务状态（对客户端的展示用）。</summary>
public static class QuestStatus
{
    public const string Available = "available"; // 可接但没接
    public const string Active = "active";       // 进行中
    public const string Completed = "completed"; // 已完成
}

/// <summary>进行中任务的持久化状态。</summary>
public sealed class QuestState
{
    public string QuestId { get; set; } = string.Empty;

    public long AcceptedAt { get; set; }

    /// <summary>进度条件键（<c>type|key</c>）→ 当前进度。</summary>
    public Dictionary<string, long> Progress { get; set; } = new(StringComparer.Ordinal);

    public static QuestState Create(string questId, long acceptedAt) =>
        new() { QuestId = questId, AcceptedAt = acceptedAt };
}

/// <summary>一条进度条件的当前值（给客户端）。</summary>
public sealed record QuestProgressView(string Type, string Key, long Current, long Target)
{
    public bool Done => Current >= Target;
}

/// <summary>任务完成回调的载荷（奖励标识原样带回，由游戏发奖）。</summary>
public sealed record QuestCompletion(
    string UserId,
    string QuestId,
    string Title,
    string RewardKey,
    long CompletedAt);

/// <summary>任务操作结果码。</summary>
public static class QuestCodes
{
    public const string Ok = "ok";
    public const string UnknownQuest = "unknown_quest";
    public const string AlreadyActive = "already_active";
    public const string AlreadyDone = "already_done";
    public const string NotActive = "not_active";
    public const string NotAvailable = "not_available";
    public const string ProgressNotFull = "progress_not_full";
    public const string GateNotSatisfied = "gate_not_satisfied";
    public const string TooManyActive = "too_many_active";
    public const string Invalid = "invalid";
}

/// <summary>任务操作结果。</summary>
public sealed record QuestResult(bool Ok, string Code = QuestCodes.Ok)
{
    public static QuestResult Success() => new(true);

    public static QuestResult Fail(string code) => new(false, code);
}
