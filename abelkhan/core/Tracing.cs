using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace core;

/// <summary>
/// OpenTelemetry 导出（推模式）：trace + 运行时指标走 OTLP 到 Collector。
///
/// 与 <see cref="Metrics"/>（prometheus-net，拉模式）是互补的两条路：
///   - 拉：业务指标 → <c>/metrics</c> → Prometheus（不依赖 Collector，始终可用）
///   - 推：trace + 运行时指标 → OTLP → Collector → Tempo / Prometheus remote-write
///
/// <b>trace 目前是「进程内」的</b>：一个 hub 的报文处理 + 它引发的 Redis 操作会在同一条 trace 里。
/// 跨进程（gate → hub）的链路需要把 trace_id 放进 <c>CallRpc</c>（要改 common.proto），
/// 属于后续工作，见 docs/monitoring.md。
/// </summary>
public static class Tracing
{
    /// <summary>所有业务 span 的 ActivitySource 名。测试里也用它断言。</summary>
    public const string SourceName = "abelkhan";

    public static ActivitySource Source { get; } = new(SourceName, "1.0.0");

    private static TracerProvider? _tracerProvider;
    private static MeterProvider? _meterProvider;

    public static bool Initialized => _tracerProvider != null;

    /// <summary>按配置初始化；<see cref="MonitoringOptions.OtlpEnabled"/> 为 false 时什么都不做。</summary>
    public static void Init(MonitoringOptions options)
    {
        if (!options.OtlpEnabled || _tracerProvider != null)
        {
            return;
        }

        try
        {
            var resource = ResourceBuilder.CreateDefault()
                .AddService(options.ServiceName, serviceVersion: "1.0.0",
                    serviceInstanceId: string.IsNullOrEmpty(options.NodeId) ? null : options.NodeId);

            _tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resource)
                .SetSampler(new TraceIdRatioBasedSampler(Math.Clamp(options.TraceSampleRatio, 0, 1)))
                .AddSource(SourceName)
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(options.OtlpEndpoint);
                    o.Protocol = OtlpExportProtocol.Grpc;
                })
                .Build();

            // 运行时指标（GC / 线程池 / CPU）走 OTel metrics → OTLP；
            // Prometheus 那边也有一套自算的版本，两边不冲突（名字不同）。
            _meterProvider = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(resource)
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(options.OtlpEndpoint);
                    o.Protocol = OtlpExportProtocol.Grpc;
                })
                .Build();

            Log.Info("tracing: OTLP exporter enabled → {0} (service={1})", options.OtlpEndpoint, options.ServiceName);
        }
        catch (Exception e)
        {
            Log.Error("tracing: init failed: {0}", e.Message);
        }
    }

    /// <summary>开一个 span；监控关闭 / 没有监听者时返回 null（using 仍然安全）。</summary>
    public static Activity? Start(string name, ActivityKind kind = ActivityKind.Internal,
        params (string Key, string Value)[] tags)
    {
        var activity = Source.StartActivity(name, kind);
        if (activity == null)
        {
            return null;
        }

        foreach (var (key, value) in tags)
        {
            activity.SetTag(key, value);
        }

        return activity;
    }

    /// <summary>记录一次失败，并在 span 上打 error 状态。</summary>
    public static void MarkError(Activity? activity, Exception? error = null, string? message = null)
    {
        if (activity == null)
        {
            return;
        }

        activity.SetStatus(ActivityStatusCode.Error, message ?? error?.Message);
        if (error != null)
        {
            activity.SetTag("error.type", error.GetType().FullName);
        }
    }

    /// <summary>退出时 flush（丢弃会丢最后一批 span）。</summary>
    public static void Shutdown()
    {
        try
        {
            _tracerProvider?.Shutdown();
            _meterProvider?.Shutdown();
        }
        catch
        {
            // 退出路径上不抛
        }
        finally
        {
            _tracerProvider?.Dispose();
            _meterProvider?.Dispose();
            _tracerProvider = null;
            _meterProvider = null;
        }
    }
}
