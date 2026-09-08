namespace NeoRadar.PluginSdk.Package;

public interface IPackageApi
{
    /// <summary>Filesystem path of the active package, or null if none loaded.</summary>
    string? GetPackagePath();
}
