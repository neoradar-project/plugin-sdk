using System.IO;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>Writes a demo sound file the first time it is played; an existing file is kept.</summary>
internal static class SoundFileWriter
{
    public static void WriteIfMissing(string path, byte[] content)
    {
        if (File.Exists(path)) return;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, content);
    }
}
