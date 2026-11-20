namespace NeoRadar.PluginSdk.Sound;

/// <summary>
/// Provides playback of sound files the plugin ships with itself.
/// </summary>
public interface ISoundApi
{
    /// <summary>
    /// Plays a <c>.wav</c> or <c>.mp3</c> file.
    /// </summary>
    /// <remarks>Rate limited to roughly one call per second per plugin.</remarks>
    /// <param name="filePath">An absolute path the plugin owns, normally under <see cref="ClientInformation.DocumentsPath"/>.</param>
    /// <returns>False if the file does not exist, is not a supported format, or the call was throttled.</returns>
    bool Play(string filePath);
}
