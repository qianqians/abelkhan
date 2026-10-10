# abelkhan → MMORPG 服务器引擎：差距分析与补齐清单

> 基于当前仓库代码（`abelkhan/` 下 6 个 C# 工程 + `client/` 客户端）逐文件通读得出。
> 所有结论均标注了文件与行号，可逐条复核。

---

## 一、当前代码基线（已确认的事实）

### 1.1 工程结构

| 工程 | 类型 | 实际内容 |
|---|---|---|
| `abelkhan/core` | 类库 | 网络（TCP/WebSocket）、Redis、Mongo、定时器、日志 |
| `abelkhan/engine` | 类库 | `WRpc` 报文封装 + `Result<T,E>` |
| `abelkhan/proto` | 类库 | protoc 生成的 C# 报文类型 |
| `abelkhan/consts` | 类库 | 报文名字符串常量 |
| `abelkhan/gate` | **可执行** | 网关：客户端接入 + 转发 |
| `abelkhan/hub` | **类库** | 游戏逻辑宿主框架——**没有 `Main`，不是可执行程序** |
| `client` | 类库 | 客户端框架：WebSocket 传输 + 自带拆包 + 断线重连 + 实体（Entity/Player）收发与重同步 |

`hub` 只有 `MainClass.RunMain(string[])`（`abelkhan/hub/MainClass.cs:236`），
没有 `static void Main`，也没有 `OutputType=Exe`，因此**当前 hub 进程无法启动**。

依赖栈：.NET 10 / Consul 1.8.0 / StackExchange.Redis 3.1.13 / MongoDB.Driver 3.11.2 /
Newtonsoft.Json / Nito.AsyncEx / Microsoft.IO.RecyclableMemoryStream（`abelkhan/core/core.csproj:9-17`）。

### 1.2 传输层

- TCP 帧格式：4 字节**小端**长度前缀 + payload，**单帧上限 65536 字节**，超限直接判为错误断开
  （`abelkhan/core/OnReceive.cs:25-33`）。
- 发送端自行拼长度前缀（`abelkhan/core/TcpNetwork.cs:13-18`）；
  **WebSocket 内部又套了一层同样的 4 字节前缀**（`abelkhan/core/WebSocketNetwork.cs:16-22`），
  与 WebSocket 自身消息边界冗余。
- 接收路径：`reader.ReadAsync()` → `buffer.ToArray()` 全量拷贝 → `OnReceive.Receive` 逐帧切分
  （`abelkhan/core/TcpAcceptService.cs:29-42`、`TcpConnectService.cs:22-35`）——**零拷贝管线被自己浪费掉了**。
- `TcpNetwork.Send` 用**同步 `Socket.Send`** 并在未写完时 `Task.Delay(1)` 轮询重试
  （`abelkhan/core/TcpNetwork.cs:20-29`）：每帧至少 1ms 延迟，且同步阻塞在异步链路上。
- 出站连接 `TcpConnectService.Connect` 也是**同步阻塞 `s.Connect()`**，失败仅记日志，无重连
  （`abelkhan/core/TcpConnectService.cs:48-62`）。
- TLS 只在客户端接入侧通过 Kestrel 提供（`abelkhan/core/WebSocketAcceptService.cs:45-54`）；
  **gate ↔ hub 内网链路是明文 TCP**（`abelkhan/gate/MainClass.cs:272-283`）。
- 网关健康检查端口用匿名 `WebApplication.Create()` 起 Kestrel（`abelkhan/gate/MainClass.cs:324-326`）。

### 1.3 RPC 与协议

- 统一信封 `Msg{ oneof: req | rsp | notify | heart_beats }`（`proto/common.proto:26-33`）。
- 方法名是字符串 `CallRpc.proto_name`，**内容二次序列化**（`proto/common.proto:3-6`）——
  即 protobuf 里再塞 protobuf bytes，多一次序列化/解析开销。
- `WRpc.Request/Response/Notify<T>()` 仅负责组包（`abelkhan/engine/WRpc.cs:6-60`）；
  分发是**手写 `switch (proto_name)`**（`abelkhan/gate/ClientMsgHandle.cs:38`、
  `HubMsgHandle.cs:213`、`abelkhan/hub/GateMsgHandle.cs:74`）。
