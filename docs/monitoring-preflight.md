# 预研：给 abelkhan 加监控

> **状态：已按本预研落地（P0~P3）** —— 用的是 prometheus-net + OpenTelemetry 双路径，
> 运维/指标清单见 `docs/monitoring.md`，部署栈见 `deploy/monitoring/`。
> 下面保留预研时的原始判断与取舍（含已被实现推翻的部分，例如「/metrics 复用健康检查端口」改成了独立端口）。
>
> 结论先行：**推荐「prometheus-net 业务指标 + 现有端口加 `/metrics` + 运行时指标用 dotnet-counters」**。
> 全部可在离线缓存下落地（已验证），工作量约 1.5～2 人日，**不新增端口、不引入框架、不改线程模型**。
> 追踪（Tracing）建议这一轮不做，理由是它要改 wire protocol 或引入 OpenTelemetry 包，收益/成本比不划算。

---

## 1. 现状盘点

### 已有的

| 能力 | 位置 | 说明 |
|---|---|---|
| 健康检查 | `hub/MainClass.cs:522`、`gate/MainClass.cs:521`、`admin/MainClass.cs:102` | `GET /health` → 固定字符串 `"healthy"`，不检查依赖 |
| 在线数 | `gate/MainClass.cs:362` | gate 每 3s 把在线玩家/连接数写进 Redis（10s TTL），admin 汇总 |
| 管理大盘 | `admin/MainClass.cs:96` + `admin/wwwroot` | 在线数、总玩家、今日注册、今日充值 |
| 日志 | `core/Log.cs` | 自研，按级别写文件；**没有结构化字段、没有错误计数** |
| 定时器 | `core/TimerService.cs` | `TimerService.Tick`（Unix ms），所有子系统的时间基准 |

### 缺的（`docs/mmorpg-engine-gap-analysis.md` 已标注 P1-11）

- 没有任何指标（QPS / 延迟 / 错误率 / 队列深度）；
- `/health` 是假的：Redis 挂了、hub 主循环卡死，它照样返回 200；
- 没有慢调用可观测性：`RedisHandle.Execute`、`OnDoMsg` 这些收口点都不计时；
- 线上排障只能靠翻日志文件（而且日志在容器里会丢）。

---

## 2. 三个层次，分开评估

监控不是一个东西，是三层，成本和紧迫性完全不同：

| 层次 | 内容 | 是否改代码 | 成本 | 价值 |
|---|---|---|---|---|
| **A. 运行时** | GC、线程池、CPU、句柄、异常数 | **不改**（.NET 自带 EventCounters） | 极低 | 高（先回答「是不是进程本身有问题」） |
| **B. 业务** | QPS、P99、错误率、在线数、队列深度 | 改（但只加几十行） | 低 | 最高（游戏服真正的排障依据） |
| **C. 追踪** | 一次请求跨 gate → hub → Redis 的链路 | 改（要透传 trace id） | 高 | 中（微服务必要，当前规模未必） |

**这一轮建议做 A + B，C 留作后续。**

---

## 3. 方案对比（只针对 B 层）

| 方案 | 离线可用 | 新依赖 | 改动量 | 评价 |
|---|---|---|---|---|
| **prometheus-net 8.2.1**（核心包） | ✅ 已在 NuGet 缓存 | 1 个包 | 小 | **推荐** |
| OpenTelemetry + OTLP exporter | ❌ 缓存里没有 | 4~6 个包 | 中 | 生态最好，但要联网还原；接 Grafana/Tempo 更顺 |
| 只用 `System.Diagnostics.Metrics`（.NET 10 内置）+ 自己写 exporter | ✅ 零依赖 | 0 | 中（要自己写 Prometheus 文本导出） | 不想加包时可以接受 |
| 自己往 Redis 写计数，admin 读 | ✅ 零依赖 | 0 | 小 | 就是现在在线数那套；只适合少量低频指标，做 Histogram/分位数不行 |

### 已验证的探测结果（不是推测）

