using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Scheduling;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeTimedTaskScheduler : ITimedTaskScheduler
{
    public List<Job> Jobs { get; } = new();

    public IDisposable Every(string name, TimeSpan period, Action work)
    {
        Job job = new(name, work);
        Jobs.Add(job);
        return job;
    }

    public void RunOnce(string name, Action work) => Jobs.Add(new Job(name, work));

    internal sealed class Job(string name, Action work) : IDisposable
    {
        public string Name { get; } = name;

        public Action Work { get; } = work;

        public bool Disposed { get; private set; }

        public void Dispose() => Disposed = true;
    }
}