- 业务侧注册是**委托字典**：`RegisterNotify/RegisterRequest`（`abelkhan/hub/BaseEntity.cs:230-347`）。
- `Response` 有 `err_msg` 字段（`proto/common.proto:16-20`），
  但节点间失败**不产生任何超时或错误回包**：`_requestCallbacks` 没有定时清理
  （`abelkhan/hub/BaseEntity.cs:96-123`），请求一旦丢失，调用方 `await` **永久挂起**。
- `proto/hub_hub.proto` 只定义了 `TakeOverUser`（跨服接管），
  在 C# 侧**只有生成的类型，没有任何调用点**（grep 确认仅出现在 `abelkhan/proto/HubHub.cs`）。

### 1.4 gate 现状

- 接入流程：分配 connId → 下发 `NotifyConnID` → 向 Redis list `EnterService` 投递请求
  （`abelkhan/gate/MainClass.cs:286-321`）。
- 入向：hub→client 走 `GateForwardHubNotifyClient`，**两条通道**：
  直连 TCP（`_dictEntityNetwork`）与 Redis list MQ
  （`abelkhan/gate/Clients.cs:32-47`、`abelkhan/gate/MainClass.cs:49-161`）。
- 可靠消息：`entity_reliability_{userId}_client_mq` 用 `Front` 窥视队头 + `DeleteListElem` 出队实现至少一次。
  hub 投递时给消息赋唯一 `seq`（`GateForwardHubNotifyClientMq.seq`，重投沿用 Redis 里同一份消息所以 seq 不变），
  gate 透传成 `HubNotifyClientMq.seq`，客户端处理完回 `AckReliabilityMsg{entity_id, seq}`；
  gate 出队前拿 ack 的 seq 与队头比对，并且**按 userId 串行处理 ack**（64 桶 `SemaphoreSlim`），
  避免重复/迟到的 ack 把还没投递的下一条弹掉。老客户端不回填 seq（=0）时退回按 `entity_id` 比对。
- 可靠消息重投：`ReliabilityAckTimeoutMs`（10s）内没等到 ack，就把 userId 重新入队再投一次
  （`gate/MainClass.cs` 的 `RetryReliabilityTimeout`）；否则 userId 只靠 ack 回队，ack 一丢该用户就永久停摆。
  投递循环整体带 try/catch，队头脏数据不会打死投递线程。
- 断线清理：3 秒扫一次，`10_000ms` 无事件则关闭（`abelkhan/gate/MainClass.cs:236-260`）；
  **hub 侧阈值是 8000ms**（`abelkhan/hub/MainClass.cs:46-65`）——两边不一致，
  且超时只关闭连接，**不通知实体做存盘/下线**。
- 客户端到实体的路由：遍历全部 clients 按 `userId` 线性查找
  （`abelkhan/gate/MainClass.cs:190-202`）；hub 内按 `connId` 反查 client 也是全表遍历
  （`abelkhan/hub/BaseEntity.cs:216-228`）——**O(N) 查找在热路径上**。

### 1.5 hub 现状

**Actor 调度器没有启动。** `Actor` 有 `PostTask` 与 `Run()`
（`abelkhan/hub/Actor.cs:26-44`），`PostTask` 在三处被调用
（`abelkhan/hub/GateMsgHandle.cs:159,179,196`），但 **`.Run()` 在整个仓库中没有任何调用点**
（grep `\.Run\(\)` 无匹配）。后果链条：

1. 客户端所有请求进入 `_jobs` 队列后**永不执行**；
2. `BaseEntity._requestCallbacks` 永不回调（`BaseEntity.cs:96-138`），
   `Request<T0,T1>` 返回的 Task 永不完成（`BaseEntity.cs:122`）；
3. `OnResponse` 只在 `GateMsgHandle.OnClientResponseHub` 里被 `PostTask` 投递
   （`GateMsgHandle.cs:175-188`），同样不执行 → **实体发起的 request 永远等不到客户端的回包**；
4. 注意：客户端的 `ack_reliability_msg` 是 gate 在网络回调线程里直接处理的
   （`gate/ClientMsgHandle.cs`），**不经过 hub 的 Actor**，所以可靠队列的出队不受 P0-1 影响
   （出队前会校验 seq 与队头一致，见 1.4）。

**游戏内容为零。** 全仓库只有抽象基类：

- `Service` 抽象，`CreateEntity` 无实现（`abelkhan/hub/Service.cs:6-19`）；
- `Player` / `Entity` / `GlobalEntity` 均抽象，无任何具体子类
  （grep `: Player` / `: Entity` / `: GlobalEntity` 无匹配）；
