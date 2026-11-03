using hub.inventory;

// 背包冒烟测试：不依赖 Redis / Mongo，只验证 hub.inventory 的纯逻辑。
// 运行：dotnet run --project abelkhan/tests/InventorySmoke

var checks = 0;
var failures = new List<string>();

void Check(bool condition, string message)
{
    checks++;
    if (!condition)
    {
        failures.Add(message);
    }
}

void CheckEqual<T>(T expected, T actual, string message)
{
    checks++;
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        failures.Add($"{message}: expected={expected}, actual={actual}");
    }
}

var catalog = new ItemCatalog();
catalog.Register(new ItemDefinition("potion", maxStack: 3));
catalog.Register(new ItemDefinition("sword", 1, ItemType.Equipment));
catalog.Register(new ItemDefinition("quest", 1, ItemType.Quest));

// 1. 添加 / 堆叠拆分 / 满包回滚
{
    var bag = new ArrayBag(2, catalog);
    var result = bag.Add("potion", 5);
    Check(result.Ok && result.Affected == 5, "add 5 potions");
    CheckEqual(2, bag.OccupiedSlots, "stack split uses 2 slots");
    CheckEqual(3L, bag[0]?.Count ?? 0, "slot 0 has 3");
    CheckEqual(2L, bag[1]?.Count ?? 0, "slot 1 has 2");

    result = bag.Add("potion", 2);
    Check(result.Failed && result.Error == BagError.NotEnoughSpace, "full bag add fails");
    CheckEqual(5L, bag.GetItemCount("potion"), "failed add leaves bag unchanged");

    result = bag.Add("potion", 1);
    Check(result.Ok, "add 1 more merges into partial stack");
    CheckEqual(3L, bag[0]?.Count ?? 0, "slot 0 still 3");
    CheckEqual(3L, bag[1]?.Count ?? 0, "slot 1 filled to 3");
}

// 2. 消费 / 数量不足
{
    var bag = new ArrayBag(2, catalog);
    bag.Add("potion", 6);

    var result = bag.Consume("potion", 4);
    Check(result.Ok && result.Affected == 4, "consume 4");
    CheckEqual(2L, bag.GetItemCount("potion"), "remaining 2");
    CheckEqual(1, bag.OccupiedSlots, "empty slot after consume");

    result = bag.Consume("potion", 99);
    Check(result.Failed && result.Error == BagError.NotEnoughItems, "consume too many fails");
    CheckEqual(2L, bag.GetItemCount("potion"), "failed consume leaves bag unchanged");
}

// 3. 删除格位 / 按 item_id 全删 / 整理排序
{
    var bag = new ArrayBag(4, catalog);
    bag.Add("potion", 2);
    bag.Add("sword", 1);

    var result = bag.RemoveAt(0, 1);
    Check(result.Ok && result.Affected == 1, "remove partial slot");
    CheckEqual(1L, bag.GetItemCount("potion"), "partial remove keeps 1");

    result = bag.Delete("potion");
    Check(result.Ok && result.Affected == 1, "delete by item_id");
    CheckEqual(0L, bag.GetItemCount("potion"), "potion deleted");

    result = bag.RemoveAt(1);
    Check(result.Ok && result.Affected == 1, "remove whole slot");
    CheckEqual(0, bag.OccupiedSlots, "bag empty");

    bag.Add("quest", 1);
    bag.Add("potion", 1);
    bag.Sort();
    CheckEqual("potion", bag[0]?.ItemId, "sort by item_id first");
    CheckEqual("quest", bag[1]?.ItemId, "sort by item_id second");
    Check(bag[2] == null && bag[3] == null, "sort leaves empty slots at the end");
}

// 4. 快照中的空洞整理后会被压缩
{
    var snapshot = new BagSnapshot(4, new IItem?[]
    {
        null,
        new ItemStack("quest", 1, 1, ItemType.Quest),
        null,
        new ItemStack("potion", 1, 3),
    });
    var bag = new ArrayBag(4, catalog);
    Check(bag.Load(snapshot).Ok, "load snapshot with gaps");
    bag.Sort();
    CheckEqual("potion", bag[0]?.ItemId, "compact + sort: first");
    CheckEqual("quest", bag[1]?.ItemId, "compact + sort: second");
    Check(bag[2] == null && bag[3] == null, "compact + sort: tail empty");
}