在 `/tmp/monspike` 里跑通过：

1. `prometheus-net 8.2.1` **从本地缓存就能还原**，不需要联网。
2. `prometheus-net.AspNetCore`（提供 `MapMetrics()`）**不在缓存里**，但**不需要它**：
   用核心包的 `Metrics.DefaultRegistry.CollectAndExportAsTextAsync(body)` 手写端点即可，5 行。
3. `/metrics` 可以和现有 `/health` 共用一个连接（同一个 `WebApplication`），**端口不用新增**。
4. `Counter` / `Gauge` / `Histogram` 输出格式正确（`abelkhan_*_bucket{le="0.75"} …`）。

探测代码（可直接搬进 `hub/MainClass.cs`）：

```csharp
app.MapGet("/health", () => Results.Ok("healthy"));   // 已有
app.MapGet("/metrics", async (HttpContext ctx) =>
{
    ctx.Response.ContentType = "text/plain; version=0.0.4";
    await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(ctx.Response.Body);
});
```

---

## 4. 落地设计（B 层）

### 4.1 指标清单

**入口（客户端 → hub）**

| 指标 | 类型 | 标签 | 采集点 |
|---|---|---|---|
| `abelkhan_hub_messages_total` | Counter | `method`（≈40 个固定值）, `kind`(request/notify) | `hub/BaseEntity.cs:374 OnDoMsg` |
| `abelkhan_hub_message_duration_seconds` | Histogram | `method` | 同上 |
| `abelkhan_hub_handler_errors_total` | Counter | `method` | 同上（catch 分支） |
| `abelkhan_hub_players` / `hub_connections` | Gauge | — | `hub/MainClass.cs:107 TickClients` |

**出口（hub → gate → 客户端）**

| 指标 | 类型 | 标签 | 采集点 |
|---|---|---|---|
| `abelkhan_gate_delivered_total` | Counter | `kind`(normal/reliability) | `gate/MainClass.cs:246 OnMqMsg` |
| `abelkhan_gate_ack_timeout_total` | Counter | — | `gate/MainClass.cs` `RetryReliabilityTimeout` |
| `abelkhan_gate_clients` / `gate_players` | Gauge | — | `gate/MainClass.cs:317 TickClients`（**这两个值现在就算了，只是没暴露**） |
| `abelkhan_gate_kickoff_total` | Counter | `reason` | 版本不匹配 / 顶号分支 |

**存储（最有价值的一层：一个收口点覆盖所有子系统）**

| 指标 | 类型 | 标签 | 采集点 |
|---|---|---|---|
| `abelkhan_redis_operation_duration_seconds` | Histogram | `operation`（`shop.purchase` / `friend.submit` / `leaderboard.submit` … 约 60 个固定值） | `core/RedisHandle.cs:147 Execute` |
| `abelkhan_redis_reconnect_total` | Counter | — | 同上（`Recover` 分支） |
| `abelkhan_mongo_operation_duration_seconds` | Histogram | `op`(update/find/count…) | `core/MongoProxy.cs` |
| `abelkhan_redis_pubsub_received_total` | Counter | `channel` | `RedisHandle.Subscribe` 回调 |

> `RedisHandle.Execute(string operation, …)` 的第一个参数**已经就是指标标签**，这是这个框架最好的一点：
> 加指标的改动量只有「外层包一个 `Stopwatch` + 一行 `Observe`」。

**业务（按子系统，各 1~3 个）**

| 指标 | 类型 | 采集点 | 说明 |
|---|---|---|---|
| `abelkhan_shop_settle_batch` | Gauge | `hub/MainClass.cs:367 TickShop` | 每轮结算条数（队列深度代理） |
| `abelkhan_friend_pending_confirms` | Gauge | `hub/Friend/FriendService` | 在途跨服确认数（`_pending.Count`） |
| `abelkhan_friend_reserve_swept_total` | Counter | 同上 `SweepReservesAsync` | 超时回收数（>0 说明跨服确认有问题） |
| `abelkhan_chat_broadcast_total` / `chat_dispatch_total` | Counter | `hub/Chat/ChatService` | 发布数 vs 本 hub 分发数 |
| `abelkhan_quest_events_total` | Counter | `hub/Quest/QuestService.ReportEventAsync` | 每秒事件数（= 玩法活跃度） |
| `abelkhan_guild_created_total` | Counter | `hub/Guild/PlayerGuild.cs` | 转化率 |
| `abelkhan_log_warn_total` / `log_error_total` | Counter | `core/Log.cs Output` | 零散错误的总量兜底 |

