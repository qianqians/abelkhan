namespace hub.inventory;

/// <summary>
/// 背包只读视图。客户端同步、UI 展示、任务统计只依赖这个接口，避免误改背包。
/// </summary>
public interface IReadOnlyBag
{
    /// <summary>背包总格数。</summary>
    int Capacity { get; }

    /// <summary>已占用的格数。</summary>
    int OccupiedSlots { get; }

    /// <summary>每次成功变更后 +1，可用于客户端判断是否需要刷新。</summary>
    long Version { get; }

    /// <summary>按格位读取物品；空位返回 null。下标越界抛 <see cref="ArgumentOutOfRangeException"/>。</summary>
    IItem? this[int slot] { get; }

    /// <summary>只读格位视图（长度 = <see cref="Capacity"/>）。</summary>
    IReadOnlyList<IItem?> Slots { get; }

    /// <summary>统计某个 item_id 的总数量（跨多个格位累加）。</summary>
    long GetItemCount(string itemId);

    /// <summary>是否至少拥有指定数量。</summary>
    bool Contains(string itemId, long count = 1);

    /// <summary>复制一份当前背包快照。</summary>
    BagSnapshot Snapshot();
}
