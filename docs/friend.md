# 好友系统（离线申请 / 临时占位 / 跨服确认）

好友关系**以 Redis 为权威**，玩家存档只保存镜像。添加好友不是「直接写入」，而是一条带占位和
跨服确认的流程：

```
B 申请加 A
  → 写在 A 的收件箱（离线消息），此时就预检 A 是否已满
  → A 上线，收到待处理申请
  → A 同意：先在自己身上「临时占一个好友位」
  → 跨服确认：找到 B 所在服务器（B 离线则加锁从数据库拉档、建离线实例）
  → B 侧再校验 B 的上限，原子写入双方好友列表
  → 回执 A：清掉临时占位，刷新好友列表
```

这样即使两人分属不同 hub 进程、甚至都不在线，也不会出现「各自读到未满然后双双写入」的超限。

---

## 1. 文件与职责

| 文件 | 职责 |
|---|---|
| `abelkhan/friend/FriendModels.cs` | `FriendResult` / `FriendLimit` / `FriendPresence` / `FriendCodes` |
| `abelkhan/friend/FriendSettings.cs` | 上限、收件箱上限、占位超时 / 确认超时、presence / 路由 TTL |
| `abelkhan/friend/FriendKeys.cs` | 全部 Redis key（好友 key 统一带 `{friend}` hash tag） |
| `abelkhan/friend/FriendStore.cs` | 申请 / 占位 / 确认 / 回滚 / 删除 / 超时回收 / presence / 路由 / hub 队列 |
| `abelkhan/hub/Friend/FriendService.cs` | 编排：上下线推送、三步加好友、跨 hub 收发、超时回收、路由续期 |
| `abelkhan/hub/Friend/PlayerFriend.cs` | 玩家实体上的 6 个好友协议 + 好友镜像 |
| `abelkhan/hub/Service.cs` | 注入 `Friend`，游戏钩子（展示信息 / 存档镜像 / 离线实例） |
| `abelkhan/hub/MainClass.cs` | 读取 `hub:{hubId}:mq`、占位回收定时器、路由续期、下线事件接线 |
| `abelkhan/gate/MainClass.cs` | 维护 presence（每 3s 续期、断开时释放）、上报下线 |
| `proto/friend.proto` → `abelkhan/proto/Friend.cs` | `ClientFriend*` 协议与通知 |
| `proto/hub_hub.proto` | `FriendConfirmAdd` / `FriendConfirmAddResult`（跨服确认） |
| `abelkhan/tests/FriendSmoke` | 基于真实 Redis 的端到端冒烟测试（39 项断言） |

---

## 2. Redis 结构

好友相关 key 全部带 `{friend}` hash tag，跨 key 的 Lua 在 Cluster 下也落在同一 slot。
多游戏共用一个 Redis 时用 `FriendSettings.KeyPrefix` 隔离。

| Key | 类型 | 说明 |
|---|---|---|
| `friend:{friend}:{userId}` | set | **好友列表（权威）** |
| `friend:{friend}:inbox:{userId}` | set | 收到的待处理申请 = **离线消息载体** |
| `friend:{friend}:sent:{userId}` | set | 已发出的待处理申请 |
| `friend:{friend}:extra:{userId}` | string | 个人额外好友位（默认 0） |
| `friend:{friend}:reserved:{userId}` | string | **临时占位计数** |
| `friend:{friend}:pending:{userId}` | zset | 占位明细：member = 对方 id，score = 截止时间 |
| `friend:{friend}:pending_index` | zset | 有占位的玩家索引：member = userId，score = 最早截止时间 |
| `presence:{userId}` | string | `gateId|entityId`，TTL 15s，gate 维护 |
| `player_route:{userId}` | string | 玩家所在 hubId，TTL 60s，hub 维护 |
| `hub:{hubId}:mq` | list | 跨 hub 指令队列 |

---

## 3. 上限方案

```
生效上限 = FriendSettings.MaxFriends（全局默认） + extra:{userId}（每玩家额外位）
有效占用 = SCARD(friend:{userId}) + reserved:{userId}（临时占位）
```

- **扩好友位**：`FriendStore.SetExtraLimitAsync(userId, extra)`（VIP / 道具 / 等级），改数据即生效，不改代码。
- **在哪一步校验**：
  - 申请时（`SubmitRequest`）：校验被申请方 + 申请方都已经满时不产生离线消息（早拒，省得对方白同意）；
  - 同意时（`AcceptReserve`）：再次校验同意方，然后 `reserved + 1` 占位；
  - 确认时（`ConfirmAdd`）：校验被申请方，然后原子双写双方好友列表并释放占位。
