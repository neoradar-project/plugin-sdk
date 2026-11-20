namespace NeoRadar.PluginSdk;

/// <summary>
/// Represents the identity a plugin declares to the host.
/// </summary>
/// <param name="Name">The plugin's display name.</param>
/// <param name="Author">The plugin's author.</param>
/// <param name="Version">The plugin's own version.</param>
/// <param name="SdkMajorVersion">The SDK major version the plugin targets; pass <see cref="PluginSdkVersion.Major"/>.</param>
/// <param name="Description">An optional short description.</param>
/// <param name="Website">An optional website address.</param>
public sealed record PluginManifest(
    string Name,
    string Author,
    string Version,
    int SdkMajorVersion,
    string? Description = null,
    string? Website = null);
