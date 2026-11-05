# 监控（指标 / 健康检查 / trace）

实现：`abelkhan/core/Metrics.cs`（prometheus-net）、`abelkhan/core/Tracing.cs`（OpenTelemetry）、
`abelkhan/core/MonitoringHost.cs`（独立 `/metrics` 监听）、`abelkhan/core/ServiceHealth.cs`（真健康检查）。
部署栈：`deploy/monitoring/`（Prometheus + Grafana + OTel Collector）。
预研与取舍记录：`docs/monitoring-preflight.md`。

---

## 1. 两条导出路径

| 方向 | 技术 | 什么时候开 | 内容 |
|---|---|---|---|
| **拉** | prometheus-net → `/metrics` | `Monitoring.PrometheusPort > 0` | 全部业务指标 + 自算的运行时指标 |
| **推** | OpenTelemetry OTLP → Collector | `Monitoring.OtlpEndpoint` 非空 | trace（进程内 span）+ .NET 运行时指标 |

两条互相独立：**默认都不开**，不改配置不会多开端口、也不会去连不存在的 collector。

`/metrics` 是**独立端口**（默认 `127.0.0.1`），刻意不复用健康检查端口 —— 那个端口绑的是
`cfg.Ip`（常常 0.0.0.0），把指标挂上去等于暴露到公网。

---

## 2. 配置

```json
"Monitoring": {
  "Enabled": true,
  "ServiceName": "abelkhan-hub",
  "NodeId": "hub-1",
  "PrometheusPort": 9101,
  "PrometheusHost": "127.0.0.1",
  "OtlpEndpoint": "",
  "TraceSampleRatio": 1.0,
  "SlowLogThresholdMs": 500
}
```

| 字段 | 说明 |
|---|---|
| `Enabled` | 总开关；false 时 `Metrics.*` / `Tracing.*` 全是空操作 |
| `ServiceName` | OTel resource 的 `service.name` |
| `NodeId` | 作为所有指标的 `node` 标签，多副本区分 |
| `PrometheusPort` | 0 = 不开 `/metrics`（默认）；生产建议 9101/9111/9121 |
| `PrometheusHost` | 默认回环；容器外采集时改 `0.0.0.0` + 网络策略 |
| `OtlpEndpoint` | 如 `http://127.0.0.1:4317`；空 = 不导出 trace |
| `TraceSampleRatio` | trace 采样率 |
| `SlowLogThresholdMs` | 报文处理超过它就打一条 warn（带 entityId/method/耗时），0 = 关 |

`admin/config.example.json` 里已有完整示例。

---

## 3. 端点

| 路径 | 端口 | 说明 |
|---|---|---|
| `/metrics` | `Monitoring.PrometheusPort` | Prometheus 文本格式（含全部指标） |
| `/health` | `Monitoring.PrometheusPort` **和** 原 `PortHealth` | 真检查：主循环 10s 内有跑 + 依赖探针（Redis PING）通过；否则 **503** |

> `/health` 变严了之后，consul 会真的把卡死/连不上 Redis 的实例摘掉 —— 这是这次改动的重点之一。
> 如果发现抖动，先看 `Monitoring` 之外的 `ServiceHealth.CheckAsync(maxIdleMs)`（默认 10s）。

---

## 4. 指标清单

所有指标都带 `node` 标签。标签值全部来自代码里的常量，**没有 userId 这类高基数维度**
（`ProtocolSmoke` 里有一条断言专门守这个）。

### 4.1 入口：hub 报文（收口点 `hub/BaseEntity.OnDoMsg`）

| 指标 | 类型 | 标签 |
|---|---|---|
| `abelkhan_hub_messages_total` | counter | `method`, `kind`(request/notify) |
| `abelkhan_hub_message_duration_seconds` | histogram | `method` |
| `abelkhan_hub_handler_errors_total` | counter | `method` |
| `abelkhan_hub_message_unrouted_total` | counter | `method`（没有 handler 的方法名 —— 配错协议名会在这里出现） |
| `abelkhan_hub_clients` | gauge | — |

`method` 就是 `Consts.ClientXxx` 那批常量，约 40 个固定值。

### 4.2 存储（收口点 `core/RedisHandle.Execute`）

| 指标 | 类型 | 标签 |
|---|---|---|
| `abelkhan_redis_operation_duration_seconds` | histogram | `operation`（`shop.purchase` / `friend.submit` / `leaderboard.submit`…） |
| `abelkhan_redis_reconnect_total` | counter | `operation` |

`operation` 是 `Execute` 的第一个参数，本来就是有限集合，**天然可当标签**。
直方图用**自定义细桶**（`0.0005 … 5s`）：默认桶从 5ms 起，Redis 单次操作根本落不进去。

### 4.3 出口：gate 投递

| 指标 | 类型 | 标签 |
|---|---|---|
| `abelkhan_gate_players` / `abelkhan_gate_clients` | gauge | — |
| `abelkhan_gate_delivered_total` | counter | `kind`(normal/reliability) |

### 4.4 业务

