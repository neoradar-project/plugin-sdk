using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.RouteInspector.Events;

/// <summary>The route inspector described the selected flight, one line per row.</summary>
internal sealed record RouteInspected(IReadOnlyList<string> Lines);
