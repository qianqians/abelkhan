namespace hub.inventory;

/// <summary>背包操作的失败原因。成功时为 <see cref="None"/>。</summary>
public enum BagError
{
    None = 0,

    /// <summary>参数为空 / 非法。</summary>
    InvalidArgument,

    /// <summary>数量必须大于 0。</summary>
    InvalidCount,

    /// <summary>配置表里没有这个 item_id。</summary>
    UnknownItem,

    /// <summary>背包剩余格子不足以放下全部物品。</summary>
    NotEnoughSpace,

    /// <summary>背包里的物品数量不足。</summary>
    NotEnoughItems,

    /// <summary>格位下标越界。</summary>
    SlotOutOfRange,

    /// <summary>目标格位是空的。</summary>
    SlotEmpty,

    /// <summary>存档快照损坏或容量不匹配。</summary>
    InvalidSnapshot,
}