| 指标 | 类型 | 标签 | 关注点 |
|---|---|---|---|
| `abelkhan_friend_pending_confirms` | gauge | — | 在途跨服确认堆积 |
| `abelkhan_friend_reserve_swept_total` | counter | — | **&gt;0 说明跨服确认超时了**，最该告警的信号 |
| `abelkhan_chat_broadcast_total` | counter | `channel` | 发布速率 |
| `abelkhan_chat_dispatch_total` | counter | `channel` | 本 hub 分发到的人数之和 |
| `abelkhan_chat_private_total` | counter | — | 私聊量 |
| `abelkhan_quest_events_total` | counter | `type` | 玩法活跃度 |
| `abelkhan_quest_completed_total` | counter | — | 任务转化 |
| `abelkhan_guild_created_total` | counter | — | 帮会创建速率 |
| `abelkhan_shop_settle_batch` | gauge | — | 每轮结算条数（队列深度代理） |

### 4.5 兜底 / 运行时

| 指标 | 类型 | 说明 |
|---|---|---|
| `abelkhan_log_warn_total` / `abelkhan_log_error_total` | counter | `Log` 里 warn/err 的总量 |
| `abelkhan_runtime_heap_bytes` | gauge | GC 托管堆 |
| `abelkhan_runtime_workingset_bytes` | gauge | 进程工作集 |
| `abelkhan_runtime_threadpool_threads` / `_queue` | gauge | 线程池线程数 / 排队数 |
| `abelkhan_runtime_gc_gen0_total` / `_gen2_total` | gauge | GC 次数 |
| `abelkhan_runtime_uptime_seconds` | gauge | 进程运行时长 |

运行时那批是**抓取时现算**的（`AddBeforeCollectCallback`），零维护成本。

---

## 5. trace（OpenTelemetry）

`Tracing.Source`（`ActivitySource("abelkhan")`）下有两类 span：

- `hub.{method}`（`ActivityKind.Server`）：标签 `rpc.method` / `rpc.kind` / `entity.type`；
  超慢调用会带 `slow=true`。
- `redis.{operation}`（`ActivityKind.Client`）：所有 Redis 命令的耗时，挂在报文 span 下面。

**目前是进程内的 trace**：一个 hub 报文处理 + 它引发的 Redis 操作在同一条链上。
跨进程（客户端 → gate → hub）需要把 `trace_id` 放进 `CallRpc`，那要改 `common.proto`（wire 格式），
属于后续工作：

```proto
message CallRpc {
  string proto_name = 1;
  bytes content = 2;
  string trace_id = 3;     // 新增：gate/hub 透传，实现端到端链路
  string span_id = 4;
}
```

> 注意 wire 兼容：新字段是可选的，旧版本会忽略；但老客户端发的没有 trace_id，链路会断在那一跳。

---

## 6. 部署与看板

```bash
cd deploy/monitoring && docker compose up -d
# Grafana    http://localhost:3000   (dashboard: Abelkhan / Abelkhan 总览)
# Prometheus http://localhost:9090
```

看板预置 14 个面板：在线玩家 / 连接数 / QPS / Handler 错误率 / Redis 重连 / 好友确认超时回收 /
报文 QPS Top10 / 延迟 P50·P95·P99 / Redis P99（按 operation）/ Redis QPS / 业务指标 / 日志 warn·err /
运行时 / 商城结算与在途确认。

### 建议的告警规则（可放到 `prometheus/alert.rules.yml`）

| 告警 | 表达式 |
|---|---|
| 在线数骤降 | `sum(abelkhan_gate_players) < 0.5 * avg_over_time(sum(abelkhan_gate_players)[15m:1m])` |
| 错误率过高 | `sum(rate(abelkhan_hub_handler_errors_total[5m])) / sum(rate(abelkhan_hub_messages_total[5m])) > 0.05` |
| P99 过高 | `histogram_quantile(0.99, sum by (le) (rate(abelkhan_hub_message_duration_seconds_bucket[5m]))) > 1` |
| Redis 延迟异常 | `histogram_quantile(0.99, sum by (le) (rate(abelkhan_redis_operation_duration_seconds_bucket[5m]))) > 0.1` |
| Redis 断连 | `increase(abelkhan_redis_reconnect_total[10m]) > 0` |
| 好友跨服确认超时 | `increase(abelkhan_friend_reserve_swept_total[10m]) > 1` |
| 实例卡死 | `up{job=~"abelkhan-.*"} == 0` 或 `/health` 返回 503 |

---

## 7. 已知边界

- **没有跨进程 trace**（见第 5 节，要改 wire 格式）。
- **MongoDB 没有指标**：`MongoProxy` 还没包计时（下一处收口点，做法和 `RedisHandle` 一样）。
- `abelkhan_redis_operation_duration_seconds_count` 之类的 `_count`/`_bucket` 由 prometheus-net 自动生成，
  别手工创建同名 counter。
- **指标本身有回归测试**：`abelkhan/tests/ProtocolSmoke` 的「监控：/metrics 输出与指标计数」一节，
  会真的抓一份 `/metrics` 文本，断言报文计数 ≥ 40、Redis 直方图有样本、细桶生效、无高基数标签。
  这一节曾经抓出过一个真实 bug：创建指标时只声明了 `node` 标签、调用时却传了 3 个值，
  样本被静默丢弃（图上永远 0）—— 所以**加指标必须同时加/改这里的断言**。
