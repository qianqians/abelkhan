# 联盟/帮会系统（创建 / 帮会频道 / 帮会任务）

帮会数据**以 Redis 为权威**：成员、职位、申请、邀请、任务全在 Redis 里，所有会改变多把 key 的操作
（创建 / 审批 / 踢人 / 转让 / 解散 / 任务）都用 Lua 在 Redis 内原子执行，所以多 hub 进程并发时
不会超员、不会出现两个帮主。

帮会频道**复用聊天系统**的 `ChatScope.Guild` 作用域——冷却、历史、跨 hub 扇出都是现成的，
帮会系统只负责把帮会 id 喂过去。

---

## 1. 文件与职责

| 文件 | 职责 |
|---|---|
| `abelkhan/guild/GuildModels.cs` | `GuildRole` / `GuildInfo` / `GuildMember` / `GuildTaskState` / `GuildTaskCompletion` / `GuildCodes` |
| `abelkhan/guild/GuildSettings.cs` | 成员 / 副帮主 / 申请 / 邀请上限、名字校验、`DefaultTaskIds` |
| `abelkhan/guild/GuildKeys.cs` | 全部 Redis key（统一 `{guild}` hash tag） |
| `abelkhan/guild/GuildTaskCatalog.cs` | `IGuildTaskCatalog` / `GuildTaskCatalog` / `GuildTaskDefinition`（可编程注册默认任务） |
| `abelkhan/guild/GuildStore.cs` | 14 段 Lua：创建 / 改名 / 公告 / 邀请 / 申请 / 审批 / 接受邀请 / 退出 / 踢人 / 设职位 / 转让 / 解散 / 任务 |
| `abelkhan/hub/Guild/GuildService.cs` | 编排：本 hub 的「玩家→帮会」缓存、通知、任务进度与完成回调、帮会频道作用域 |
| `abelkhan/hub/Guild/PlayerGuild.cs` | 17 个玩家协议 + `GuildId`/`GuildRole` 镜像 |
| `abelkhan/hub/Chat/ChatService.cs` | `GuildScopeResolver` 注入点（帮会 id 只由帮会系统回答） |
| `abelkhan/hub/Service.cs` / `MainClass.cs` | 注入 `Guild`、任务目录、三个游戏钩子、上下线接线 |
| `proto/client.proto` | `ClientGuild*` 协议与通知 |
| `abelkhan/tests/GuildSmoke` | 基于真实 Redis 的冒烟测试（68 项断言） |

---

## 2. Redis 结构

所有 key 共用 `{guild}` hash tag（跨 key 的 Lua 在 Cluster 下同 slot）。
多游戏共用一个 Redis 时用 `GuildSettings.KeyPrefix` 隔离。

| Key | 类型 | 说明 |
|---|---|---|
| `guild:{guild}:info:{guildId}` | hash | `name` / `leaderId` / `notice` / `level` / `memberCapExtra` / `createTime` |
| `guild:{guild}:members:{guildId}` | hash | `userId → role`（1 成员 / 2 副帮主 / 3 帮主） |
| `guild:{guild}:name:{nameKey}` | string | 帮会名（小写归一化）→ guildId，做唯一性 |
| `guild:{guild}:player:{userId}` | string | userId → guildId（**一人一会**） |
| `guild:{guild}:invites:{userId}` | hash | `guildId → inviterId`（收到的邀请） |
| `guild:{guild}:applies:{guildId}` | hash | `userId → 申请时间` |
| `guild:{guild}:tasks:{guildId}` | hash | `taskId → JSON 状态` |
| `guild:{guild}:taskMembers:{guildId}:{taskId}` | set | 已领取的 memberId |
| `guild:{guild}:taskProgress:{guildId}:{taskId}` | hash | `memberId → 进度`（共享任务用保留键 `_`） |
| `guild:{guild}:index` | set | 全部 guildId |

---

## 3. 创建帮会

- 名字：`GuildSettings.NormalizeName` 做长度 / 控制字符 / 花括号 / `|` 校验
  （花括号会干扰 Redis Cluster 的 hash tag 解析）；唯一性用 `name:{小写名}` 这个 key，在创建 Lua 里校验并写入。
- 一人一会：`player:{userId}` 存在就拒绝。
- 钩子：`Service.CheckGuildCreate(player, name)` 可以先做等级 / 花费 / 冷却校验（返回非空字符串即拒绝）。
- 创建完成 → 创建者直接是帮主 → **自动发布 `DefaultTaskIds` 里的任务**。

---

## 4. 职位与权限

| 操作 | 帮主 | 副帮主 | 成员 |
|---|:--:|:--:|:--:|
| 邀请 / 审批申请 | ✓ | ✓ | — |
| 踢成员 | ✓ | ✓（不能踢副帮主/帮主） | — |
| 设/撤副帮主 | ✓ | — | — |
| 改公告 / 改名 | ✓ | — | — |
| 转让帮主 / 解散 | ✓ | — | — |
| 发布 / 取消任务 | ✓ | 按任务定义的 `OfficerCanPublish` | — |
| 领取任务 | ✓ | ✓ | ✓ |
| 退出 | 需先转让或解散 | ✓ | ✓ |

副帮主名额由 `GuildSettings.MaxOfficers` 限制（升职时在 Lua 里数一遍）。

---

## 5. 帮会任务（可编程目录 + 状态流转）

### 目录（对应「留出接口可编程配置默认帮会任务」）

