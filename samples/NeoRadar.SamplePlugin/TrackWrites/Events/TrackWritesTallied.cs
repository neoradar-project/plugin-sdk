using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.TrackWrites.Events;

/// <summary>The track-writes demo tried a controller-data write; the lines tally what was sent and refused.</summary>
internal sealed record TrackWritesTallied(IReadOnlyList<string> Lines);
