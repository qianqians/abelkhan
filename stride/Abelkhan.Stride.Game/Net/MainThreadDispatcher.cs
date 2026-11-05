using System.Collections.Concurrent;

namespace Abelkhan.Stride;

/// <summary>
/// 主线程调度器：Stride 没有「一行回主线程」的 API，所以自己维护一个队列，
/// 在 <c>Game.Update</c>（也就是 <see cref="Net.AbelkhanClientSystem.Update"/>）里 drain。
///
/// 同时作为 <see cref="SynchronizationContext"/> 装在主线程上：
/// 这样 <c>await</c> 之后的续体也会回到主线程，handler 里可以直接接触游戏对象。
/// </summary>
public sealed class MainThreadDispatcher : SynchronizationContext
{
    private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _queue = new();
    private readonly int _mainThreadId;

    public MainThreadDispatcher()
    {
        _mainThreadId = Environment.CurrentManagedThreadId;
    }

    public bool IsMainThread => Environment.CurrentManagedThreadId == _mainThreadId;

    /// <summary>排一个回调到主线程</summary>
    public void Post(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _queue.Enqueue((_ => action(), null));
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        ArgumentNullException.ThrowIfNull(d);
        _queue.Enqueue((d, state));
    }

    /// <summary>同步执行（已经主线程就直接跑）</summary>
    public override void Send(SendOrPostCallback d, object? state)
    {
        if (IsMainThread)
        {
            d(state);
            return;
        }

        using var done = new ManualResetEventSlim(false);
        Exception? error = null;
        Post(_ =>
        {
            try
            {
                d(state);
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                done.Set();
            }
        }, null);

        done.Wait();
        if (error != null)
        {
            throw error;
        }
    }

    /// <summary>
    /// 在主线程上把队列跑一轮。每帧调用一次，<paramref name="max"/> 防止一帧被刷爆。
    /// 返回本轮实际执行的回调数。
    /// </summary>
    public int Drain(int max = 512)
    {
        if (!IsMainThread)
        {
            throw new InvalidOperationException("Drain 必须在主线程上调用");
        }

        var count = 0;
        while (count < max && _queue.TryDequeue(out var item))
        {
            try
            {
                item.Callback(item.State);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[abelkhan] main-thread callback error: {e}");
            }

            count++;
        }

        return count;
    }
}
