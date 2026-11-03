using MongoDB.Bson;

namespace hub.inventory;

/// <summary>
/// 背包的 BSON 存档格式。字段使用 PascalCase，与仓库里玩家存档的约定一致。
///
/// <code>
/// {
///   "SchemaVersion": 1,
///   "Capacity": 100,
///   "Slots": [
///     null,
///     { "ItemId": "1001", "Count": 10, "MaxStack": 99, "Type": 0 }
///   ]
/// }
/// </code>
///
/// 用 null 保留格位下标，而不是只存非空物品，这样装备格 / 快捷栏的位置不会在
/// 存读档后错位；同时以后新增格位语义（例如锁定格）也不会破坏旧存档。
/// </summary>
public static class BagBson
{
    public const int CurrentSchemaVersion = 1;

    /// <summary>把背包（通常来自 <see cref="IBag"/>）序列化成 BSON 文档。</summary>
    public static BsonDocument ToBson(IReadOnlyBag bag)
    {
        ArgumentNullException.ThrowIfNull(bag);

        var slots = new BsonArray();
        for (var i = 0; i < bag.Capacity; i++)
        {
            var item = bag[i];
            BsonValue value = item == null ? BsonNull.Value : ItemToBson(item);
            slots.Add(value);
        }

        return new BsonDocument
        {
            { "SchemaVersion", CurrentSchemaVersion },
            { "Capacity", bag.Capacity },
            { "Slots", slots },
        };
    }

    /// <summary>
    /// 把 BSON 文档还原成快照。未提供 / 非法容量时使用 <paramref name="fallbackCapacity"/>；
    /// 格位数组比容量长时自动扩容，避免旧存档被截断。
    /// </summary>
    /// <exception cref="FormatException">文档字段格式非法。</exception>
    /// <exception cref="NotSupportedException">存档版本高于当前代码支持的版本。</exception>
    public static BagSnapshot ToSnapshot(BsonDocument document, int fallbackCapacity = 0)
    {
        ArgumentNullException.ThrowIfNull(document);

        var version = ReadInt32(document, "SchemaVersion", CurrentSchemaVersion);
        if (version > CurrentSchemaVersion)
        {
            throw new NotSupportedException(
                $"背包存档版本 {version} 高于当前支持版本 {CurrentSchemaVersion}");
        }

        BsonArray slots;
        if (document.TryGetValue("Slots", out var slotsValue) && !slotsValue.IsBsonNull)
        {
            if (!slotsValue.IsBsonArray)
            {
                throw new FormatException("背包存档 Slots 字段不是数组");
            }

            slots = slotsValue.AsBsonArray;
        }
        else
        {
            slots = new BsonArray();
        }

        var capacity = ReadInt32(document, "Capacity", fallbackCapacity);
        if (capacity < slots.Count)
        {
            capacity = slots.Count;
        }

        if (capacity <= 0)
        {
            throw new FormatException("背包存档缺少有效的 Capacity");
        }

        var items = new List<IItem?>(slots.Count);
        for (var i = 0; i < slots.Count; i++)
        {
            items.Add(ItemFromBson(slots[i], i));
        }

        return new BagSnapshot(capacity, items);
    }

    private static BsonDocument ItemToBson(IItem item) =>
        new BsonDocument
        {
            { "ItemId", item.ItemId },
            { "Count", item.Count },
            { "MaxStack", item.MaxStack },
            { "Type", (int)item.Type },
        };

    private static IItem? ItemFromBson(BsonValue value, int slot)
    {
        if (value.IsBsonNull)
        {
            return null;
        }

        if (!value.IsBsonDocument)
        {
            throw new FormatException($"背包存档格位 {slot} 不是文档或 null");
        }

        var document = value.AsBsonDocument;
        if (!document.TryGetValue("ItemId", out var itemIdValue) ||
            itemIdValue.IsBsonNull ||
            !itemIdValue.IsString ||
            string.IsNullOrWhiteSpace(itemIdValue.AsString))
        {
            throw new FormatException($"背包存档格位 {slot} 的 ItemId 非法");
        }

        var count = ReadInt64(document, "Count", 0);
        if (count <= 0)
        {
            throw new FormatException($"背包存档格位 {slot} 的 Count 必须大于 0");
        }

        var maxStack = ReadInt32(document, "MaxStack", ItemStack.Unlimited);
        var type = (ItemType)ReadInt32(document, "Type", (int)ItemType.Stackable);
        return new ItemStack(itemIdValue.AsString, count, maxStack, type);
    }

    private static int ReadInt32(BsonDocument document, string name, int fallback)
    {
        if (!document.TryGetValue(name, out var value) || value.IsBsonNull)
        {
            return fallback;
        }

        try
        {
            return value.ToInt32();
        }
        catch (Exception e)
        {
            throw new FormatException($"背包存档字段 {name} 不是整数", e);
        }
    }

    private static long ReadInt64(BsonDocument document, string name, long fallback)
    {
        if (!document.TryGetValue(name, out var value) || value.IsBsonNull)
        {
            return fallback;
        }

        try
        {
            return value.ToInt64();
        }
        catch (Exception e)
        {
            throw new FormatException($"背包存档字段 {name} 不是整数", e);
        }
    }
}
