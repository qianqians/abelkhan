namespace hub.inventory;

/// <summary>
/// 默认物品实现：item_id + 数量 + 堆叠上限 + 大类。
/// 需要绑定状态、品质、强化等级等实例属性的游戏可以自行实现 <see cref="IItem"/>。
/// </summary>
public sealed class ItemStack : IItem, IEquatable<ItemStack>
{
    /// <summary>无堆叠上限。</summary>
    public const int Unlimited = int.MaxValue;

    public ItemStack(string itemId, long count, int maxStack = Unlimited, ItemType type = ItemType.Stackable)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("item_id 不能为空", nameof(itemId));
        }

        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "物品数量必须大于 0");
        }

        ItemId = itemId;
        Count = count;
        MaxStack = maxStack <= 0 ? Unlimited : maxStack;
        Type = type;
    }

    public string ItemId { get; }

    public long Count { get; }

    public int MaxStack { get; }

    public ItemType Type { get; }

    public bool Stackable => Type == ItemType.Stackable && MaxStack > 1;

    public IItem WithCount(long count) => new ItemStack(ItemId, count, MaxStack, Type);

    public bool Equals(ItemStack? other) =>
        other is not null &&
        ItemId == other.ItemId &&
        Count == other.Count &&
        MaxStack == other.MaxStack &&
        Type == other.Type;

    public override bool Equals(object? obj) => obj is ItemStack other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(ItemId, Count, MaxStack, Type);

    public override string ToString() => $"{ItemId} x{Count}";
}
