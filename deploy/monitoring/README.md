# 监控栈（Prometheus + Grafana + OTel Collector）

配套代码：`abelkhan/core/Metrics.cs`（prometheus-net，拉）、`abelkhan/core/Tracing.cs`（OTel，推）、
`abelkhan/core/MonitoringHost.cs`（独立的 `/metrics` 监听）、`abelkhan/core/ServiceHealth.cs`（真健康检查）。
设计与取舍见 `docs/monitoring.md`。

## 起

```bash
cd deploy/monitoring
docker compose up -d
# Grafana  → http://localhost:3000  (admin/admin，dashboard 在 Abelkhan 目录下)
# Prometheus → http://localhost:9090
```

## 让服务被采集到

每个服务在自己配置里加一段（hub / gate / admin 都支持）：

```json
"Monitoring": {
  "Enabled": true,
  "ServiceName": "abelkhan-hub",
  "NodeId": "hub-1",
  "PrometheusPort": 9101,
  "PrometheusHost": "0.0.0.0",
  "OtlpEndpoint": "http://127.0.0.1:4317"
}
```

- `PrometheusPort`：**默认 0 = 关闭**，`/metrics` 不开。开了之后是独立端口，
  不和现有健康检查端口共用（那个端口常常绑 0.0.0.0，挂 `/metrics` 会暴露到公网）。
- `PrometheusHost`：默认 `127.0.0.1`（内网/同机采集）。Prometheus 跑在别的容器/机器时改 `0.0.0.0`，
  然后用网络策略限制来源。
- `OtlpEndpoint`：空 = 不导出 trace / 运行时指标。填上就推给 `otel-collector`。
- `NodeId`：作为所有指标的 `node` 标签，多副本时用来区分。

采集目标写在 `prometheus/prometheus.yml`，按实际端口改 `targets`。

## 自检

```bash
curl -s 127.0.0.1:9101/metrics | grep abelkhan_ | head
curl -s 127.0.0.1:9101/health    # 服务卡死/Redis 不通时返回 503
```
