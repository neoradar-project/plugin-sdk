namespace NeoRadar.PluginSdk;

public static class PluginSdkVersion
{
    /// <summary>
    /// Major version of the SDK contract. Plugins declaring a different major
    /// in their <see cref="PluginManifest.SdkMajorVersion"/> are refused.
    /// </summary>
    public const int Major = 0;

    /// <summary>Minor version — additive changes within the same major.</summary>
    public const int Minor = 1;

    /// <summary>Patch — non-breaking fixes.</summary>
    public const int Patch = 0;
}
