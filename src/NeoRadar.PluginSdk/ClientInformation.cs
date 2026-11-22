namespace NeoRadar.PluginSdk;

/// <summary>
/// Represents the host details passed to a plugin on initialisation.
/// </summary>
/// <param name="ClientName">The host application's name.</param>
/// <param name="Version">The host application's version.</param>
/// <param name="DocumentsPath">The host's data root, Documents/NeoRadar on Windows and macOS and $XDG_DATA_HOME/NeoRadar on Linux.</param>
public sealed record ClientInformation(
    string ClientName,
    string Version,
    string DocumentsPath);
