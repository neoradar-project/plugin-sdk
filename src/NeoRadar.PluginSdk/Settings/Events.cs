namespace NeoRadar.PluginSdk.Settings;

/// <summary>
/// The client loaded a different profile. Fires after the change, so a plugin re-reads its
/// profile-scoped values from the handler. <c>ProfileName</c> is null when the profile was closed
/// and none is active.
/// </summary>
public sealed record ActiveProfileChangedEvent(string? ProfileName);
