using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.Commands.Events;

/// <summary>The commands demo registered its chat commands, or one of them ran.</summary>
internal sealed record CommandsListed(IReadOnlyList<string> Lines);