// 5. AddRange 原子回滚 / AddPartial
{
    var bag = new ArrayBag(1, catalog);
    var result = bag.AddRange(new IItem[]
    {
        new ItemStack("potion", 2, 3),
        new ItemStack("sword", 1, 1, ItemType.Equipment),
    });
    Check(result.Failed && result.Error == BagError.NotEnoughSpace, "AddRange full fails");
    CheckEqual(0, bag.OccupiedSlots, "AddRange rolls back");
    CheckEqual(0L, bag.GetItemCount("potion"), "AddRange rollback leaves no potion");

    result = bag.AddPartial(new ItemStack("potion", 5, 3), out var remaining);
    Check(result.Ok && result.Affected == 3 && remaining == 2, "AddPartial returns leftover");
    CheckEqual(3L, bag.GetItemCount("potion"), "AddPartial puts what fits");
}

// 6. 非堆叠物品按件占格
{
    var bag = new ArrayBag(3, catalog);
    var result = bag.Add(new ItemStack("sword", 2, 1, ItemType.Equipment));
    Check(result.Ok && result.Affected == 2, "two non-stackable items");
    CheckEqual(2, bag.OccupiedSlots, "non-stackable uses one slot each");
    CheckEqual(1L, bag[0]?.Count ?? 0, "non-stackable slot count is 1");
    CheckEqual(1L, bag[1]?.Count ?? 0, "non-stackable second slot count is 1");
}

// 7. 配置校验 / 无配置时允许任意 id
{
    var strict = new ArrayBag(1, catalog);
    var result = strict.Add("unknown", 1);
    Check(result.Failed && result.Error == BagError.UnknownItem, "unknown item rejected with catalog");

    var free = new ArrayBag(1);
    Check(free.Add("anything", 2).Ok, "unknown item allowed without catalog");
}

// 8. 快照 / BSON 往返
{
    var bag = new ArrayBag(3, catalog);
    bag.Add("potion", 4);
    bag.Add("quest", 1);

    var snapshot = bag.Snapshot();
    CheckEqual(3, snapshot.Capacity, "snapshot capacity");
    CheckEqual(3, snapshot.OccupiedSlots, "snapshot occupied slots");

    var restored = new ArrayBag(3, catalog);
    Check(restored.Load(snapshot).Ok, "load snapshot");
    CheckEqual(bag.GetItemCount("potion"), restored.GetItemCount("potion"), "snapshot potion count");
    CheckEqual(bag.GetItemCount("quest"), restored.GetItemCount("quest"), "snapshot quest count");

    var document = BagBson.ToBson(bag);
    var fromBson = BagBson.ToSnapshot(document);
    var bsonBag = new ArrayBag(fromBson.Capacity, catalog);
    Check(bsonBag.Load(fromBson).Ok, "load bson snapshot");
    CheckEqual(bag.GetItemCount("potion"), bsonBag.GetItemCount("potion"), "bson potion count");
    CheckEqual(bag.GetItemCount("quest"), bsonBag.GetItemCount("quest"), "bson quest count");
    CheckEqual(bag[0]?.ItemId, bsonBag[0]?.ItemId, "bson keeps slot position");
}

// 9. 变更事件 / Version
{
    var bag = new ArrayBag(2, catalog);
    var events = new List<IReadOnlyList<BagSlotChange>>();
    bag.Changed += changes => events.Add(changes);

    bag.Add("potion", 3);
    CheckEqual(1L, bag.Version, "version increments on add");
    CheckEqual(1, events.Count, "one change event");
    CheckEqual(0, events[0][0].Slot, "changed slot");
    CheckEqual(3L, events[0][0].Count, "changed count");

    bag.Consume("potion", 3);
    CheckEqual(2L, bag.Version, "version increments on consume");
    CheckEqual(2, events.Count, "second change event");
    CheckEqual(null, events[1][0].ItemId, "empty slot after consume");
}

// 10. 载入容量不足的快照失败
{
    var tooBig = new BagSnapshot(5, new IItem?[5]);
    var small = new ArrayBag(2, catalog);
    var result = small.Load(tooBig);
    Check(result.Failed && result.Error == BagError.InvalidSnapshot, "load capacity mismatch fails");
}

if (failures.Count > 0)
{
    Console.Error.WriteLine($"FAILED: {failures.Count}/{checks} checks");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"  - {failure}");
    }
    return 1;
}

Console.WriteLine($"OK: {checks} checks passed");
return 0;
