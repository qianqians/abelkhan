namespace core;

/// <summary>
/// 监控配置。每个服务（hub / gate / admin）的配置文件里都可以带一段 <c>Monitoring</c>。
///
/// 两条导出路径，互相独立：
///   - **拉**：<see cref="PrometheusPort"/> &gt; 0 时，在 <see cref="PrometheusHost"/> 上开一个
///     只服务 <c>/metrics</c>+<c>/health</c> 的监听（默认 127.0.0.1，内网）；Prometheus 来抓。
///   - **推**：<see cref="OtlpEndpoint"/> 非空时，用 OpenTelemetry 把 trace / 运行时指标
///     推到 OTel Collector（OtlpExporter）。
///
/// 默认值刻意保守：metrics 只在回环地址开，OTLP 关闭 —— 不改配置就什么外网端口都不会多开。
/// </summary>
public sealed class MonitoringOptions
{
    /// <summary>总开关：false 时 <see cref="Metrics"/> / <see cref="Tracing"/> 全部退化成空实现。</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>服务名，作为 OTel resource 的 <c>service.name</c>，也作为 Prometheus 的 job 参考。</summary>
    public string ServiceName { get; set; } = "abelkhan";

    /// <summary>
    /// 节点 id（hubId / gateId / adminId）。会作为指标标签 <c>node</c>，
    /// 一组副本在同一个 Prometheus 里也能区分开。
    /// </summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>独立 metrics 监听端口。0 = 不开（<c>/metrics</c> 不可用）。</summary>
    public int PrometheusPort { get; set; }

    /// <summary>
    /// metrics 监听地址。默认回环 —— 内网/同机采集就够，不要配 0.0.0.0。
    /// 容器里 Prometheus 在别的容器时，改成 0.0.0.0 并靠网络策略隔离。
    /// </summary>
    public string PrometheusHost { get; set; } = "127.0.0.1";

    /// <summary>OTLP 端点（如 <c>http://127.0.0.1:4317</c>）。空 = 不导出 trace / 运行时指标。</summary>
    public string OtlpEndpoint { get; set; } = string.Empty;

    /// <summary>trace 采样比例（0~1，默认全采；量大时可以调低）。</summary>
    public double TraceSampleRatio { get; set; } = 1.0;

    /// <summary>慢调用日志阈值（毫秒）：超过就打一条 warn。0 = 不打。</summary>
    public int SlowLogThresholdMs { get; set; } = 500;

    public bool PrometheusEnabled => Enabled && PrometheusPort > 0;

    public bool OtlpEnabled => Enabled && !string.IsNullOrWhiteSpace(OtlpEndpoint);
}
