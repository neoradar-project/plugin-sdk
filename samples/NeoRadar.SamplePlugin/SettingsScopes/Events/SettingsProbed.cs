using System.Collections.Generic;

namespace NeoRadar.SamplePlugin.SettingsScopes.Events;

/// <summary>The settings demo read or wrote a setting in one of its two scopes.</summary>
internal sealed record SettingsProbed(IReadOnlyList<string> Lines);
