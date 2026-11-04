namespace quest;

/// <summary>
/// 任务判定的纯函数集合（不碰 Redis、不碰 Player），方便单独测试，也避免判定逻辑散落在服务里。
/// </summary>
public static class QuestRules
{
    /// <summary>一条条件是否满足：Flag 看有没有发生过，其余看是否达到目标值。</summary>
    public static bool ConditionSatisfied(QuestCondition condition, long value) =>
        condition.Mode == QuestConditionMode.Flag ? value >= 1 : value >= condition.Target;

    /// <summary>触发条件是否全部满足（对玩家事件状态求值）。</summary>
    public static bool TriggersSatisfied(QuestDefinition definition,
        IReadOnlyDictionary<string, long> events)
    {
        foreach (var condition in definition.Triggers)
        {
            if (!ConditionSatisfied(condition, events.GetValueOrDefault(condition.ConditionKey)))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>能不能接：未完成、未进行中、前置任务都已完成、触发条件满足。</summary>
    public static bool CanBeAccepted(QuestDefinition definition,
        IReadOnlyDictionary<string, long> events,
        IReadOnlyCollection<string> done,
        IReadOnlyCollection<string> active)
    {
        if (Contains(done, definition.QuestId) || Contains(active, definition.QuestId))
        {
            return false;
        }

        foreach (var require in definition.Requires)
        {
            if (!Contains(done, require))
            {
                return false;
            }
        }

        return TriggersSatisfied(definition, events);
    }

    /// <summary>进度条件是否全部达到目标（没有进度条件的任务视为直接满）。</summary>
    public static bool ProgressFull(QuestDefinition definition,
        IReadOnlyDictionary<string, long> progress)
    {
        foreach (var condition in definition.Progress)
        {
            if (progress.GetValueOrDefault(condition.ConditionKey) < condition.Target)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 「多后继 + 条件挑一」：按 <see cref="QuestDefinition.NextQuests"/> 的顺序，
    /// 返回第一个满足接取条件的后继任务 id；都不满足返回 null。
    /// </summary>
    public static string? PickNext(QuestDefinition completed, IQuestCatalog catalog,
        IReadOnlyDictionary<string, long> events,
        IReadOnlyCollection<string> done,
        IReadOnlyCollection<string> active)
    {
        foreach (var nextId in completed.NextQuests)
        {
            if (!catalog.TryGet(nextId, out var next) || next == null)
            {
                continue;
            }

            if (CanBeAccepted(next, events, done, active))
            {
                return nextId;
            }
        }

        return null;
    }

    private static bool Contains(IReadOnlyCollection<string> source, string value)
    {
        foreach (var item in source)
        {
            if (string.Equals(item, value, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
