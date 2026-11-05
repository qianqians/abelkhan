namespace core;

/// <summary>
/// 服务健康状态：让 <c>/health</c> 变成真的（而不是恒返回 healthy）。
///
/// 现在 consul 的健康检查是拿 <c>/health</c> 的 200 判定实例可用，但那个端点从来不检查任何东西 ——
/// Redis 挂了、主循环卡死，实例照样「健康」，流量继续往上打。这里补上两件事：
///   1. 主循环每轮调 <see cref="OnLoop"/>，健康检查看它多久没动（卡死会被摘掉）；
///   2. 各服务注册自己的依赖探针（hub/gate → Redis PING；admin → Redis + Mongo）。
/// </summary>
public static class ServiceHealth
{
    private static long _lastLoopTick;
    private static volatile bool _loopStarted;
    private static Func<CancellationToken, Task<bool>>? _dependencies;

    /// <summary>
    /// 主循环每轮调一次（O(1)，就一个 Interlocked 写）。启动时也应先调一次。
    ///
    /// 为什么要 <see cref="_loopStarted"/>：这个类的静态字段是**首次访问时**才初始化的，
    /// 如果第一次访问发生在第一个 /health 请求里，时间戳就是「请求时刻」，
    /// 于是卡死的进程也会显示健康。所以没跑过主循环一律判不健康。
    /// </summary>
    public static void OnLoop()
    {
        Interlocked.Exchange(ref _lastLoopTick, Environment.TickCount64);
        _loopStarted = true;
    }

    /// <summary>注册依赖探针：返回 true 表示依赖都通。</summary>
    public static void RegisterDependencies(Func<CancellationToken, Task<bool>> check) => _dependencies = check;

    /// <summary>
    /// 判定健康。<paramref name="maxIdleMs"/> = 主循环允许的最长空闲（默认 10s，
    /// 比 consul 的 <c>DeregisterCriticalServiceAfter=5s</c> 宽松一点，避免 GC 抖动误摘）。
    /// </summary>
    public static async Task<(bool Ok, string Detail)> CheckAsync(int maxIdleMs = 10_000)
    {
        if (!_loopStarted)
        {
            return (false, "main loop has not started yet");
        }

        var idle = Environment.TickCount64 - Interlocked.Read(ref _lastLoopTick);
        if (idle > maxIdleMs)
        {
            return (false, $"main loop stalled for {idle}ms");
        }

        var check = _dependencies;
        if (check == null)
        {
            return (true, "ok");
        }

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var ok = await check(cts.Token);
            return ok ? (true, "ok") : (false, "dependency check failed");
        }
        catch (Exception e)
        {
            return (false, $"dependency check error: {e.Message}");
        }
    }
}
