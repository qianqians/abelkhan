using System.Collections.Concurrent;
using core;

namespace hub;

interface IJob
{
    Task RunAsync();
}

class SyncJob(Action action) : IJob
{
    public Task RunAsync()
    {
        action();
        return Task.CompletedTask;
    }
}

class AsyncJob(Func<Task> func) : IJob
{
    public Task RunAsync() => func();
}

/// <summary>
/// 每个实体一条**串行**任务队列，自调度：
///
///   - <see cref="PostTask"/> 入队后，若当前没有 drain 任务就起一个（线程池上跑）；
///   - drain 任务把队列跑空后**自动退出**，空闲实体不占任何线程 / Task。
///
/// claim 协议（保证「同一时刻至多一个 drain」且「队列非空必有人 drain」）：
/// <code>
///   Post:     Enqueue → CAS(0→1)，赢了才起 PumpAsync
///   PumpAsync:跑空 → Exchange(0) 释放 → 若此刻队列又非空：
///             CAS(0→1) 赢 → 继续跑；输 → 说明另一个 Post 已接管，自己退出
/// </code>
/// 这样「Post 入队」与「释放 claim」之间的任何交错都恰好有一个 drain 接管，不会漏跑也不会并发跑。
///
/// 单个 job 抛异常只记日志、不中断队列（否则一个坏 handler 会让该实体永久停摆）。
/// </summary>
public class Actor
{
    private readonly ConcurrentQueue<IJob> _jobs = new();

    /// <summary>0 = 没有 drain 任务在跑；1 = 有一个。用 Interlocked 当锁。</summary>
    private int _draining;

    public void PostTask(Action action) => Post(new SyncJob(action));

    public void PostTask(Func<Task> func) => Post(new AsyncJob(func));

    /// <summary>队列里当前积压的 job 数（监控 / 诊断用）。</summary>
    public int PendingCount => _jobs.Count;

    private void Post(IJob job)
    {
        _jobs.Enqueue(job);
        EnsureDrain();
    }

    private void EnsureDrain()
    {
        if (Interlocked.CompareExchange(ref _draining, 1, 0) == 0)
        {
            // 不 await：Post 的调用方不该被 drain 阻塞
            _ = PumpAsync();
        }
    }

    /// <summary>进入时已持有 claim；退出时保证 claim 已释放或已移交。</summary>
    private async Task PumpAsync()
    {
        try
        {
            while (true)
            {
                while (_jobs.TryDequeue(out var job))
                {
                    try
                    {
                        await job.RunAsync();
                    }
                    catch (Exception e)
                    {
                        Log.Error($"actor job failed: {e}");
                    }
                }

                // 队列空 → 释放 claim
                Interlocked.Exchange(ref _draining, 0);

                // 释放后复查：若在「判空」与「释放」之间有 Post 进来，重新抢 claim 继续跑；
                // 抢不到说明那个 Post 自己已经起了 drain，本任务退出
                if (_jobs.IsEmpty)
                {
                    return;
                }

                if (Interlocked.CompareExchange(ref _draining, 1, 0) != 0)
                {
                    return;
                }
            }
        }
        catch (Exception e)
        {
            // 理论上到不了（job 异常已在内层捕获）；兜底释放 claim 并重启，防止队列永久停摆
            Log.Error($"actor pump failed: {e}");
            Interlocked.Exchange(ref _draining, 0);
            if (!_jobs.IsEmpty)
            {
                EnsureDrain();
            }
        }
    }
}
