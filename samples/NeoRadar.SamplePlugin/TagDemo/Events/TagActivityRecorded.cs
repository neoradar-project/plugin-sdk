using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.TagDemo.Events;

/// <summary>The tag demo pushed a tag value or heard a tag action or dropdown press.</summary>
internal sealed record TagActivityRecorded(IReadOnlyList<string> Lines);
