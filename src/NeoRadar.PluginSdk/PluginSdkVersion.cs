namespace NeoRadar.PluginSdk;

/// <summary>
/// The SDK contract version. The client compares the version a plugin's SDK reference records with
/// its own copy of this assembly; the comparison is client code, not part of the SDK.
/// </summary>
public static class PluginSdkVersion
{
    /// <summary>
    /// Major version of the SDK contract. Plugins pass it as
    /// <see cref="PluginManifest.SdkMajorVersion"/>; the client refuses any other value.
    /// </summary>
    public const int Major = 0;

    /// <summary>
    /// Minor version: a change that breaks a plugin bumps it and sets <see cref="Patch"/> to 0.
    /// </summary>
    public const int Minor = 10;

    /// <summary>Patch: an addition to the public surface bumps it.</summary>
    public const int Patch = 0;

    /// <summary>Full version string, e.g. "0.6.0". Reported to plugins and in host logs.</summary>
    public static string Current => $"{Major}.{Minor}.{Patch}";
}
