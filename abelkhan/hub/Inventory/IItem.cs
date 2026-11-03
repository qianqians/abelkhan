namespace hub.inventory;

/// <summary>
/// 物品抽象。背包只依赖这个接口，不关心物品是堆叠道具、装备还是任务道具。
///
/// 约定：
/// 1. 实现必须尽量不可变；<see cref="WithCount"/> 返回新对象而不是修改自身。
///    背包的快照 / 事务回滚依赖这一点（只复制引用，不做深拷贝）。
/// 2. <see cref="Count"/> 表示这一份物品堆叠的数量。装备等不可堆叠物品应使用 Count = 1。
/// 3. 同一个 <see cref="ItemId"/> 只有在 <see cref="Stackable"/> 为 true、
///    且 <see cref="MaxStack"/> / <see cref="Type"/> 都相同时才允许合并。
/// </summary>
public interface IItem
{
    /// <summary>物品配置 id（对应 item_id）。</summary>
    string ItemId { get; }

    /// <summary>当前堆叠数量，必须大于 0。</summary>
    long Count { get; }

    /// <summary>单格最大堆叠数量。&lt;= 0 按无上限处理。</summary>
    int MaxStack { get; }

    /// <summary>物品大类。</summary>
    ItemType Type { get; }

    /// <summary>是否允许与同 id 物品堆叠。</summary>
    bool Stackable { get; }

    /// <summary>返回一个只有数量不同的新物品实例。</summary>
    IItem WithCount(long count);
}
