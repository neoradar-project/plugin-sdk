namespace NeoRadar.PluginSdk;

/// <summary>
/// Version constants a plugin inlines into its <see cref="PluginManifest"/>. The client compares
/// them with its own copy of this assembly; the comparison is client code, not part of the SDK.
/// </summary>
public static partial class PluginSdkVersion
{
    /// <summary>
    /// Major version of the SDK contract. Plugins pass it as
    /// <see cref="PluginManifest.SdkMajorVersion"/>; the client refuses any other value.
    /// </summary>
    public const int Major = 0;

    /// <summary>
    /// Minor version. While <see cref="Major"/> is 0 the public surface may still break
    /// between minors; a break bumps this and nothing else.
    /// </summary>
    public const int Minor = 5;

    /// <summary>Patch: non-breaking fixes.</summary>
    public const int Patch = 0;

    /// <summary>Full version string, e.g. "0.5.0". Reported to plugins and in host logs.</summary>
    public static string Current => $"{Major}.{Minor}.{Patch}";
}
