namespace hub.inventory;

/// <summary>
/// 物品静态配置：邮件、掉落、GM 等只带 item_id + 数量时，用它补全堆叠上限和物品类型。
/// </summary>
public sealed class ItemDefinition(string itemId, int maxStack = ItemStack.Unlimited, ItemType type = ItemType.Stackable)
{
    public string ItemId { get; } = !string.IsNullOrWhiteSpace(itemId)
        ? itemId
        : throw new ArgumentException("item_id 不能为空", nameof(itemId));

    public int MaxStack { get; } = maxStack <= 0 ? ItemStack.Unlimited : maxStack;

    public ItemType Type { get; } = type;

    public bool Stackable => Type == ItemType.Stackable && MaxStack > 1;

    public ItemStack CreateStack(long count) => new(ItemId, count, MaxStack, Type);

    public override string ToString() => $"{ItemId}({Type}, max={MaxStack})";
}