```csharp
// hub/MyService.cs —— 启动时注册，热重载时 Clear() 后重新注册
GuildTaskCatalog = new GuildTaskCatalog();
GuildTaskCatalog.Register(new GuildTaskDefinition(
    TaskId: "daily_kill",
    Title: "讨伐守卫",
    Description: "全帮累计击杀 100 只守卫",
    Target: 100,
    PerMember: false,        // 全帮共享一份进度
    OfficerCanPublish: true,
    RewardKey: "guild_daily_reward"));

GuildTaskCatalog.Register(new GuildTaskDefinition("solo_donate", "个人捐献", Target: 5));
```

配合 `GuildSettings.DefaultTaskIds = { "daily_kill" }`，新建帮会就会自动带上这条任务。

### 生命周期

```
帮会创建  ──自动发布 DefaultTaskIds
client_guild_task_publish { task_id }   帮主/副帮主发布（权限在 Lua 里判）
client_guild_task_accept  { task_id }   成员领取（写入领取集合 + 进度 0）
GuildService.AddTaskProgress(guildId, taskId, memberId, delta)   ← 游戏调用
    ├─ HINCRBY 进度（共享任务写 '_'）
    └─ 达到 Target 时：任务置 finished + 回调 Service.OnGuildTaskCompletedAsync
client_guild_task_cancel  { task_id }   帮主取消（连进度一起清掉）
```

**职责边界**：框架只管「目录 / 发布 / 领取 / 进度存取 / 完成判定 / 通知」；
**什么时候加进度、完成后发什么奖励都在游戏侧**：

```csharp
// 游戏：击杀回调里上报进度
await guildService.AddTaskProgressAsync(guildId, "daily_kill", killerUserId, 1);

// 游戏：完成时发奖（成员可能离线，所以只给 id，走邮件或直接发背包）
protected internal override Task OnGuildTaskCompletedAsync(GuildTaskCompletion c)
{
    Log.Info("guild {0} task {1} done by {2}", c.GuildId, c.TaskId, c.MemberId);
    return Task.CompletedTask;
}
```

---

## 6. 帮会频道（复用聊天系统）

帮会系统在入帮 / 退帮 / 被踢 / 解散时调 `player.SetChatScope(ChatScope.Guild, guildId)`，
聊天系统用注入的 `GuildScopeResolver`（→ `GuildService.ScopeOf(userId)`）解析作用域。

为什么不让游戏的 `Player.GetChatScope` 回答：帮会 id 是帮会系统自己维护的数据，
游戏一旦重写 `GetChatScope` 忘了调 `base`，帮会频道就会静默失效。所以帮会作用域单独走一个注入点。

---

## 7. 协议

| 方法 | 说明 |
|---|---|
| `client_guild_info` | 我的帮会：信息 / 成员 / 任务 + 我的进度 / 待审批数 / 待处理邀请 |
| `client_guild_create {name}` | 创建帮会 |
| `client_guild_rename {name}` / `client_guild_notice {notice}` | 改名 / 改公告（帮主） |
| `client_guild_invite {user_id}` | 邀请（帮主 / 副帮主） |
| `client_guild_apply {guild_id}` | 申请加入 |
| `client_guild_applies` | 待审批列表 |
| `client_guild_approve {user_id, accept}` | 同意 / 拒绝申请 |
| `client_guild_invite_reply {guild_id, accept}` | 处理自己收到的邀请 |
| `client_guild_leave` | 退出（帮主需先转让或解散） |
| `client_guild_kick {user_id}` | 踢人 |
| `client_guild_set_role {user_id, role}` | 升 / 降副帮主 |
| `client_guild_transfer {user_id}` / `client_guild_disband` | 转让 / 解散 |
| `client_guild_task_publish` / `client_guild_task_cancel` / `client_guild_task_accept` | 任务 |

通知：`client_guild_updated_notify`（成员/职位/公告/改名变了，客户端重新拉 `client_guild_info`）、
`client_guild_invited_notify`、`client_guild_removed_notify`（kicked / disbanded / left）、
`client_guild_task_updated_notify`。

错误文本（`GuildCodes`）：`already_in_guild` / `not_in_guild` / `name_taken` / `name_invalid` /
`not_found` / `member_full` / `officer_full` / `no_permission` / `target_in_guild` /
`target_not_in_guild` / `leader_must_transfer` / `no_request` / `task_unknown` / `task_published` /
`task_not_published` / `task_not_accepted` / `task_already_accepted` / `apply_full` / `invite_full`。

---

## 8. 上限与扩展

```
成员上限 = GuildSettings.MaxMembers（全局默认） + guild.info.memberCapExtra（帮会自身附加）
```

上限校验和写入在同一个 Lua 里（`ApproveScript` / `AcceptInviteScript`），所以并发审批不会超员。
给帮会增加附加位：

```csharp
await guildStore.SetMemberCapExtraAsync(guildId, extra: 10);  // 帮会升级时调用
```

---

## 9. 边界与后续

- **跨 hub 镜像自愈**：入帮 / 被踢可能发生在别的 hub 上，本地 `GuildId` 镜像是懒校正的
  ——`client_guild_info` 每次都从 Redis 重新读（顺带修正聊天作用域），并靠通知让客户端去拉。
  两次交互之间该玩家在别的 hub 上的本地镜像可能是旧的，但不影响 Redis 里的权威数据。
- 被解散帮会的过期邀请没有 TTL，靠 `AcceptInvite` 时发现帮会不存在顺手清掉（自愈）。
- 未实现：帮会等级 / 经验 / 资金 / 建筑、四级职位与细粒度权限位、帮会搜索与排行榜、
  跨帮会联盟与帮会战、帮会操作日志、任务的时间限制与每日重置。
