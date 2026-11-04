namespace guild;

/// <summary>
/// 帮会任务定义（静态配置）。游戏在启动时把它们注册进 <see cref="GuildTaskCatalog"/>，
/// 帮主从目录里挑任务发布；新建帮会时自动发布 <see cref="GuildSettings.DefaultTaskIds"/> 里那些。
/// </summary>
/// <param name="TaskId">唯一 id。</param>
/// <param name="Title">标题。</param>
/// <param name="Description">描述（给玩家看的目标说明）。</param>
/// <param name="Target">进度达到它就完成；进度由游戏上报。</param>
/// <param name="PerMember">true = 每人各自领取、各自进度；false = 全帮共享一份进度。</param>
/// <param name="OfficerCanPublish">是否允许副帮主发布（默认仅帮主）。</param>
/// <param name="MinGuildLevel">帮会等级下限。</param>
/// <param name="RewardKey">奖励标识：框架不解析，完成时原样交给游戏钩子。</param>
public sealed record GuildTaskDefinition(
    string TaskId,
    string Title,
    string Description = "",
    long Target = 1,
    bool PerMember = true,
    bool OfficerCanPublish = false,
    int MinGuildLevel = 0,
    string RewardKey = "")
{
    public long NormalizedTarget => Target <= 0 ? 1 : Target;
}

/// <summary>帮会任务目录查询接口（仿 hub.inventory.IItemCatalog）。</summary>
public interface IGuildTaskCatalog
{
    bool TryGet(string taskId, out GuildTaskDefinition? definition);

    IReadOnlyCollection<GuildTaskDefinition> All { get; }
}

/// <summary>
/// 内存里的帮会任务目录。游戏可以从配置表 / 数据库加载后注册进来；
/// 热重载时 <see cref="Clear"/> 再批量注册即可。
/// </summary>
public sealed class GuildTaskCatalog : IGuildTaskCatalog
{
    private readonly Dictionary<string, GuildTaskDefinition> _definitions = new(StringComparer.Ordinal);

    /// <summary>空目录。</summary>
    public static GuildTaskCatalog Empty { get; } = new();

    public int Count => _definitions.Count;

    public IReadOnlyCollection<GuildTaskDefinition> All => _definitions.Values;

    public void Register(GuildTaskDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (string.IsNullOrWhiteSpace(definition.TaskId))
        {
            throw new ArgumentException("帮会任务 task_id 不能为空", nameof(definition));
        }

        _definitions[definition.TaskId] = definition;
    }

    public void RegisterRange(IEnumerable<GuildTaskDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        foreach (var definition in definitions)
        {
            Register(definition);
        }
    }

    public void Clear() => _definitions.Clear();

    public bool TryGet(string taskId, out GuildTaskDefinition? definition)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            definition = null;
            return false;
        }

        return _definitions.TryGetValue(taskId, out definition);
    }
}
