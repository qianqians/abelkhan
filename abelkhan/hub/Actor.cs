using System.Collections.Concurrent;

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
    
public class Actor
{
    private readonly ConcurrentQueue<IJob> _jobs = new();

    public void PostTask(Action action)
    {
        _jobs.Enqueue(new SyncJob(action));
    }
    
    public void PostTask(Func<Task> func)
    {
        _jobs.Enqueue(new AsyncJob(func));
    }
    
    public async Task Run()
    {
        while (_jobs.TryDequeue(out var job))
        {
            await job.RunAsync();
        }
    }
}