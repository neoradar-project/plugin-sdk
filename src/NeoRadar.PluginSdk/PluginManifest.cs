namespace NeoRadar.PluginSdk;

public sealed record PluginManifest(
    string Name,
    string Author,
    string Version,
    int SdkMajorVersion,
    string? Description = null,
    string? Website = null);
