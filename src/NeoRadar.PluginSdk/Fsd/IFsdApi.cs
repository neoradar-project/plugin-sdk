namespace NeoRadar.PluginSdk.Fsd;

/// <summary>
/// Provides read access to the user's FSD connection.
/// </summary>
public interface IFsdApi
{
    /// <summary>
    /// Gets the current connection details, or null when the user is not connected.
    /// </summary>
    ConnectionInfo? GetConnection();
}
