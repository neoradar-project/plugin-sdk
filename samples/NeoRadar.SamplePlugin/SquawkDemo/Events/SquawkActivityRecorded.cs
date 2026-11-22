using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.SquawkDemo.Events;

/// <summary>The squawk demo registered, unregistered, listed or activated its provider, or heard an assignment.</summary>
internal sealed record SquawkActivityRecorded(IReadOnlyList<string> Lines);
