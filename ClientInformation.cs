namespace NeoRadar.PluginSdk;

/// <summary>
/// Passed to plugins on Initialize so they can show host version info and
/// locate their own user data folder.
/// </summary>
public sealed record ClientInformation(
    string ClientName,
    string Version,
    string DocumentsPath);
