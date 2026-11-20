namespace NeoRadar.PluginSdk.Package;

/// <summary>
/// Provides access to the active package.
/// </summary>
public interface IPackageApi
{
    /// <summary>
    /// Gets the filesystem path of the active package, or null when none is loaded.
    /// </summary>
    string? GetPackagePath();
}
