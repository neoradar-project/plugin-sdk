namespace NeoRadar.PluginSdk;

/// <summary>
/// Passed to plugins on Initialize so they can show host version info and locate their own user
/// data folder. <c>DataPath</c> is the host's data root: Documents/NeoRadar on Windows and macOS,
/// $XDG_DATA_HOME/NeoRadar (default ~/.local/share/NeoRadar) on Linux.
/// </summary>
public sealed record ClientInformation(
    string ClientName,
    string Version,
    string DataPath);
