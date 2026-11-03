using System.Diagnostics.CodeAnalysis;

namespace hub.inventory;

/// <summary>
/// 物品配置查询接口。背包用它把「item_id + 数量」补全成具体物品。
/// </summary>
public interface IItemCatalog
{
    /// <summary>按 item_id 查配置；查不到返回 false。</summary>
    bool TryGet(string itemId, [NotNullWhen(true)] out ItemDefinition? definition);
}

/// <summary>
/// 内存物品配置表。游戏可以从配置表 / 数据库加载后注册进来。
///
/// 注意：如果给背包传了非空 catalog，那么添加未知 item_id 会返回
/// <see cref="BagError.UnknownItem"/>；不传 catalog 时则允许任意 id
/// （MaxStack 按无上限处理），方便框架层先跑通、后续再接配置。
/// </summary>
public sealed class ItemCatalog : IItemCatalog
{
    private readonly Dictionary<string, ItemDefinition> _definitions = new(StringComparer.Ordinal);

    /// <summary>空配置表。</summary>
    public static ItemCatalog Empty { get; } = new();

    public int Count => _definitions.Count;

    /// <summary>注册 / 覆盖一条物品配置。</summary>
    public void Register(ItemDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        _definitions[definition.ItemId] = definition;
    }

    /// <summary>批量注册，重复的 item_id 以后注册的为准。</summary>
    public void RegisterRange(IEnumerable<ItemDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        foreach (var definition in definitions)
        {
            Register(definition);
        }
    }

    /// <summary>清空配置（热重载时使用）。</summary>
    public void Clear() => _definitions.Clear();

    public bool TryGet(string itemId, [NotNullWhen(true)] out ItemDefinition? definition)
    {
        if (string.IsNullOrWhiteSpace(itemId) ||
            !_definitions.TryGetValue(itemId, out var found))
        {
            definition = null;
            return false;
        }

        definition = found;
        return true;
    }

    /// <summary>按 item_id 创建物品；未配置时使用无上限堆叠的默认实现。</summary>
    public ItemStack Create(string itemId, long count) =>
        TryGet(itemId, out var definition) ? definition.CreateStack(count) : new ItemStack(itemId, count);
}
