namespace NeoRadar.PluginSdk.Fsd;

public interface IFsdApi
{
    /// <summary>Current FSD connection info, or null if not connected.</summary>
    ConnectionInfo? GetConnection();
}