- `MainClass.RegisterService` **从未被调用**（`abelkhan/hub/MainClass.cs:67-70`）；
- `Group` 只在 `Service` 内 `new` 一次（`abelkhan/hub/Service.cs:8`），
  没有任何 Service 实例，等于死代码。

**持久化薄弱。** `Player.Save()` / `GlobalEntity.Save()` 都是 `async void` 即发即忘
（`abelkhan/hub/Player.cs:29-47`、`GlobalEntity.cs:21-39`）：

- 无脏标记、无批量、无定时存盘、无重试；
- 写失败只打日志（`Player.cs:40`）；
- **从未被任何代码调用**（grep 确认无调用点）；
- `MongoProxy.Update` 返回**恒为 true**（`abelkhan/core/MongoProxy.cs:100-102`），
  `Save`/`Remove` 同样恒 true（`MongoProxy.cs:84,171`），错误一律吞掉；
- `CheckIntGuid` / `get_guid` 是 `async void` 与无索引保证的自增（`MongoProxy.cs:51-73,174-186`）。

**其他缺陷：**

- `Client.UserId/GateName/ConnId` 的 **getter 有副作用**（刷新 `LastEventTime`）
  （`abelkhan/hub/Client.cs:11-37`），任何日志/循环读取都会阻止超时清理。
- `Group.Notify` 遍历一轮就把同一个 `CallRpc` 复用给多个客户端（`abelkhan/hub/Group.cs:101-124`）。
- `Group.Join/Leave` 没有与 `MainClass._entities` 联动，也没有 AOI 概念。
- 关闭流程只置 `_isRun=false` 并关监听（`gate/MainClass.cs:354-359`、`hub/MainClass.cs:168-172`），
  **不落盘、不排空队列、不注销 Consul**。

### 1.6 服务编排

- gate 与 hub 都向 Consul 注册，服务名固定 `"gate"`（`gate/MainClass.cs:218-233`）。
- hub 用 `ConsulServiceWatcher` 监听 `gate` 与 `db_proxy`（`hub/ConsulServiceWatcher.cs:29-36`），
  但**只对 `gate` 建立 TCP 连接**（`hub/MainClass.cs:197-203`），
  `db_proxy` **没有对应的工程/进程**（全仓库 grep 仅此一处提及）。
- 发现是**只加不减**：`CheckServiceInstancesAsync` 用 `_knownServiceInstances[serviceName] = currentIds`
  覆盖集合，但没有下线/迁移处理，也没有重连。
- hub 之间**没有直接通信链路**（`TcpConnectService _serviceGate` 只连 gate，
  `abelkhan/hub/MainClass.cs:35,186-203`），跨 hub 只能靠 Redis list。

### 1.7 客户端现状（`client/`）

`client` 是**独立的类库工程**，只引用 `engine`（→ `proto`）与 `consts`，**不引用服务端的 `core`**，
所以不会把 MongoDB/Consul/Redis/AspNetCore 带进客户端。

- 传输：**只支持 WebSocket**（`client/WebSocketNetwork.cs`），自带拆包
  （`client/ReceiveBuffer.cs`，与服务端 `core/OnReceive.cs` 相同的 4 字节小端长度前缀 + 64KB 上限）。
  Kestrel 侧是 `UseHttps(pfx, password)`（`core/WebSocketAcceptService.cs:45-54`），
  所以地址必须是 `wss://`，自签证书需要 `ClientConfig.IgnoreCertificate`。
- 断线重连：读循环结束/发送失败 → 统一断线处理（失败所有未完成的 request + `OnDisconnected`）
  → 按 `ReconnectIntervalMs` 自动重连（`AutoReconnect` 可关；收到 `KickOff` 不重连）。
- 重连实体重同步：重连成功把所有本地实体标记为"未认领"，服务端用
  `CreatePlayerEntity`/`CreateRemoteEntity`/`RefreshEntity` 重新下发一遍
  （本地已存在的对象只走 `OnRefresh`，不重建），`ResyncTimeoutMs` 后仍未被认领的实体本地删除
  （`client/Client.cs` 的 `StartResync`/`ResyncCheck`）。
- 实体模型：`Entity`（远端实体）与 `Player`（自己的玩家实体，对应 hub 侧 `hub.Player`）两个基类；
  工厂 `Service.CreatePlayerEntity → Player`、`Service.CreateRemoteEntity → Entity?`。
  实体自带 request/notify 注册与自动回包（Ok→response，Err/异常→error），每个实体一条串行队列保证顺序。
