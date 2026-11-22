using System;
using System.Text;
using NeoRadar.SamplePlugin.Alerts;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Alerts;

public sealed class BeepWaveTests
{
    [Fact]
    public void TheTone_IsAPcmWave_WhoseSizesMatchItsLength()
    {
        byte[] wave = BeepWave.Build(seconds: 0.1);

        Assert.Equal("RIFF", Encoding.ASCII.GetString(wave, 0, 4));
        Assert.Equal("WAVE", Encoding.ASCII.GetString(wave, 8, 4));
        Assert.Equal(wave.Length - 8, BitConverter.ToInt32(wave, 4));
        Assert.Equal(wave.Length - 44, BitConverter.ToInt32(wave, 40));
    }
}
