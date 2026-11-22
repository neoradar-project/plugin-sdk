using System;
using NeoRadar.SamplePlugin.Alerts.Events;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>The alerts demo's recent activity, each line timed and published as it is recorded.</summary>
internal sealed class AlertActivity
{
    private readonly IPluginEventBus _bus;
    private readonly RecentLines _lines = new(8);

    public AlertActivity(IPluginEventBus bus) => _bus = bus;

    public void Record(string line)
    {
        _lines.Add($"{DateTimeOffset.UtcNow:HH:mm:ss} {line}");
        _bus.Publish(new AlertActivityRecorded(_lines.Snapshot()));
    }
}
