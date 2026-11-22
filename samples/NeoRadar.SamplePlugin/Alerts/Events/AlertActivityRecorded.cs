using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Alerts.Events;

/// <summary>The alerts demo showed a toast, played a sound or moved the selection.</summary>
internal sealed record AlertActivityRecorded(IReadOnlyList<string> Lines);