- 可靠消息：收到 `HubNotifyClientMq(need_ack)` 处理完回 `AckReliabilityMsg{entity_id, seq}`；
  找不到对应实体时也会回 ack，避免 gate 每 10s 重投。
- 已知缺口：协议里 `CreatePlayerEntity` 不带 `user_id`，重连用的账号需要游戏从 `argv` 解析后写进 `Player.UserId`；
  没有压缩/批量/背压；"半死"连接只能靠 gate 侧 10s 超时断开。

---

## 二、缺口清单（按严重度排序）

### P0 — 不修则引擎跑不起来

| # | 缺口 | 证据 | 说明 |
|---|---|---|---|
| P0-1 | Actor 分区调度器未接入 | `hub/Actor.cs:38-44`，无 `.Run()` 调用点 | 需要 per-entity 调度循环（每实体单线程 mailbox），否则消息全部丢失 |
| P0-2 | hub 不是可执行进程、无入口 | `hub/MainClass.cs:236` | 补 `Program.cs` + `<OutputType>Exe</OutputType>` |
| P0-3 | 无任何具体 Service/Player/Entity | 全仓库无子类；`RegisterService` 无调用 | 需要业务框架层（至少一个可继承的示例服务） |
| P0-4 | 超时与断线不触达实体 | `gate/MainClass.cs:247-254`、`hub/BaseEntity.cs` | 需要 `OnClientDisconnect` → 实体下线/存盘钩子 |
| P0-5 | 请求无超时、无回滚 | `hub/BaseEntity.cs:96-123` | `_requestCallbacks` 需带 deadline 的定时清理 + `Err("timeout")` |
| P0-6 | 持久化事实上不存在 | `hub/Player.cs:29`（从未调用） | 需要存档生命周期（登录加载 / 定时 / 下线落盘 / 崩溃恢复） |
| P0-7 | ~~可靠队列只增不减~~ **已修** | `gate/ClientMsgHandle.cs`、`gate/MainClass.cs` | 现在：hub 赋 `seq` → gate 与队头比对后才出队、按 userId 串行 ack、10s 超时重投、轮询队列去重、投递循环带 try/catch。残留：多 gate 实例共享同一个 `entity_reliability_*` list 时会互相抢（需要归属/租约） |
| P0-8 | `MongoProxy` 恒返回成功 | `core/MongoProxy.cs:84,102,171` | 无法感知写失败，需真实返回结果 + 异常传播 |

### P1 — 网络与会话基础

| # | 缺口 | 说明 |
|---|---|---|
| P1-1 | **会话/断线重连：客户端侧已就绪，服务端待补** | 服务端：重连只重建 `Client`（`hub/MainClass.cs:72-76`），不校验旧连接、不迁移实体网络绑定、**不重新下发 `HubCreatePlayerEntity`**；`GateForwardClientRequestReconnect` 无幂等/去重。客户端（`client/`）已实现 WebSocket + 拆包 + 断线自动重连 + 按 `CreatePlayerEntity`/`CreateRemoteEntity`/`RefreshEntity`/`DeleteRemoteEntity` 重同步场景对象（未重新认领的实体会在 `ResyncTimeoutMs` 后本地删掉），只等服务端的重连 hook |
| P1-2 | **无登录鉴权层** | 无账号校验、无 token、无 `ClientRequestService` 的 `argv` 语义定义；`EnterService` 硬编码单服务（`gate/MainClass.cs:303-310`） |
| P1-3 | **反外挂/输入合法性为零** | 客户端可任意指定 `entity_id` 发 `ClientNotifyHub`（`gate/ClientMsgHandle.cs:66-77`），服务端不校验归属，等于**任意实体越权调用**；无频率限制、无合法性校验 |
| P1-4 | 单帧 64KB 硬上限 | `core/OnReceive.cs:30`；大包（背包/邮件列表）需要分片或提高上限并做流控 |
| P1-5 | 无背压/流量控制 | 发送侧无队列水位、无丢弃策略；慢客户端会拖住 `Task.Delay(1)` 轮询 |
| P1-6 | 无压缩 | ZstdSharp/Snappier 已在依赖里（见 `gate/bin`），但代码未使用；MMO 大量冗余小包需要批量+压缩 |
| P1-7 | 无连接数上限/防 DDoS | `TcpAcceptService` 无最大连接数、无 accept 限速、无 IP 黑名单（`core/TcpAcceptService.cs:55-75`） |
| P1-8 | 定时器精度与漂移 | 主循环固定 16ms 轮询（`gate/MainClass.cs:331-340`）；`TimerService` 用 `DateTime.UtcNow` 毫秒（`core/TimerService.cs:8,20`）→ 系统时钟回拨会破坏所有定时；无单调时钟 |
| P1-9 | `SortedDictionary` 遍历、Poll 双次刷新 | `core/TimerService.cs:60-86,437-442`，O(n) 遍历全部定时器，规模大时是热点 |
| P1-10 | 日志为同步文件锁 | `core/Log.cs:58-90` 全局 lock + AutoFlush，每行一次系统调用，高并发下会成为瓶颈；无异步日志、无分级输出、无日志采集格式（JSON） |
| P1-11 | 无指标/链路追踪 | 无 Prometheus/OpenTelemetry；只有 `/health` 返回固定字符串（`gate/MainClass.cs:325`） |

