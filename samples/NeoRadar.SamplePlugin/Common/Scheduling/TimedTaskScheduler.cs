using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NeoRadar.PluginSdk.Logger;

namespace NeoRadar.SamplePlugin.Common.Scheduling;

/// <summary>Starts each due job on the thread pool when the host tick calls <see cref="Tick"/>.</summary>
/// <remarks>A job never overlaps itself: it is due again a period after its work returns.</remarks>
internal sealed class TimedTaskScheduler : ITimedTaskScheduler
{
    private static readonly TimeSpan StopTimeout = TimeSpan.FromSeconds(2);

    private readonly ILoggerApi _logger;
    private readonly TimeProvider _time;
    private readonly TaskScheduler _pool;

    // Guards the jobs, the running tasks and the stopped flag: Tick runs on the timer thread while finished work reschedules from the pool.
    private readonly object _sync = new();
    private readonly List<Job> _jobs = new();
    private readonly List<Task> _running = new();
    private bool _stopped;

    /// <param name="pool">Where job work runs; the composition passes the thread pool's default scheduler.</param>
    public TimedTaskScheduler(ILoggerApi logger, TimeProvider time, TaskScheduler pool)
    {
        _logger = logger;
        _time = time;
        _pool = pool;
    }

    public IDisposable Every(string name, TimeSpan period, Action work) => Add(new Job(this, name, period, work, once: false));

    public void RunOnce(string name, Action work) => Add(new Job(this, name, TimeSpan.Zero, work, once: true));

    /// <summary>Called by the host tick, which is never re-entered, so never concurrently with itself.</summary>
    public void Tick()
    {
        DateTimeOffset now = _time.GetUtcNow();

        // Jobs start under the lock, so an overlapping Stop either waits for each one or comes first and none starts.
        lock (_sync)
        {
            if (_stopped) return;
            _running.RemoveAll(static task => task.IsCompleted);
            foreach (Job job in TakeDueJobsLocked(now)) _running.Add(Start(job));
        }
    }

    /// <summary>Stops taking work and waits a bounded time for jobs in flight, warning if any is still running. Never throws.</summary>
    public void Stop()
    {
        Task[] running;
        lock (_sync)
        {
            if (_stopped) return;
            _stopped = true;
            _jobs.Clear();
            running = _running.ToArray();
        }

        try
        {
            if (!Task.WaitAll(running, StopTimeout))
                _logger.Warning($"Timed jobs still running {StopTimeout.TotalSeconds:0}s after shutdown began");
        }
        catch (Exception ex)
        {
            _logger.Error($"[scheduler Stop] {ex}");
        }
    }

    private Job Add(Job job)
    {
        lock (_sync)
        {
            if (!_stopped) _jobs.Add(job);
        }
        return job;
    }

    private void Remove(Job job)
    {
        lock (_sync) _jobs.Remove(job);
    }

    // Caller holds _sync.
    private List<Job> TakeDueJobsLocked(DateTimeOffset now)
    {
        List<Job> due = new();
        for (int i = _jobs.Count - 1; i >= 0; i--)
        {
            Job job = _jobs[i];
            if (job.Running || job.NextDue > now) continue;
            job.Running = true;
            if (job.Once) _jobs.RemoveAt(i);
            due.Add(job);
        }
        return due;
    }

    private Task Start(Job job)
        => Task.Factory.StartNew(() => Run(job), CancellationToken.None, TaskCreationOptions.DenyChildAttach, _pool);

    private void Run(Job job)
    {
        try
        {
            job.Work();
        }
        catch (Exception ex)
        {
            _logger.Error($"[scheduler {job.Name}] {ex}");
        }
        finally
        {
            lock (_sync)
            {
                job.NextDue = _time.GetUtcNow() + job.Period;
                job.Running = false;
            }
        }
    }

    private sealed class Job : IDisposable
    {
        private readonly TimedTaskScheduler _owner;

        public Job(TimedTaskScheduler owner, string name, TimeSpan period, Action work, bool once)
        {
            _owner = owner;
            Name = name;
            Period = period;
            Work = work;
            Once = once;
        }

        public string Name { get; }

        public TimeSpan Period { get; }

        public Action Work { get; }

        public bool Once { get; }

        public DateTimeOffset NextDue { get; set; }

        public bool Running { get; set; }

        public void Dispose() => _owner.Remove(this);
    }
}
