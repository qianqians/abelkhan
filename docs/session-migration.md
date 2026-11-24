# 顶号（登录挤占）与玩家实体跨 hub 迁移

两个功能共用一条 **Redis 会话归属记录** 和同一条 **hub→hub 命令通道**（`hub_hub_{hubId}_mq`）。
编排在 `abelkhan/hub/Session/SessionCoordinator.cs`（从 MainClass 抽出，可注入、可双节点测试），
MainClass 只做委托。

---

## 0. 前置修复：登录入口曾经是断的

gate 把登录请求推进 Redis list，但 hub 的消费循环 `_gateWaitQueue` **从未被播种**，
`OnRequestService` 在生产路径上永不触发。本次一并修复：

- gate 的 accept 登录推送统一到 `entity_{serviceName}_server_mq`（与重连回退同 key）；
- hub 启动时把所有已注册服务名播种进 `_gateWaitQueue`，新建实体也入轮转
  （`MainClass.EnqueueMqRotation`，去重）。

---

## 1. 会话归属记录

```
session:{userId} = { hub, gate, conn, entity, seq }     TTL 90s（在线续期）
```

- `SessionRegistry.ClaimAsync`：**单条 Lua** 做「读旧值 + seq+1 + 写新值」，跨 hub 并发登录
  只有一个赢家拿到旧记录去踢人。
- `RefreshAsync` / `ReleaseAsync` 都带 seq 比对：迟到的旧 hub 不会给新会话续命、也不会删掉新记录。
- hub 的 `TickClients` 每 3s 对本 hub 在线玩家续期。

单 hub 内存里的 `clients` 字典跨 hub 看不见，所以权威记录必须在 Redis。

---

## 2. 顶号

```
新登录 → hub.OnLoginAsync
  ├─ 复用优先：Service.PeekLoginUserId(argv) 能在创建前看出 userId 时，
  │            本 hub 已有活实体 → RebindAsync（换 clients[userId] + 重发 CreatePlayerEntity），不重建
  │            看不出 → 创建后按 userId 去重（丢弃刚创建的对象，复用旧的）
  ├─ SessionRegistry.ClaimAsync → (prev, seq)
  └─ prev 存在且 conn 不同：
       同 hub  → HubKickOffClient{旧 connId} + 旧实体 Save 后摘除
       跨 hub  → hub_hub_{prevHub}_mq 发 HubSessionReplaced{entityId, seq}
                 旧 hub 的 player 实体（RegisterHubNotify）校验 seq 更大后：
                 踢自己的旧连接 → 游戏钩子 OnSessionReplacedAsync → Save → 摘除
```

要点：

- **踢旧连接必须用旧 connId 定向**（`HubKickOffClient.conn_id`）：那一刻 `clients[userId]` 已指向新 gate。
- 旧客户端收到 `KickOff` 后 `_kicked=true`，**不自动重连**（client SDK 已具备），不会顶号死循环。
- `HubSessionReplaced` 的 `EntityId` 必须带旧实体 id —— `HubGlobalEvent` 按它路由到旧 hub 的 player 实体；
  留空会被当成 hub 级命令丢掉。
- 同 hub 复用时 `Service.RebindAsync` 的 `AddOrUpdate` 工厂必须**返回新 Client**；
  `EchoQueryServiceEntity` 里的 `(key, old) => old` 是「首次登录优先」语义，照抄会把重绑静默变成 no-op
  （实现时踩过，测试抓出来了）。

---

## 3. 迁移（六阶段）

触发无关：`MigrateAsync(userId, targetHubId, options)`。场景切换 / 排水 / admin 手动都调它；
场景→hub 的映射由游戏的 `ResolveTargetHub` 钩子给，框架不猜。

```
A 切割（旧 hub）  _migrating=1：此后客户端请求只缓冲不执行（BaseEntity.OnDoMsg 入口判断）
                  向 actor 队尾投 fence job：它完成 = 切割前入队的 job 都跑完了
B 落地（旧 hub）  fence 完成 → Player.Save()（Mongo）；Redis 子系统本就权威
                  发 HubMigrateRequest{userId, entityId, seq, fromHub, buffer[]}
C 重建（新 hub）  Service.LoadOfflinePlayerAsync(userId) 从存档建实体（无 client）
                  AttachMigratedPlayerAsync：重挂全部协议 + 各子系统上线同步
                  按序 replay buffer → AttachClientOnly（挂 clients 条目，不发 CreatePlayerEntity）
                  CAS 抢 session(seq+1) → 回 HubMigrateResult{ok, seq}     ← 此刻 gate 仍指旧 hub
D 转尾（旧 hub）  转剩余 buffer（HubMigrateForward）→ 立墓碑 → RetirePlayerAsync 摘除
E 接管（新 hub）  等旧 hub「墓碑静默」（grace 内无转发）→ CreateRemotePlayer 发 CreatePlayerEntity
                  → gate 用新 hub 的连接重注册 cli.RegisterNetwork(entityId, 新 hub)   【接管完成】
                  → 客户端本地同 entityId → OnRefresh，零感知
F 收尾            旧 hub 墓碑到期删除；新 hub 触发 OnEntityMigratedInAsync
```

