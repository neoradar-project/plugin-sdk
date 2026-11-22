using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.RunwayBoard.Events;

/// <summary>The runway board described the active airports, their runways and its last write.</summary>
internal sealed record RunwayBoardRefreshed(IReadOnlyList<string> Lines);
