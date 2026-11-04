# 任务系统（可配置触发 / 进度 / 完成条件 + 任务链）

任务用数据配置：

- **触发条件** —— 满足后任务变成「可接」（`AutoAccept` 的自动接取，其余进可接列表）；
- **进度条件** —— 任务自己的计数器，被游戏上报的事件推进；
- **完成条件** —— 门槛，不累计，完成时判定；
- **任务链** —— 完成后按 `NextQuestIds` 顺序挑第一个满足条件的后继。

条件的语义完全由游戏定义：框架只认 `(type, key, mode, target)`，所有事件都通过
`ReportEvent(type, key, amount)` 上报。

---

## 1. 文件与职责

| 文件 | 职责 |
|---|---|
| `abelkhan/quest/QuestModels.cs` | `QuestCondition` / `QuestConditionMode` / `QuestState` / `QuestCompletion` / `QuestCodes` |
| `abelkhan/quest/QuestCatalog.cs` | `QuestDefinition` / `IQuestCatalog` / `QuestCatalog`（含根任务计算） |
| `abelkhan/quest/QuestRules.cs` | 纯判定函数：条件满足 / 能否接 / 进度满 / 多后继挑一 |
| `abelkhan/quest/QuestSettings.cs` | `KeyPrefix` / `MaxActiveQuests` / `AutoCompleteWhenReady` |
| `abelkhan/quest/QuestKeys.cs` | 四个 key（统一 `{quest}` hash tag） |
| `abelkhan/quest/QuestStore.cs` | 事件状态 / 接取 / 进度推进 / 完成 / 重置（Lua） |
| `abelkhan/hub/Quest/QuestService.cs` | 编排：事件分发、触发判定、完成与任务链、发奖回调、协议组装 |
| `abelkhan/hub/Quest/PlayerQuest.cs` | 3 个玩家协议 |
| `abelkhan/hub/Service.cs` / `MainClass.cs` | 注入 `Quest`、任务目录、2 个游戏钩子 |
| `proto/client.proto` | `ClientQuest*` 协议与通知 |
| `abelkhan/tests/QuestSmoke` | 冒烟测试（44 项断言） |

---

## 2. 条件模型（地基）

```csharp
public enum QuestConditionMode
{
    Count = 1,  // value += amount        击杀 10 只
    Max   = 2,  // value = max(value, amount)   等级达到 10（上报的是等级本身）
    Flag  = 3,  // 发生过就为真            和 NPC 对过话
}

public sealed record QuestCondition(
    string Type,                            // 事件类型：kill / level / talk / ...
    string Key,                             // 事件 key：goblin / npc_7 / ""
    QuestConditionMode Mode = QuestConditionMode.Count,
    long Target = 1);
```

> `Max` 不是可选装饰：**等级 / 战力是绝对值不是 +1**。没有它，「等级达到 10」就得让游戏自己算差值，
> 配置极易出错，玩家重登/掉级还会算歪。

---

## 3. 三条条件的分工

| | 数据源 | 何时用 |
|---|---|---|
| 触发条件 | 玩家**事件状态**（`type|key → value`） | 上线 / 每次事件 / 任务完成后判定「能不能接」 |
| 进度条件 | 该任务**自己的计数器** | 任务进行中，匹配到的事件推进它；全部达到 Target 才算进度满 |
| 完成条件 | 玩家**事件状态**（不满足再问游戏钩子） | 提交完成时判定 |

为什么进度不共用事件状态：同一个「击杀」事件，任务 A 要 10 只、任务 B 要 50 只，
共用一套计数会互相污染。

**完成条件里的「实时状态」**（例如「背包里要有 5 个药水」）：
配置成 `Flag` 门槛 + 重写 `Service.CheckQuestCondition(...)`，框架在完成时逐条问游戏。

---

## 4. 任务定义

```csharp
var catalog = new QuestCatalog();
catalog.RegisterRange(
[
    new QuestDefinition(
        QuestId: "main_1",
        Title: "初入江湖",
        AutoAccept: true,
        TriggerConditions: [new("level", "", QuestConditionMode.Max, 5)],
        ProgressConditions: [new("kill", "goblin", QuestConditionMode.Count, 10)],
        CompletionConditions: [new("talk", "npc_elder", QuestConditionMode.Flag)],
        NextQuestIds: ["main_2", "side_1"],
        RewardKey: "reward_main_1"),

    new QuestDefinition("main_2", "深入密林", RequireQuests: ["main_1"],
        ProgressConditions: [new("kill", "wolf", QuestConditionMode.Count, 20)]),
    new QuestDefinition("side_1", "支线", RequireQuests: ["main_1"], AutoAccept: false),
]);
```

---

## 5. 流程

