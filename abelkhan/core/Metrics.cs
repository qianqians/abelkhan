using System.Collections.Concurrent;
using System.Diagnostics;
using Prometheus;

namespace core;

/// <summary>
/// 业务指标的统一收口。
///
/// 业务代码只依赖这几个方法，换 exporter（prometheus-net → OpenTelemetry metric）时只改这一层。
/// prometheus-net 的指标在**进程内自注册**，`/metrics` 直接读 <see cref="Prometheus.Metrics.DefaultRegistry"/>。
///
/// 命名规范：<c>abelkhan_{子系统}_{单位}</c>；标签只允许用**代码里的常量**（方法名 / operation 名 /
/// channel 名），不要传 userId 这类高基数值。
/// </summary>
public static class Metrics
{
    // 缓存键是「指标名 + 完整标签名集合」：prometheus-net 要求同一个指标名的标签集合固定，
    // 不同调用点传不同标签时是不同实例，用名字单键会创建出标签数不匹配的指标（静默丢样本）。
    private static readonly ConcurrentDictionary<string, Counter> Counters = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, Gauge> Gauges = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, Histogram> Histograms = new(StringComparer.Ordinal);

    private static volatile MonitoringOptions _options = new();
    private static string[] _defaultLabels = Array.Empty<string>();

    /// <summary>
    /// Redis 单次操作通常是亚毫秒级，用默认的 5ms~10s 桶什么都看不到，所以单独给一组细桶。
    /// </summary>
    public static readonly double[] FastBuckets =
        [0.0005, 0.001, 0.0025, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5];

    /// <summary>业务处理延迟的桶（报文级，通常 ms 量级）。</summary>
    public static readonly double[] HandlerBuckets =
        [0.001, 0.0025, 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5, 10];

    public static MonitoringOptions Options => _options;

    /// <summary>初始化：注册全部指标共有的 node 标签 + 运行时指标的采集回调。</summary>
    public static void Init(MonitoringOptions options)
    {
        _options = options ?? new MonitoringOptions();
        if (!_options.Enabled)
        {
            return;
        }

        _defaultLabels = string.IsNullOrEmpty(_options.NodeId) ? [] : ["node"];
        RegisterRuntimeCollectors();
    }

    // ================================================================
    // 写入
    // ================================================================

    /// <summary>计数器 +1（或 +<paramref name="value"/>）。</summary>
    public static void Count(string name, string description, double value = 1,
        params (string Key, string Value)[] labels)
    {
        if (!_options.Enabled)
        {
            return;
        }

        try
        {
            var labelNames = AllLabelNames(labels);
            var counter = Counters.GetOrAdd(CacheKey(name, labelNames),
                _ => Prometheus.Metrics.CreateCounter(name, description, labelNames));
            var values = LabelValues(labels);
            if (values.Length == 0)
            {
                counter.Inc(value);
            }
            else
            {
                counter.WithLabels(values).Inc(value);
            }
        }
        catch (Exception e)
        {
            SafeLog("count", name, e);
        }
    }

    /// <summary>设置一个 Gauge 的当前值。</summary>
    public static void SetGauge(string name, string description, double value,
        params (string Key, string Value)[] labels)
    {
        if (!_options.Enabled)
        {
            return;
        }

        try
        {
            var labelNames = AllLabelNames(labels);
            var gauge = Gauges.GetOrAdd(CacheKey(name, labelNames),
                _ => Prometheus.Metrics.CreateGauge(name, description, labelNames));
            var values = LabelValues(labels);
            if (values.Length == 0)
            {
                gauge.Set(value);
            }
            else
            {
                gauge.WithLabels(values).Set(value);
            }
        }
        catch (Exception e)
        {
            SafeLog("gauge", name, e);
        }
    }

    /// <summary>
    /// 计时一段代码：<c>using var _ = Metrics.Time("abelkhan_x_seconds", "…", ("a","b"));</c>
    /// 返回 null 时表示监控关闭（using 仍然安全）。
    /// </summary>
    public static Prometheus.ITimer? Time(string name, string description, double[]? buckets = null,
        params (string Key, string Value)[] labels)
    {
        if (!_options.Enabled)
        {
            return null;
        }

        try
        {
            var labelNames = AllLabelNames(labels);
            var histogram = Histograms.GetOrAdd(CacheKey(name, labelNames),
                _ => Prometheus.Metrics.CreateHistogram(name, description, new HistogramConfiguration
                {
                    LabelNames = labelNames,
                    Buckets = buckets ?? HandlerBuckets,
                }));
            var values = LabelValues(labels);
            return values.Length == 0 ? histogram.NewTimer() : histogram.WithLabels(values).NewTimer();
        }
        catch (Exception e)
        {
            SafeLog("time", name, e);
            return null;
        }
    }

