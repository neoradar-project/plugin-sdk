namespace NeoRadar.PluginSdk.Settings;

/// <summary>
/// Raised after the client loads a different profile, so a plugin can re-read its profile-scoped values.
/// </summary>
/// <param name="ProfileName">The new profile's name, or null when the profile was closed and none is active.</param>
public sealed record ActiveProfileChangedEvent(string? ProfileName);
