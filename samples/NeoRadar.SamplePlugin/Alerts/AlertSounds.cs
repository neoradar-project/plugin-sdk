using System.IO;
using NeoRadar.PluginSdk;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>Where the alerts demo keeps its sound files, under the client's data root.</summary>
internal sealed record AlertSounds(string WavPath, string Mp3Path)
{
    public static AlertSounds In(ClientInformation client)
    {
        string folder = Path.Combine(client.DocumentsPath, "SamplePlugin");
        return new AlertSounds(Path.Combine(folder, "sample-beep.wav"), Path.Combine(folder, "sample-beep.mp3"));
    }
}