### 保序与不丢

- 客户端请求在一条 WebSocket 上有序；gate 在 re-point 前发旧 hub、之后发新 hub。
  若新 hub 提前直收就会乱序 → **re-point 放在最后**（阶段 E），并用墓碑静默期收尾。
- 墓碑：旧 hub 摘除实体后，`GateMsgHandle` 发现实体不在 → `ForwardMissingEntityAsync`
  把请求原样转给新 hub（`HubMigrateForward`），grace 内无转发才发 `HubMigrateQuiet`。
- 失败回滚：`FailMigrationAsync` 停缓冲 + 把 buffer 按序本地 replay + 恢复服务，不丢请求。
- buffer 上限（默认 512 条）/ 整体超时（默认 10s）超限即失败回滚，不做无界缓冲。

### 迁移重建必须重挂协议

从存档建出的实体如果漏了 `RegisterPlayerProtocols`，所有客户端请求都会 `method not exist`
（实现时踩过，测试抓出来了）。`Service.AttachMigratedPlayerAsync` 把「注册协议 + 子系统上线同步」
收敛成一处，登录与迁移共用 `RegisterPlayerProtocols`。

---

## 4. 新增消息 / 键 / 钩子

`hub_hub.proto`（hub 级命令，`EntityId` 留空走 `DispatchHubFrameAsync`，不进实体路由）：

| 消息 | 方向 | 用途 |
|---|---|---|
| `HubSessionReplaced` | 新 hub → 旧 hub | 顶号：踢旧连接 + 保存 + 摘除（EntityId 带旧实体） |
| `HubMigrateRequest` | 旧 → 新 | 迁移：带切割缓冲 |
| `HubMigrateForward` | 旧 → 新 | 尾批 / 墓碑转发 |
| `HubMigrateQuiet` | 旧 → 新 | 墓碑静默，可以 re-point |
| `HubMigrateResult` | 新 → 旧 | 重建结果 |

Redis：`session:{userId}`（见上）。

`Service` 钩子：`PeekUserId`（顶号创建前复用）、`OnSessionReplacedAsync`、
`OnEntityMigratingOutAsync` / `OnEntityMigratedInAsync`、`LoadOfflinePlayerAsync`（迁移重建，friend 引入）。

对外入口：`MainClass.MigrateAsync(userId, targetHubId, options)`。

---

## 5. 测试

`abelkhan/tests/ProtocolSmoke` 的 harness 支持**双 hub 节点**（各自独立的 clients / entities /
service / coordinator，共享 Redis 与同一个假 gate 路由；MiniGate 按 entityId 记录归属节点，
等价于 gate 的 `RegisterNetwork` 切换），每节点跑 `hub_hub_{hubId}_mq` 消费循环。

- 顶号 12 项：同 hub 复用同对象、旧客户端 KickOff 不重连、seq=2 指新连接、跨 hub 踢旧 + 旧实体摘除。
- 迁移 13 项：状态随迁（Score=42）、entityId 不变、墓碑期请求转发有回包、gate 归属切到 hub-1、
  re-point 后请求落到新 hub、same_hub / not_found 拒绝。
- Actor 自调度 6 项（迁移依赖的保序基础）：FIFO、异常隔离、重入、并发恰好一次。

---

## 6. 边界与后续

- 迁移窗口（约 grace + 传输，秒级）内客户端请求延迟变高但不失败。
- `LoadOfflinePlayerAsync` 返回 null（游戏没实现读档）→ 迁移失败 `not_found`，旧 hub 回滚。
- 未做：自动负载均衡 / 热点疏散、跨 Redis 前缀迁移、迁移期间的可靠消息（`entity_reliability_*`）
  特殊处理（仍按 userId 队列投递，gate 侧不区分 hub，天然可用）。
