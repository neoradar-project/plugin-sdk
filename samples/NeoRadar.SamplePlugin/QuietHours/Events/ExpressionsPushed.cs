using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.QuietHours.Events;

/// <summary>The quiet-hours demo pushed a session or aircraft expression value.</summary>
internal sealed record ExpressionsPushed(IReadOnlyList<string> Lines);