```
游戏 ReportEvent(type, key, amount, mode)
  ├─ 更新玩家事件状态（Count / Max / Flag）
  ├─ 进行中任务的进度条件命中 → 推进（Max 模式用绝对值覆盖）
  ├─ AutoCompleteWhenReady 且进度满 + 门槛满足 → 直接完成
  └─ 触发判定（只扫「根任务」）→ 可接 / 自动接取

玩家 client_quest_accept {quest_id}
  └─ 校验：未完成、未进行中、前置任务都完成、触发条件满足

玩家 client_quest_complete {quest_id}
  └─ 校验：进行中 + 进度全满 + 完成门槛满足
       ├─ 标记完成（记完成时间、清该任务进度）
       ├─ 回调 Service.OnQuestCompletedAsync（按 RewardKey 发奖）
       ├─ 任务链：按 NextQuestIds 顺序挑第一个满足条件的后继接取（挑一）
       └─ 再跑一次根任务触发判定
```

**根任务（Roots）**：没有被任何任务的 `NextQuestIds` 引用过的任务。只有根任务参与全局触发扫描，
链上的任务只在「前驱完成」时按顺序挑——否则同一条链会被激活两次，分支语义就没了。

**挑一的含义**：多个后继只有第一个满足条件的会被接取（`AutoAccept=false` 的后继只变成可接）。
想让另一条分支也能走，就把它配成根任务（靠触发条件接），或者放进别的任务的后继里。

---

## 6. Redis 结构（统一 `{quest}` hash tag）

| Key | 类型 | 说明 |
|---|---|---|
| `quest:{quest}:event:{userId}` | hash | `type|key → value`（事件状态，触发 / 完成条件读它） |
| `quest:{quest}:active:{userId}` | hash | `questId → JSON`（进度快照 + 接取时间） |
| `quest:{quest}:done:{userId}` | hash | `questId → 完成时间` |
| `quest:{quest}:progress:{userId}` | hash | `questId|type|key → 进度` |

---

## 7. 协议

| 方法 | 说明 |
|---|---|
| `client_quest_list` | 进行中（含逐条进度 + `ready_to_complete`）+ 可接列表 + 已完成数 |
| `client_quest_accept {quest_id}` | 手动接取 |
| `client_quest_complete {quest_id}` | 提交完成，返回本次激活 / 可接的后继 id |
| `client_quest_updated_notify` | 某条任务的状态 / 进度变化 |

错误文本（`QuestCodes`）：`unknown_quest` / `already_active` / `already_done` / `not_active` /
`not_available` / `progress_not_full` / `gate_not_satisfied` / `too_many_active`。

---

## 8. 游戏接入

```csharp
// 1) 注册任务目录（从 JSON / 配置表加载后注册；热重载 Clear() 再批量注册）
QuestCatalog = QuestConfigLoader.Load("quests.json");

// 2) 上报事件（游戏里到处调，框架只认 type / key / amount / mode）
await questService.ReportEventAsync(userId, "kill", "goblin", 1);                    // Count
await questService.ReportEventAsync(userId, "level", "", level, QuestConditionMode.Max);
await questService.ReportEventAsync(userId, "talk", "npc_7", 1, QuestConditionMode.Flag);

// 3) 发奖 + 可选的实时门槛
protected internal override Task OnQuestCompletedAsync(QuestCompletion c)
{
    Mail.Send(c.UserId, "任务完成", c.RewardKey);   // 奖励标识由你自己的配置解释
    return Task.CompletedTask;
}

protected internal override bool CheckQuestCondition(Player p, QuestDefinition def, QuestCondition cond)
    => cond.Type == "has_item" && p.Bag.GetItemCount(cond.Key) >= cond.Target;
```

---

## 9. 客户端（C# SDK）

`client/Player` 已经封装好（见 `client/Player.Quest.cs`）：

```csharp
var list     = await player.RequestQuestList();            // 进行中（带进度）+ 可接 + 已完成数
var accepted = await player.AcceptQuest("main_1");         // 手动接（AutoAccept=false 的任务）
var done     = await player.CompleteQuest("main_1");       // 返回本次激活 / 可接的后继
foreach (var next in done.Value.NextQuestIds) { /* 提示玩家 */ }

// 服务端推送（框架自动注册）
player.QuestUpdated += n => { /* 某条任务的状态 / 进度变化 */ };
```

## 10. 边界与后续

- **离线不推进**：`ReportEvent` 对离线玩家仍会更新事件状态，但不推通知（没有连接可投）。
- **事件回放**：只统计接入后上报的事件；老玩家登录时如果需要补算历史（等级 / 已完成任务），
  由游戏在登录流程里补一次 `ReportEvent`。
- **并发**：同一玩家的请求在 `Actor` 队列里串行，且一个玩家只在一个 hub 上，所以
  「读状态 → 算增量 → 写回」按玩家串行安全，不需要把整条判定塞进 Lua。
- 未实现：放弃任务、任务时间限制 / 每日重置、任务分类与追踪字段、任务奖励直接发放
  （只给 `RewardKey`）、队伍 / 帮会共享任务进度。