    /// <summary>Prometheus 抓取时的文本导出（供 <c>/metrics</c> 端点用）。</summary>
    public static Task ExportAsync(Stream stream) =>
        Prometheus.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

    /// <summary>指标自身的错误不能走 Log（Log 里也计数，会递归）。</summary>
    private static void SafeLog(string what, string name, Exception e) =>
        Console.Error.WriteLine($"[metrics] {what} {name} failed: {e.Message}");

    /// <summary>默认标签（node）+ 调用方标签名。</summary>
    private static string[] AllLabelNames((string Key, string Value)[] labels)
    {
        if (labels.Length == 0)
        {
            return _defaultLabels;
        }

        var names = new string[_defaultLabels.Length + labels.Length];
        _defaultLabels.CopyTo(names, 0);
        for (var i = 0; i < labels.Length; i++)
        {
            names[_defaultLabels.Length + i] = labels[i].Key;
        }

        return names;
    }

    private static string CacheKey(string name, string[] labelNames) =>
        labelNames.Length == 0 ? name : name + "|" + string.Join(',', labelNames);

    private static string[] LabelValues((string Key, string Value)[] labels)
    {
        // 默认标签（node）在前，调用方标签在后 —— 顺序必须与 CreateXxx 时的 LabelNames 一致
        if (_defaultLabels.Length == 0)
        {
            return labels.Select(l => l.Value).ToArray();
        }

        var values = new string[_defaultLabels.Length + labels.Length];
        values[0] = _options.NodeId;
        for (var i = 0; i < labels.Length; i++)
        {
            values[i + 1] = labels[i].Value;
        }

        return values;
    }

    // ================================================================
    // 运行时指标（A 层）：抓取时现算，零维护
    // ================================================================

    private static void RegisterRuntimeCollectors()
    {
        var heap = Prometheus.Metrics.CreateGauge("abelkhan_runtime_heap_bytes",
            "GC managed heap size", _defaultLabels);
        var workingSet = Prometheus.Metrics.CreateGauge("abelkhan_runtime_workingset_bytes",
            "Process working set", _defaultLabels);
        var threads = Prometheus.Metrics.CreateGauge("abelkhan_runtime_threadpool_threads",
            "ThreadPool thread count", _defaultLabels);
        var pending = Prometheus.Metrics.CreateGauge("abelkhan_runtime_threadpool_queue",
            "ThreadPool pending work items", _defaultLabels);
        var gcGen0 = Prometheus.Metrics.CreateGauge("abelkhan_runtime_gc_gen0_total",
            "Gen0 collections", _defaultLabels);
        var gcGen2 = Prometheus.Metrics.CreateGauge("abelkhan_runtime_gc_gen2_total",
            "Gen2 collections", _defaultLabels);
        var uptime = Prometheus.Metrics.CreateGauge("abelkhan_runtime_uptime_seconds",
            "Process uptime", _defaultLabels);

        var started = DateTime.UtcNow;

        Prometheus.Metrics.DefaultRegistry.AddBeforeCollectCallback(async ct =>
        {
            try
            {
                var node = _defaultLabels.Length == 0 ? Array.Empty<string>() : [_options.NodeId];
                heap.WithLabels(node).Set(GC.GetTotalMemory(false));
                workingSet.WithLabels(node).Set(Environment.WorkingSet);
                ThreadPool.GetMinThreads(out _, out var minIo);
                ThreadPool.GetAvailableThreads(out var worker, out _);
                threads.WithLabels(node).Set(Math.Max(0, ThreadPool.ThreadCount));
                pending.WithLabels(node).Set(Math.Max(0, ThreadPool.PendingWorkItemCount));
                gcGen0.WithLabels(node).Set(GC.CollectionCount(0));
                gcGen2.WithLabels(node).Set(GC.CollectionCount(2));
                uptime.WithLabels(node).Set((DateTime.UtcNow - started).TotalSeconds);
                _ = worker + minIo;
            }
            catch
            {
                // 采集失败不能影响抓取
            }

            await Task.CompletedTask;
        });
    }
}
