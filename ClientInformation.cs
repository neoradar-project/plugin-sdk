namespace NeoRadar.PluginSdk;

/// <summary>
/// Mirrors <c>PluginSDK::ClientInformation</c>. Passed to plugins on Initialize
/// so they can show host version info / locate their own user data folder.
/// </summary>
public sealed record ClientInformation(
    string ClientName,
    string ClientVersion,
    string FdpsVersion,
    string CombinedVersion,
    string DocumentsPath);