### P2 — MMORPG 核心玩法系统（当前完全缺失）

| # | 系统 | 需要补的内容 |
|---|---|---|
| P2-1 | **地图 / 场景 / 副本管理** | `Scene`/`Map`/`Instance` 生命周期与注册表；每场景一个 Actor；场景间实体迁移；副本创建/销毁/超时回收 |
| P2-2 | **AOI（兴趣管理）** | 网格（grid）/九宫格或十字链表；进入/离开视野事件；`CreateRemoteEntity`/`DeleteRemoteEntity` 已有协议（`proto/client.proto:10-18`）但没有驱动它的 AOI 实现 |
| P2-3 | **移动同步** | 寻路（A*/NavMesh）、移动状态机、位置校验（速度上限）、位置广播与插值、服务器权威（防瞬移） |
| P2-4 | **战斗系统框架** | 技能/效果（buff）系统、伤害结算、命中判定、CD、仇恨、战斗日志；需要可配置的技能表与确定性结算顺序 |
| P2-5 | **属性/数值系统** | 属性管线（基础值→加成→最终值）、公式可配置、成长曲线 |
| P2-6 | **道具/背包/装备** | 容器模型、堆叠、绑定、强化、掉落到地面物品、拾取保护 |
| P2-7 | **任务 / 成就 / 活动** | 条件-触发器模型、进度追踪、周期性活动时间轴（可用 `TimerService` 的 LoopDay/LoopWeek 接口） |
| P2-8 | **社交** | 好友、黑名单、组队、公会（组织架构 + 权限 + 仓库）、聊天频道（世界/地图/队伍/私聊/跨服） |
| P2-9 | **交易 / 邮件 / 拍卖行** | 需要**事务性**操作与防刷（当前 Mongo 无事务使用） |
| P2-10 | **匹配 / 排队** | 竞技场、战场、副本组队撮合——需要一个全局撮合服务 |
| P2-11 | **怪物 / AI / 刷怪** | 刷怪点与刷新计时、AI 状态机/行为树、巡逻与仇恨 |
| P2-12 | **掉落与随机** | 权重掉落表、保底、随机数一致性（`core/RandomHelper.cs` 需检查是否可用且可复现） |
| P2-13 | **排行榜 / 统计** | 定时快照、跨服榜 |
| P2-14 | **世界事件广播** | 现有 `GateForwardHubCallGlobal`（`proto/gate_hub.proto:56-59`）是**无差别全服广播**给单个 gate 的所有客户端，不是真正的世界频道/兴趣广播 |

### P3 — 服务编排、跨服与运维

