using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace NeoRadar.PluginSdk.Tests.PublicApi;

// The public surface frozen with the version that ships it, so an addition cannot ship without
// the next patch and a break cannot ship without the next minor.
public sealed class PublicApiBaselineTests
{
    private const string UpdateVariable = "NEORADAR_UPDATE_SDK_API";
    private const string VersionPrefix = "sdk-version ";
    private const string BaselineRelativePath = "tests/NeoRadar.PluginSdk.Tests/PublicApi/public-api.txt";

    [Fact]
    public void Surface_IsTheOneFrozenForThisVersion()
    {
        string path = Path.Combine(RepoRoot(), BaselineRelativePath);
        Version current = new(PluginSdkVersion.Major, PluginSdkVersion.Minor, PluginSdkVersion.Patch);
        List<string> surface = PublicApiSurface.Describe(typeof(PluginSdkVersion).Assembly);
        bool updating = Environment.GetEnvironmentVariable(UpdateVariable) == "1";

        if (!File.Exists(path))
        {
            Assert.True(updating, $"No public API is frozen. Rerun with {UpdateVariable}=1 to write {BaselineRelativePath}.");
            Write(path, current, surface);
            return;
        }

        (Version frozen, HashSet<string> frozenLines) = Read(path);
        string[] added = surface.Where(l => !frozenLines.Contains(l)).ToArray();
        string[] removed = frozenLines.Where(l => !surface.Contains(l)).OrderBy(l => l, StringComparer.Ordinal).ToArray();
        string? refusal = PublicApiVersionRule.Refusal(frozen, current, added.Length, removed.Length);
        string changes = Describe(added, removed);

        Assert.True(refusal is null, $"{refusal}{changes}");
        if (added.Length == 0 && removed.Length == 0) return;

        Assert.True(
            updating,
            $"The public API changed and the version moved from {frozen} to {current} as it must. " +
            $"Rerun with {UpdateVariable}=1 to freeze it.{changes}");
        Write(path, current, surface);
    }

    private static (Version Version, HashSet<string> Lines) Read(string path)
    {
        string[] lines = File.ReadAllText(path).Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.StartsWith(VersionPrefix, lines[0]);
        return (Version.Parse(lines[0][VersionPrefix.Length..]), lines.Skip(1).ToHashSet(StringComparer.Ordinal));
    }

    private static void Write(string path, Version version, List<string> surface)
    {
        StringBuilder text = new();
        text.Append(VersionPrefix).Append(version).Append('\n');
        foreach (string line in surface) text.Append(line).Append('\n');
        File.WriteAllText(path, text.ToString(), new UTF8Encoding(false));
    }

    private static string Describe(string[] added, string[] removed)
    {
        StringBuilder text = new();
        foreach (string line in removed) text.Append("\n- ").Append(line);
        foreach (string line in added) text.Append("\n+ ").Append(line);
        return text.ToString();
    }

    private static string RepoRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NeoRadar.PluginSdk.slnx")))
            directory = directory.Parent;
        Assert.NotNull(directory);
        return directory.FullName;
    }
}