**标签基数（cardinality）控制**：所有标签都来自**代码里的常量**（方法名、Redis operation 名、channel 名），
没有 userId / guildId 这类高基数值，安全。

### 4.2 代码形态

新增 `abelkhan/core/Metrics.cs`（避免 prometheus-net 的类型漏到业务层）：

```csharp
namespace core;

/// 统一收口：业务只依赖这几个方法，换 exporter 只改这个文件
public static class Metrics
{
    public static void Count(string name, string description, params (string Key, string Value)[] labels);
    public static void Gauge(string name, string description, double value, params (string Key, string Value)[] labels);
    public static IDisposable? Time(string name, string description, params (string Key, string Value)[] labels);
}
```

落地后的调用点示例（`RedisHandle.Execute`）：

```csharp
using var _ = Metrics.Time("abelkhan_redis_operation_duration_seconds", "redis op", ("operation", operation));
```

`Log.Output` 里加两行：`if (level == "warn") Metrics.Count(...)`。

### 4.3 `/health` 顺手修成真的

现在 `/health` 恒返回 `healthy`，consul 的健康检查因此**永远通过**，Redis/Mongo 挂了也不会摘流量。
建议改成：

```
GET /health   → 200 只在「redis 可 PING + 主循环 3s 内有 tick」时返回；否则 503
GET /health/ready  → 就绪（依赖都通）
GET /health/live   → 存活（进程在跑）
```

这三条各 3~5 行，但能把 consul 的服务发现变成真正有效。

---

## 5. A 层：运行时指标（零代码）

.NET 自带 EventCounters，`dotnet-counters` 直接看，不改任何代码：

```bash
dotnet-counters monitor --process-id <hub.pid> \
  --counters System.Runtime[gc-heap-size,threadpool-thread-count,cpu-usage], \
              Microsoft.AspNetCore.Hosting[requests-per-second,current-requests]
```

**限制**：需要一个全局工具（本机没装，`dotnet tool install -g dotnet-counters` 要联网/缓存），
且只能**人在看**，进不了 Prometheus。所以它是「临时排障」，不是常驻监控。

要让 A 层也进 Prometheus，得开 `System.Diagnostics.Metrics` 的 runtime meter 或自己把
EventCounters 转成 Gauge；这属于第二步，不急。

---

## 6. C 层：追踪（建议不做）

要做端到端 trace，最小代价是**在 `CallRpc` 里加一个 trace_id 字段**，让 gate/hub 一路透传：

- 好处：定位「这条消息卡在哪一跳」；配 Tempo/Jaeger 能看到 flame graph。
- 代价：改 `common.proto`（所有 wire 格式）、所有 `WRpc.Notify/Request/Response` 调用点、
  加 OpenTelemetry 包（离线没有）、以及**每个 hub 的 actor 队列会引入 context 传播的复杂度**。
- 判断：当前是单进程 actor 模型 + Redis 收口点，**用 B 层指标 + 结构化日志足够定位**。
  等真的拆成多进程/多机房再上。

**中间态**：不做真 trace，但可以在 `OnDoMsg` 的慢调用日志里带上 `method` / `entityId` / 耗时
（阈值比如 500ms），成本几行，排障够用。

---

## 7. 分阶段计划