- **为什么占位放 Redis 而不是只放存档**：占位的一方如果 hub 崩溃，存档根本没机会写；
  只有 Redis 的权威计数 + 定时回收能保证「既不泄漏好友位、也不超限」。
  存档里的 `PendingFriendReserve` 只是镜像，供游戏侧展示 / 核对。
- **超时回收**：`pending` 的 score 是截止时间，hub 每秒扫 `pending_index`，
  超期就 `reserved -= n`、从 `pending` 删除，并给同意方推 `client_friend_accept_timeout_notify`。

---

## 4. 加好友流程（逐步对应代码）

| 步骤 | 位置 | 行为 |
|---|---|---|
| ① 申请 | `FriendStore.SubmitRequestAsync` | 单 Lua：去重 / 收件箱限额 / 双方上限 → `SADD inbox:A B`、`SADD sent:B A` |
| ② 上线通知 | `FriendService.OnOnlineAsync` | 写路由、`SMEMBERS inbox:A` 逐条推 `client_friend_request_notify` |
| ③ 同意占位 | `FriendStore.AcceptReserveAsync` | 单 Lua：校验 A 上限 → `INCR reserved:A`、`ZADD pending:A` |
| ④ 跨服确认 | `FriendService.AcceptAsync` | 读 `player_route:B`：同 hub 直接确认；远端投 `hub:{hubB}:mq`；离线加用户锁、按需建离线实例 |
| ⑤ 对方确认 | `FriendStore.ConfirmAddAsync` | 单 Lua：校验 B 上限 → 双写好友列表 → 清 inbox/sent/pending |
| ⑥ 回执 | `hub:{hubA}:mq` | 命中等待中的 TCS → 刷新 A 的好友列表与存档镜像 → `client_friend_accept` 返回 |

失败处理：

- 对方已满 → `ConfirmAdd` 返回 `target_full` 并在 Lua 内回滚占位；申请**保留在 A 的收件箱**（可再试）。
- 确认超时（`AcceptTimeoutMs`，默认 5s）/ 对方 hub 不可达 → `AcceptAsync` 回滚占位并返回 `timeout`。
  超时设得短是因为同意请求在玩家实体的任务队列里**同步等待**回执，等太久会拖住该玩家的其它请求；
  真正的泄漏兜底是占位超时回收（`ReserveTimeoutMs`，默认 30s）。
- 重复 ack / 重复确认 → Lua 幂等（`already_friend` 分支先清残留申请与占位）。

---

## 5. 在线状态与上下线事件

- **gate 是连接权威**：它收客户端心跳、剔除超时连接。所以 presence 由 gate 维护：
  每 3s 给所有在线玩家 `SET presence:{userId} "gateId|entityId" EX 15`；
  连接被剔除时用 Lua「值完全相同才 DEL」释放，避免误删同一玩家的新连接。
- **hub 维护路由**：玩家登录时写 `player_route:{userId}`，hub 每 3s 续期。
- **通知走 `entity_{userId}_client_mq`**：gate 消费后按 userId 找连接，所以**好友在别的 hub 上也能收到**。
  （直接走 `GateNetwork` 只能发给本 hub 的玩家，这也是必须用队列的原因。）
- **上线**：`OnOnlineAsync` → `SMEMBERS friends` → 一次 `MGET presence` 过滤 → 只给在线好友推
  `client_friend_online_notify`。
- **下线**：gate 剔除连接时发 `ClientDisconnect{conn_id,user_id,entity_id}` →
  `FriendService.OnOfflineAsync` → 同上推 `client_friend_offline_notify`。
- 上下线事件走**不可靠队列**：一次性 toast，丢了可以接受；可靠队列会对离线好友反复重投。

> hub 自己的 `MainClass.TickClients` 依赖 `Client.LastEventTime`，它只在属性 getter 时刷新、
> 没有心跳喂入，**对空闲玩家永不过期**，所以不能用来判定下线——这是 presence 放在 gate 的原因。

---

## 6. 协议

