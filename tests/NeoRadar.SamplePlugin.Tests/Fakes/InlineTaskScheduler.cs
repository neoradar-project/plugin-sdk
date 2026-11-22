using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

// Runs each task on the thread that queues it, so a timed job's work is done when Tick returns.
internal sealed class InlineTaskScheduler : TaskScheduler
{
    protected override void QueueTask(Task task) => TryExecuteTask(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => TryExecuteTask(task);

    protected override IEnumerable<Task> GetScheduledTasks() => [];
}
