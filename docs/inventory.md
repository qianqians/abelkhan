# 背包与物品系统

服务端背包实现位于 `abelkhan/hub/Inventory/`。核心逻辑（物品 / 背包 / 数组实现）只依赖
BCL，不依赖 Redis；只有存档适配器 `BagBson` 依赖 MongoDB.Bson。`hub.Player` /
`hub/Inventory/PlayerBag.cs` 负责把背包接到玩家实体、邮件领取流程和客户端协议上。

## 1. 文件与职责

| 文件 | 职责 |
|---|---|
| `ItemType.cs` | 物品大类：可堆叠 / 装备 / 任务道具 |
| `IItem.cs` | 物品抽象接口，背包只依赖它 |
| `ItemStack.cs` | 默认物品实现（item_id + 数量 + 堆叠上限 + 大类），不可变 |
| `ItemDefinition.cs` / `ItemCatalog.cs` | 物品静态配置；把「item_id + 数量」补全成物品 |
| `BagError.cs` / `BagResult.cs` | 预期内失败的错误码与结果，不靠异常控制流程 |
| `BagSlotChange.cs` | 单格变更，用于同步客户端 |
| `BagSnapshot.cs` | 背包快照（容量 + 每格物品），用于存档 / 回滚 |
| `IReadOnlyBag.cs` / `IBag.cs` | 背包只读 / 读写接口 |
| `ArrayBag.cs` | 固定长度数组实现，包含添加、消费、删除、移动、整理、快照 |
| `PlayerBag.cs` | 玩家实体上的背包协议（查询 / 移动 / 整理 / 丢弃 / 使用 + 变更推送） |
| `BagBson.cs` | 背包存档的 BSON 结构 |

## 2. 物品抽象

```csharp
var catalog = new ItemCatalog();
catalog.Register(new ItemDefinition("gold"));                    // 无上限堆叠
catalog.Register(new ItemDefinition("potion", maxStack: 99));    // 每格 99
catalog.Register(new ItemDefinition("sword", 1, ItemType.Equipment)); // 一格一把
```

`IItem` 的约定：

- `Count` 必须大于 0；
- `WithCount` 返回新对象，不修改自身。背包的快照 / 回滚只复制引用，
  依赖物品不可变；
- 同 `ItemId` 的物品只有 `Stackable == true`、且 `MaxStack` / `Type` 相同才允许合并。

需要绑定、品质、强化等级等实例属性时，游戏实现自己的 `IItem` 即可；
只要 `WithCount` 保留这些属性，背包逻辑不用改。注意：默认的 `BagBson`
只序列化 `ItemId / Count / MaxStack / Type` 四个字段，自定义 `IItem` 的额外字段
需要游戏扩展存档结构或自己实现序列化。

## 3. 背包接口

```csharp
IBag bag = player.Bag;

bag.Add("potion", 10);                       // 空间不足整体失败
bag.Add(new ItemStack("sword", 1, 1, ItemType.Equipment));
bag.AddRange([itemA, itemB]);                // 批量原子添加
bag.AddPartial(item, out var remaining);     // 能放多少放多少，适合掉落

bag.Consume("potion", 3);                    // 数量不足失败，背包不变
bag.RemoveAt(0);                             // 删除整格
bag.RemoveAt(1, 2);                          // 删除某格的部分数量
bag.Delete("sword");                         // 删除某 item_id 的所有堆叠

bag.Sort();                                  // 压缩空格 + 按 item_id 稳定排序
bag.Move(0, 3);                              // 移动到空格位；目标非空则交换
```

`IReadOnlyBag` 提供 `Capacity`、`OccupiedSlots`、`Version`、索引器、`GetItemCount`、
`Contains`、`Snapshot`，UI / 任务统计只依赖它。

`BagResult` 用 `Ok` / `Error` / `Affected` / `Message` 表达结果；预期内失败不抛异常。

### 数组实现的行为

- 固定容量，构造时 `new ArrayBag(capacity, catalog)`；
- 添加时先补满已有同类堆叠，再用空格。非堆叠物品（装备 / 任务道具）每格 1 件；
- `Add` / `AddRange` 空间不足会回滚到操作前，不会留下"加了一半"的状态；
- `Consume` 先校验总数，再按格位顺序扣；
- `Sort` 是稳定排序（相同 `item_id` 保持原有相对顺序），空位集中到末尾；
- `Move` 用于拖拽：目标格为空则移动，非空则交换；源格为空会失败；
- 每次成功变更 `Version++`，并触发 `Changed` 事件（参数是变化格位的最终状态）。

背包本身不加锁，默认由 hub 的 per-entity Actor 串行访问；如果在 Actor 外共享同一个
背包实例，需要调用方自己加锁。

## 4. 邮件领取物品

领取全局邮件时 `hub.Player.RegisterMailRequest` 执行：

