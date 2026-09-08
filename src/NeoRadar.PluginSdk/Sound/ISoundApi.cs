namespace NeoRadar.PluginSdk.Sound;

/// <summary>
/// Play a sound file the plugin ships with itself. The host resolves nothing from the package on
/// a plugin's behalf, so the plugin owns the file and passes an absolute path to it.
/// </summary>
public interface ISoundApi
{
    /// <summary>
    /// Play a <c>.wav</c> or <c>.mp3</c> file at an absolute path the plugin owns, normally under
    /// <see cref="ClientInformation.DocumentsPath"/>. Returns false when the file does not exist,
    /// is not a supported format, or the call was throttled: playback is rate limited to roughly
    /// one call per second per plugin.
    /// </summary>
    bool Play(string filePath);
}
