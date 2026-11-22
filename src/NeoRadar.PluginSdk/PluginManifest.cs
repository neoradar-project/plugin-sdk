namespace NeoRadar.PluginSdk;

/// <summary>
/// Represents the identity a plugin declares to the host.
/// </summary>
/// <remarks>
/// The host refuses to load a plugin whose name holds anything but letters, digits, spaces, '-', '_' and '.', or matches
/// another loaded plugin's name in any letter case. It also refuses a version that is empty or holds anything but letters,
/// digits, spaces, '.', '-' and '+', because the client sends the version to the network when asked which plugins it runs.
/// </remarks>
/// <param name="Name">The plugin's display name and identity.</param>
/// <param name="Author">The plugin's author.</param>
/// <param name="Version">The plugin's own version.</param>
/// <param name="SdkMajorVersion">The SDK major version the plugin targets, which is <see cref="PluginSdkVersion.Major"/>.</param>
/// <param name="Description">An optional short description.</param>
/// <param name="Website">An optional website address, shown in Plugin Settings as a link that opens the system browser.</param>
public sealed record PluginManifest(
    string Name,
    string Author,
    string Version,
    int SdkMajorVersion,
    string? Description = null,
    string? Website = null);
