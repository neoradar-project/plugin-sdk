namespace NeoRadar.PluginSdk.Sound;

/// <summary>
/// Provides playback of sound files the plugin ships with itself.
/// </summary>
public interface ISoundApi
{
    /// <summary>
    /// Plays a <c>.wav</c> file, or an <c>.mp3</c> on macOS and Linux only.
    /// </summary>
    /// <remarks>Rate limited per plugin to a burst of five, then one call per second.</remarks>
    /// <param name="filePath">An absolute path the plugin owns, normally under <see cref="ClientInformation.DocumentsPath"/>.</param>
    /// <returns>False if the file does not exist, is not a format this platform plays (an <c>.mp3</c> on Windows), or the call was throttled.</returns>
    bool Play(string filePath);
}