| 阶段 | 内容 | 改动 | 预估 |
|---|---|---|---|
| **P0** | `core/Metrics.cs` + `prometheus-net` 引用 + `/metrics` 端点（hub/gate/admin 三处）+ `/health` 下沉到真实检查 | 新增 1 文件，改 4 处 | 0.5 人日 |
| **P1** | 两个收口点：`RedisHandle.Execute`、`BaseEntity.OnDoMsg`（这两处就覆盖了 80% 的排障需求） | 2 处各 ~10 行 | 0.5 人日 |
| **P2** | 子系统指标（shop / friend / chat / quest / guild / gate 交付）+ `Log` 计数 | ~10 处 | 0.5 人日 |
| **P3** | 部署侧：`prometheus.yml` 抓取配置 + Grafana dashboard JSON（在线数 / QPS / P99 / 错误率 / Redis 延迟 heatmap） | 新增 `deploy/monitoring/` | 0.5 人日 |
| P4（可选） | 运行时指标进 Prometheus、真 trace | — | 后续 |

**回归保障**：`abelkhan/tests/ProtocolSmoke` 已经能把 hub/gate 的协议链路跑起来，
P1/P2 的指标可以用它断言（例如「跑完 45 条协议后 `abelkhan_hub_messages_total` > 45」）。
这一点很重要：**指标本身也要被测**，否则很容易出现「指标写错名字，图上永远是 0」。

---

## 8. 风险与坑

1. **`/metrics` 的安全**：它只在 `PortHealth`（consul 健康检查端口）上，那个端口本来就监听 `cfg.Ip`。
   如果 `Ip=0.0.0.0`，`/metrics` 会暴露到公网 → 建议 `/metrics` 单独绑 `127.0.0.1` 或加 token
   （`admin` 已有 `AdminToken` 的 Bearer 方案可复用）。
2. **Histogram 的 bucket 选择**：默认 bucket 是 0.005~10s，对 Redis 单次操作（通常 <2ms）太粗。
   `RedisHandle` 应该用自定义 bucket（`0.0005,0.001,0.002,0.005,0.01,0.05,0.1,0.5,1`）。
3. **`Log.Output` 是 `lock` 里写文件**，在它里面加指标要小心别做重活（只做 `Interlocked` 级操作）。
4. **`OnDoMsg` 在 actor 队列里串行执行**，加 `Stopwatch` 是安全的，但**不要**在里面对 Redis 再发请求
   （会放大延迟）。
5. **多实例聚合**：hub 是无状态多副本，Prometheus 侧必须保留 `instance` 标签做 `sum by (job)`，
   不要图省事在服务端做聚合。

---

## 9. 待决策

1. **exporter 选型**：prometheus-net（离线可用，推荐）/ OpenTelemetry（生态好，需联网还原）/
   内置 `System.Diagnostics.Metrics` + 自写导出（零依赖）。
2. **这一轮做到 P2 还是 P3**（只出指标，还是连 Grafana dashboard 一起）。
3. **`/metrics` 的暴露方式**：只在内网/健康检查端口（默认）还是加鉴权。
4. 追踪（C 层）确认推到后续？

---

## 附：本次预研已验证的事实清单

- [x] `prometheus-net 8.2.1` 在本地 NuGet 缓存里，**离线可还原**（`~/.nuget/packages/prometheus-net/8.2.1`）
- [x] `prometheus-net.AspNetCore`（`MapMetrics()`）**不在**缓存里 → 用核心包手写端点
- [x] `/metrics` 与 `/health` 共用一个 `WebApplication`/端口，探测通过
- [x] Counter / Gauge / Histogram 文本输出格式正确
- [x] `core.csproj` 已有 `FrameworkReference Microsoft.AspNetCore.App` → hub/gate **不用新增框架引用**
- [x] 收口点确认：`RedisHandle.Execute(operation, …)` 的 `operation` 参数天然就是指标标签
- [x] `gate/MainClass.cs:317 TickClients` 已在计算在线数，只是没暴露成指标
- [ ] 运行时指标进 Prometheus（需 `dotnet-counters` 工具或自写 EventCounters→Gauge，未验证）
- [ ] Grafana dashboard（未验证，属于 P3 交付物）
