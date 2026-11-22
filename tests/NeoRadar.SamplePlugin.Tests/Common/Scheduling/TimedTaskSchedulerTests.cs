using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoRadar.SamplePlugin.Common.Scheduling;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Common.Scheduling;

public sealed class TimedTaskSchedulerTests
{
    [Fact]
    public void AJob_RunsOffTheTickingThread_AndNotAgainBeforeItsPeriod()
    {
        TimedTaskScheduler scheduler = new(new FakeLoggerApi(), new FakeTimeProvider(), TaskScheduler.Default);
        int runs = 0;
        int tickThread = Environment.CurrentManagedThreadId;
        int workThread = tickThread;
        using ManualResetEventSlim ran = new();
        scheduler.Every("probe", TimeSpan.FromSeconds(5), () =>
        {
            workThread = Environment.CurrentManagedThreadId;
            Interlocked.Increment(ref runs);
            ran.Set();
        });

        scheduler.Tick();
        Assert.True(ran.Wait(TimeSpan.FromSeconds(5)));
        scheduler.Tick();
        scheduler.Stop();

        Assert.Equal(1, runs);
        Assert.NotEqual(tickThread, workThread);
    }

    [Fact]
    public void AJobThatThrows_IsLogged_AndStopStillReturns()
    {
        FakeLoggerApi logger = new();
        TimedTaskScheduler scheduler = new(logger, new FakeTimeProvider(), TaskScheduler.Default);
        scheduler.RunOnce("boom", () => throw new InvalidOperationException("boom"));

        scheduler.Tick();
        scheduler.Stop();

        Assert.Contains(logger.Lines, line => line.Message.Contains("scheduler boom"));
    }

    [Fact]
    public void AJobATickHasTaken_NeverRunsAfterAnOverlappingStop_HasReturned()
    {
        using PausingTaskScheduler pool = new();
        using ManualResetEventSlim jobRan = new();
        TimedTaskScheduler scheduler = new(new FakeLoggerApi(), new FakeTimeProvider(), pool);
        ConcurrentQueue<string> order = new();
        scheduler.RunOnce("probe", () =>
        {
            order.Enqueue("job ran");
            jobRan.Set();
        });

        Thread tick = new(scheduler.Tick);
        tick.Start();
        Assert.True(pool.Queued.Wait(TimeSpan.FromSeconds(5)));
        Thread stop = new(() =>
        {
            scheduler.Stop();
            order.Enqueue("stop returned");
        });
        stop.Start();
        stop.Join(TimeSpan.FromMilliseconds(300));
        pool.Released.Set();
        tick.Join();
        stop.Join();
        jobRan.Wait(TimeSpan.FromSeconds(2));

        Assert.Equal("stop returned", order.Last());
    }
}
