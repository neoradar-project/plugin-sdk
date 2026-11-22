namespace NeoRadar.PluginSdk;

/// <summary>
/// Provides the SDK contract version the client checks a plugin's SDK reference against.
/// </summary>
public static class PluginSdkVersion
{
    /// <summary>
    /// The major version of the SDK contract, which a plugin's manifest must name.
    /// </summary>
    public const int Major = 0;

    /// <summary>
    /// The minor version, bumped by a change that breaks a plugin, which resets <see cref="Patch"/> to 0.
    /// </summary>
    public const int Minor = 1;

    /// <summary>
    /// The patch version, bumped by an addition to the public surface.
    /// </summary>
    public const int Patch = 0;

    /// <summary>
    /// Gets the full version string, for example "0.1.0".
    /// </summary>
    public static string Current => $"{Major}.{Minor}.{Patch}";
}
