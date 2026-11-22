using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

// Holds the thread that queues a task until the test releases it, then runs the task on the pool.
internal sealed class PausingTaskScheduler : TaskScheduler, IDisposable
{
    public ManualResetEventSlim Queued { get; } = new();

    public ManualResetEventSlim Released { get; } = new();

    public void Dispose()
    {
        Queued.Dispose();
        Released.Dispose();
    }

    protected override void QueueTask(Task task)
    {
        Queued.Set();
        Released.Wait(TimeSpan.FromSeconds(10));
        ThreadPool.UnsafeQueueUserWorkItem(_ => TryExecuteTask(task), null);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

    protected override IEnumerable<Task> GetScheduledTasks() => [];
}