| 方法 | 请求 → 响应 | 说明 |
|---|---|---|
| `client_friend_list` | `ClientFriendList` → `ClientFriendListResult` | 好友列表（含在线状态）+ 上限 / 已用 |
| `client_friend_requests` | `ClientFriendRequests` → `ClientFriendRequestsResult` | 待处理申请（离线消息） |
| `client_friend_send_request` | `ClientFriendSendRequest` → `ClientFriendSendRequestResult` | 发起申请 |
| `client_friend_accept` | `ClientFriendAccept` → `ClientFriendAcceptResult` | 同意（占位 → 跨服确认 → 回执） |
| `client_friend_reject` | `ClientFriendReject` → `ClientFriendRejectResult` | 拒绝 |
| `client_friend_remove` | `ClientFriendRemove` → `ClientFriendRemoveResult` | 删除好友（双向） |

hub 主动推送的通知：`client_friend_request_notify`、`client_friend_online_notify`、
`client_friend_offline_notify`、`client_friend_added_notify`、`client_friend_removed_notify`、
`client_friend_accept_timeout_notify`。

失败时响应里的错误文本（`FriendCodes`）：`self` / `already_friend` / `duplicate` / `no_request` /
`inbox_full` / `target_full` / `requester_full` / `self_full` / `not_friend` / `timeout`。

---

## 7. 游戏接入

```csharp
public struct MyHubConfig
{
    public FriendSettings Friend { get; set; } = new() { MaxFriends = 50 };
    public int FriendSweepIntervalMs { get; set; } = 1000;
    public int FriendSweepBatchSize { get; set; } = 200;
}
```

`Service` 上有三个可选重写：

```csharp
// 1) 好友条目的昵称 / 头像（不实现则为空，客户端自己兜底）
protected internal override Task<IReadOnlyDictionary<string, FriendDisplay>>
    ResolveFriendDisplayAsync(IReadOnlyCollection<string> userIds) { ... }

// 2) 把好友关系写进玩家存档（读 player.Friends / player.PendingFriendReserve，然后 Save()）
protected internal override Task SyncFriendMirrorAsync(Player player)
{
    player.MySaveDoc["Friends"] = new BsonArray(player.Friends);
    player.MySaveDoc["PendingFriendReserve"] = player.PendingFriendReserve;
    player.Save();
    return Task.CompletedTask;
}

// 3) 按 userId 加载离线实例（给离线玩家的存档同步好友镜像）
//    调用方已持有该玩家的用户级锁，实现要与登录读档保持一致
protected internal override Task<Player?> LoadOfflinePlayerAsync(string userId) { ... }
```

> 第 3 个钩子不实现也完全可用（纯 Redis 模式）：关系数据以 Redis 为准，玩家下次登录时
> `OnOnlineAsync` 会把好友列表灌回去。

**gate 与 hub 的配置必须一致**：`Friend.KeyPrefix` 与 `Friend.PresenceTtlMs` 相同，否则 gate 写的
presence 和 hub 读的不是同一个 key。

---

## 8. 客户端（C# SDK）

`client/Player` 已经封装好（见 `client/Player.Friend.cs`），游戏不用自己拼协议：

```csharp
var list     = await player.RequestFriendList();
var pending  = await player.RequestFriendRequests();       // 上线后拉一次待处理申请
var sent     = await player.SendFriendRequest("u100");     // 离线也受理
var accepted = await player.AcceptFriendRequest("u100");   // 占位 + 跨服确认，可能稍慢
var rejected = await player.RejectFriendRequest("u100");
var removed  = await player.RemoveFriend("u100");

// 服务端推送（框架在创建 Player 时自动注册，游戏只订阅）
player.FriendRequested   += n => { /* 收到申请 */ };
player.FriendOnline      += n => { /* 好友上线 */ };
player.FriendOffline     += n => { /* 好友下线 */ };
player.FriendAdded       += n => { /* 加好友成功 */ };
player.FriendRemoved     += n => { /* 被删除 */ };
player.FriendAcceptTimeout += n => { /* 同意超时，可重试 */ };
```

## 9. 边界与后续

- 申请没有时间戳排序（`inbox` 用 set）；要「按时间排序 / 过期自动失效」改成 zset 即可。
- 跨 hub 确认依赖目标 hub 的队列消费；目标 hub 长时间挂掉时靠 30s 超时 + 定时回收兜底，
  不会卡住玩家，但这次加好友会失败（可重试）。
- 暂未实现：黑名单、好友备注 / 分组、最近互动、跨游戏好友、好友位的后台发放页面
  （游戏调 `FriendStore.SetExtraLimitAsync` 即可）。