| # | 缺口 | 说明 |
|---|---|---|
| P3-1 | hub↔hub 无直连通道 | `proto/hub_hub.proto` 的 `TakeOverUser` 无实现；跨服/跨地图交互当前只能走 Redis list，延迟与吞吐都不合适 |
| P3-2 | 无服务网格的容量与路由 | 服务名写死、无一致性哈希、无按地图/分线分片、无灰度与版本路由（`consts` 里没有版本字段） |
| P3-3 | 服务下线/迁移无处理 | `ConsulServiceWatcher` 只处理新增（`hub/ConsulServiceWatcher.cs:63-75`），实例消失后 `_gates` 残留、无重连、无断线期间的请求缓冲 |
| P3-4 | 无优雅停机 | gate/hub 的 `Stop()` 不排空、不落盘、不注销 Consul（`gate/MainClass.cs:354-359`、`hub/MainClass.cs:168-172`） |
| P3-5 | `db_proxy` 进程缺失 | `ConsulServiceWatcher.cs:32` 引用了不存在的服务；README 声称有 dbproxy，实际没有对应工程 |
| P3-6 | 数据库层能力不足 | `MongoProxy` 无事务/无 session、无批量写、无分页游标复用、无重试策略、无连接池配置、无分库分表；缓存（Redis）与 DB 无一致性策略 |
| P3-7 | 配置系统 | 仅从 `args[0]` 读一个 JSON 文件（`gate/MainClass.cs:375-389`、`hub/MainClass.cs:238-252`），无热更新、无 schema 校验、无默认值合并、无敏感信息加密 |
| P3-8 | 数据版本与迁移 | 存档无 `schema_version`、无升级脚本；表结构变更无法灰度 |
| P3-9 | 无热更新/热重载 | 配置表、技能表、掉落表无法运行时重载 |
| P3-10 | GM / 后台工具 | 无 GM 命令通道、无后台 HTTP API、无封禁/补发/查档接口 |
| P3-11 | 无审计与安全日志 | 登录、交易、充值、管理员操作无审计流水 |
| P3-12 | 无压测/仿真客户端 | 无法评估单 hub 承载量 |
| P3-13 | 无测试与 CI | 仓库无任何测试工程、无 CI 配置 |
| P3-14 | 可观测性 | 无 QPS/延迟/在线数/队列深度指标，无 tracing，无告警 |

---

## 三、建议的实施顺序（分阶段）

**阶段 0：让框架真的能跑（1–2 周）**
1. 补 `hub` 的 `Program.cs`，改为可执行程序。
2. 接入 Actor 调度：`MainClass` 里为每个 `BaseEntity` 启动 drain 协程（或引入中央调度轮询），
   并给 mailbox 设上限与溢出策略。
3. 给 `_requestCallbacks` 加超时清理（deadline + `TimerService` 定时扫描）。
4. 打通 `OnClientDisconnect` → 实体下线钩子 → 存档。
5. `MongoProxy` 返回值改为真实结果，异常向上抛。
6. 写一个最小可玩示例：一个 `Service` + `Player` + echo 请求，端到端（客户端→gate→hub→回包）。

**阶段 1：网络与会话加固（2–4 周）**
- 重连幂等、会话令牌、输入合法性校验（实体归属校验，堵住 `ClientNotifyHub` 越权）。
- 发送队列 + 背压 + 批量打包 + 可选压缩。
- 单调时钟定时器；异步日志；Prometheus 指标（在线数、QPS、P99、队列深度）。
- 优雅停机：先摘 Consul → 拒新连接 → 排空 → 全量落盘 → 退出。

**阶段 2：世界与玩法骨架（1–2 月）**
- 场景/地图/副本管理器 + AOI 网格 + 移动同步（含位置校验）。
- 战斗/属性/技能框架 + 配置表加载（支持热重载）。
- 道具背包、任务、掉落。

**阶段 3：服务化与跨服（1–2 月）**
- hub↔hub 直连 RPC，实现 `TakeOverUser`（跨服/换线接管）。
- 分片与路由（按地图/玩家 ID 一致性哈希），服务发现的下线与重连处理。
- 社交、组队、公会、聊天频道、交易/邮件事务化。

**阶段 4：运维与质量**
- GM/后台 API、审计日志、数据迁移框架、压测机器人、单元/集成测试、CI。

---

## 四、最关键的 5 个结论

1. **Actor 调度器没启动**（`abelkhan/hub/Actor.cs:38` 无调用点）——这是当前最致命的问题，
   所有客户端请求都进队列后石沉大海。
2. **hub 目前不是一个能跑的进程，且没有任何游戏内容**——只有抽象基类，
   `RegisterService` 从未被调用。
3. **存档路径事实上不存在**——`Player.Save()` 从未被调用，且 `MongoProxy` 恒返回成功。
4. **安全模型缺失**——客户端可直接指定 `entity_id` 调用任意实体，
   反外挂、频率限制、鉴权全部没有。
5. **MMO 的核心（地图/场景/AOI/移动/战斗）一行都没有**——
   协议里预留了 `CreateRemoteEntity`/`DeleteRemoteEntity`/`RefreshEntity`，
   但没有任何 AOI 实现去驱动它们。

---

*生成方式：逐文件通读 `abelkhan/` 下 6 个工程的全部 `.cs` 源码与 `proto/` 下 6 个 `.proto`，
所有路径与行号均可复核。*
