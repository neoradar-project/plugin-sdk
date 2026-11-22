using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Sound;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

// Refuses an .mp3, as the host on Windows does.
internal sealed class FakeSoundApi : ISoundApi
{
    public List<string> Played { get; } = new();

    public bool Play(string filePath)
    {
        Played.Add(filePath);
        return !filePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase);
    }
}
