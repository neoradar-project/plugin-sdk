using System;
using System.IO;
using System.Text;

namespace NeoRadar.SamplePlugin.Alerts;

/// <summary>A short tone as a 16-bit mono PCM <c>.wav</c>, so the sample ships no sound file.</summary>
internal static class BeepWave
{
    public const int SampleRate = 22_050;

    public static byte[] Build(double frequencyHz = 880, double seconds = 0.2)
    {
        int samples = (int)(SampleRate * seconds);
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream, Encoding.ASCII);

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + samples * 2);
        writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(SampleRate);
        writer.Write(SampleRate * 2);
        writer.Write((short)2);
        writer.Write((short)16);
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(samples * 2);
        for (int i = 0; i < samples; i++)
            writer.Write((short)(Math.Sin(2 * Math.PI * frequencyHz * i / SampleRate) * short.MaxValue * 0.3));

        writer.Flush();
        return stream.ToArray();
    }
}
