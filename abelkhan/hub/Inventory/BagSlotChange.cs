namespace hub.inventory;

/// <summary>
/// 单个格位在操作后的最终状态，用于把背包变更同步给客户端。
/// 空位用 <see cref="ItemId"/> = null、<see cref="Count"/> = 0 表示。
/// </summary>
public readonly record struct BagSlotChange(int Slot, string? ItemId, long Count);
