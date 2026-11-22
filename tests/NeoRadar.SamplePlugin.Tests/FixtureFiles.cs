using System;
using System.IO;

namespace NeoRadar.SamplePlugin.Tests;

// A CI build maps source paths to /_/, so a fixture is found from the repo root, never [CallerFilePath].
internal static class FixtureFiles
{
    private const string RootMarker = "NeoRadar.PluginSdk.slnx";

    public static string InTestProject(string relativePath)
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, RootMarker)))
            directory = directory.Parent;
        if (directory is null)
            throw new InvalidOperationException($"No {RootMarker} above {AppContext.BaseDirectory}.");
        return Path.Combine(directory.FullName, "tests", "NeoRadar.SamplePlugin.Tests", relativePath);
    }
}
