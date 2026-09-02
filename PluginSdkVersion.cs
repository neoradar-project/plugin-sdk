namespace NeoRadar.PluginSdk;

public static class PluginSdkVersion
{
    /// <summary>
    /// Major version of the SDK contract. Plugins declaring a different major
    /// in their <see cref="PluginManifest.SdkMajorVersion"/> are refused.
    /// </summary>
    public const int Major = 0;

    /// <summary>
    /// Minor version. While <see cref="Major"/> is 0 the public surface may still break
    /// between minors; a break bumps this and nothing else.
    /// </summary>
    public const int Minor = 2;

    /// <summary>Patch: non-breaking fixes.</summary>
    public const int Patch = 0;

    /// <summary>Full version string, e.g. "0.2.0". Reported to plugins and in host logs.</summary>
    public static string Current => $"{Major}.{Minor}.{Patch}";

    /// <summary>
    /// True when a plugin declaring <paramref name="pluginSdkMajor"/> can bind against this SDK.
    /// </summary>
    public static bool IsMajorCompatible(int pluginSdkMajor) => pluginSdkMajor == Major;

    /// <summary>
    /// Message for a refused plugin, naming both majors and the fix.
    /// </summary>
    public static string DescribeMajorMismatch(string pluginName, int pluginSdkMajor) =>
        $"Plugin '{pluginName}' declares SdkMajorVersion {pluginSdkMajor} but this host ships "
        + $"NeoRadar.PluginSdk {Current} (major {Major}). Rebuild the plugin against SDK {Current} "
        + "and set Manifest.SdkMajorVersion = PluginSdkVersion.Major.";

    /// <summary>
    /// Message for a plugin that loaded but hit a missing SDK member, i.e. it was compiled against
    /// a different minor of the same major.
    /// </summary>
    public static string DescribeBindingFailure(string pluginName, string detail) =>
        $"Plugin '{pluginName}' was built against a different NeoRadar.PluginSdk than this host "
        + $"({Current}) and references a member that no longer exists: {detail}. "
        + $"Rebuild it against SDK {Current}. Breaking renames in 0.x land on a minor bump.";
}