1. `GetClaimable` 校验邮件是否可领；
2. `MarkClaimed`（Redis `SADD`）原子认领，并发 / 重复请求只有一次成功；
3. `GrantMailAttachments` 把附件转成 `IItem`，调用 `Bag.AddRange` **原子**放入背包；
4. 如果背包满或附件配置错误：
   - `Bag.AddRange` 已自动回滚，背包保持原样；
   - 调用 `GlobalMailBox.UnmarkClaimed`（Redis `SREM`）回滚领取标记，
     玩家整理背包后可以重新领取；
5. 全部成功后才把邮件返回给客户端。

> 崩溃窗口：第 2 步成功、第 3 步执行前进程崩溃，邮件会处于"已领取但未发物品"状态。
> 这是无数据库事务下的 at-most-once 取舍；如果业务要求绝对不丢，需要把
> 「领取标记 + 背包存档」放进同一个 Mongo 事务 / outbox，而不是只用 Redis SET。

附件数量 `<= 0` 会被跳过；所有附件都为空时只认领、不加物品。

## 5. 存档

`Player.BagInfo()` 返回可放进玩家文档的 BSON：

```csharp
public override BsonDocument FullInfo() => new()
{
    { "EntityType", EntityType },
    { "EntityId", EntityId },
    { "Bag", BagInfo() },
};
```

读档：

```csharp
if (document.TryGetValue("Bag", out var bagValue) && bagValue.IsBsonDocument)
{
    var snapshot = BagBson.ToSnapshot(bagValue.AsBsonDocument, Player.DefaultBagCapacity);
    var bag = new ArrayBag(snapshot.Capacity, ItemCatalog);
    var loaded = bag.Load(snapshot);
    if (loaded.Failed)
    {
        // 存档损坏：记录日志 / 走补偿，不要静默丢物品
    }
    RestoreBag(bag);
}
else
{
    // 新玩家：首次访问 Bag 时按 BagCapacity 自动创建
}
```

`BagBson` 用 `null` 保留空位下标，装备格 / 快捷栏位置不会在存读档后错位；
`SchemaVersion` 为 1，未来格式升级时可以在 `ToSnapshot` 里按版本迁移。

## 6. 客户端协议

协议定义在 `proto/bag.proto`（生成到 `abelkhan/proto/Bag.cs`），服务端处理在
`hub/Inventory/PlayerBag.cs`，客户端封装在 `client/Player.Bag.cs`：

| 方法 | 请求 / 结果 | 说明 |
|---|---|---|
| `client_bag_info` | `ClientBagInfo` / `ClientBagInfoResult` | 拉完整背包（容量、已用格数、版本、非空格列表） |
| `client_bag_move` | `ClientBagMove` / `ClientBagMoveResult` | 移动 / 交换格位 |
| `client_bag_sort` | `ClientBagSort` / `ClientBagSortResult` | 压缩空格 + 按 item_id 排序 |
| `client_bag_discard` | `ClientBagDiscard` / `ClientBagDiscardResult` | 丢弃物品；`count <= 0` 丢整格 |
| `client_bag_use` | `ClientBagUse` / `ClientBagUseResult` | 使用物品；具体效果由游戏钩子实现 |

所有会改动背包的路径（邮件附件、商城结算、任务奖励、游戏逻辑）都会触发
`ClientBagChangedNotify`，只携带变化格位的最终状态（空格 `item_id = ""`、`count = 0`）
和 `version`；客户端按格位覆盖即可，乱序时可以用 `version` 丢弃过期通知。

登录 / 重连时拉一次 `client_bag_info` 做全量初始化；离线期间的变更不会推送，下次登录同样靠
`client_bag_info` 同步。

### 游戏钩子

`hub.Service` 提供两个钩子：

```csharp
// 使用物品：返回 Ok(提示文本) 表示成功；Err(原因) 表示拒绝。
// 默认返回「该物品不能使用」——框架不知道物品效果，游戏必须自己实现。
protected override Task<Result<string, string>> UseBagItemAsync(
    Player player, IItem item, long count)
{
    var consumed = player.Bag.Consume(item.ItemId, count);
    return Task.FromResult(consumed.Failed
        ? Result<string, string>.Err(consumed.Message)
        : Result<string, string>.Ok($"使用了 {item.ItemId} x{count}"));
}

// 丢弃前校验：绑定 / 任务道具等可以在这里拒绝。
protected override string? CheckDiscardBagItem(Player player, IItem item, long count)
    => item.ItemId == "quest" ? "任务道具不能丢弃" : null;
```

`Player.BagChanged` 事件仍然保留，游戏可以在协议推送之外做自己的处理。

## 7. 冒烟测试

`abelkhan/tests/InventorySmoke` 是纯逻辑冒烟测试，不依赖 Redis / Mongo：

```bash
dotnet run --project abelkhan/tests/InventorySmoke
```

覆盖：堆叠拆分、满包原子回滚、消费不足、按格删除、按 item_id 全删、
移动 / 交换格位、整理排序与空洞压缩、`AddRange` 原子性、`AddPartial`、非堆叠物品、
配置校验、快照 / BSON 往返、变更事件 / Version、快照容量不匹配。

