namespace quest;

/// <summary>
/// 一条任务的定义。三条条件各自负责一件事：
/// <list type="bullet">
/// <item>触发条件：对「玩家事件状态」求值，满足 + 前置任务都完成 → 可接（或自动接）。</item>
/// <item>进度条件：该任务自己的计数器，任务进行中时被匹配到的事件推进。</item>
/// <item>完成条件：门槛，不累计，完成时对事件状态求值（满足不了再问游戏钩子）。</item>
/// </list>
/// </summary>
public sealed record QuestDefinition(
    string QuestId,
    string Title,
    string Description = "",
    bool AutoAccept = true,
    IReadOnlyList<string>? RequireQuests = null,
    IReadOnlyList<QuestCondition>? TriggerConditions = null,
    IReadOnlyList<QuestCondition>? ProgressConditions = null,
    IReadOnlyList<QuestCondition>? CompletionConditions = null,
    IReadOnlyList<string>? NextQuestIds = null,
    string RewardKey = "")
{
    public IReadOnlyList<string> Requires { get; } = RequireQuests ?? Array.Empty<string>();

    public IReadOnlyList<QuestCondition> Triggers { get; } = TriggerConditions ?? Array.Empty<QuestCondition>();

    public IReadOnlyList<QuestCondition> Progress { get; } = ProgressConditions ?? Array.Empty<QuestCondition>();

    public IReadOnlyList<QuestCondition> Completions { get; } =
        CompletionConditions ?? Array.Empty<QuestCondition>();

    public IReadOnlyList<string> NextQuests { get; } = NextQuestIds ?? Array.Empty<string>();

    /// <summary>是否有可累计的进度条件（没有的话接取即完成，等价于一次性任务）。</summary>
    public bool HasProgress => Progress.Count > 0;
}

/// <summary>任务目录查询接口。</summary>
public interface IQuestCatalog
{
    bool TryGet(string questId, out QuestDefinition? definition);

    IReadOnlyCollection<QuestDefinition> All { get; }

    /// <summary>
    /// 「根任务」：没有被任何任务的 <c>NextQuestIds</c> 引用过的任务。
    /// 只有根任务会被全局触发扫描自动接取；链上的后继任务由完成时按顺序挑，避免同一条链被激活两次。
    /// </summary>
    IReadOnlyCollection<QuestDefinition> Roots { get; }
}

/// <summary>
/// 内存任务目录。游戏从配置表 / JSON / 数据库读进来后注册，
/// 热重载时 <see cref="Clear"/> 再批量注册即可（会自动重算根任务集合）。
/// </summary>
public sealed class QuestCatalog : IQuestCatalog
{
    private readonly Dictionary<string, QuestDefinition> _definitions = new(StringComparer.Ordinal);
    private readonly HashSet<string> _chained = new(StringComparer.Ordinal);
    private List<QuestDefinition> _roots = new();

    public static QuestCatalog Empty { get; } = new();

    public int Count => _definitions.Count;

    public IReadOnlyCollection<QuestDefinition> All => _definitions.Values;

    public IReadOnlyCollection<QuestDefinition> Roots => _roots;

    public void Register(QuestDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (string.IsNullOrWhiteSpace(definition.QuestId))
        {
            throw new ArgumentException("任务 quest_id 不能为空", nameof(definition));
        }

        _definitions[definition.QuestId] = definition;
        Rebuild();
    }

    public void RegisterRange(IEnumerable<QuestDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        foreach (var definition in definitions)
        {
            ArgumentNullException.ThrowIfNull(definition);
            if (string.IsNullOrWhiteSpace(definition.QuestId))
            {
                throw new ArgumentException("任务 quest_id 不能为空", nameof(definition));
            }

            _definitions[definition.QuestId] = definition;
        }

        Rebuild();
    }

    public void Clear()
    {
        _definitions.Clear();
        _chained.Clear();
        _roots = new List<QuestDefinition>();
    }

    public bool TryGet(string questId, out QuestDefinition? definition)
    {
        if (string.IsNullOrWhiteSpace(questId))
        {
            definition = null;
            return false;
        }

        return _definitions.TryGetValue(questId, out definition);
    }

    private void Rebuild()
    {
        _chained.Clear();
        foreach (var definition in _definitions.Values)
        {
            foreach (var next in definition.NextQuests)
            {
                _chained.Add(next);
            }
        }

        _roots = _definitions.Values.Where(d => !_chained.Contains(d.QuestId)).ToList();
    }
}
