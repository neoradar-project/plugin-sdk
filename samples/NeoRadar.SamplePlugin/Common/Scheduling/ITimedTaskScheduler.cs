using System;

namespace NeoRadar.SamplePlugin.Common.Scheduling;

/// <summary>Every periodic or deferred job, driven by the host timer, so no feature starts a thread of its own.</summary>
/// <remarks>Work runs on the thread pool, never on the timer thread; dispose the handle to stop a job.</remarks>
internal interface ITimedTaskScheduler
{
    IDisposable Every(string name, TimeSpan period, Action work);

    void RunOnce(string name, Action work);
}
