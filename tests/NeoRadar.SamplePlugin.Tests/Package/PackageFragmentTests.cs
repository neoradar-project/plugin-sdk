using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NeoRadar.PluginSdk;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Package;

public sealed partial class PackageFragmentTests
{
    [Fact]
    public void EveryPluginIdTheFragmentNames_IsOneThePluginRegisters()
    {
        FakeCoreApi core = new();
        SamplePlugin plugin = new();
        plugin.Initialize(plugin.Manifest, core, new ClientInformation("Test", "0.0.0", @"C:\NeoRadar"));
        HashSet<string> registered =
        [
            .. core.TagFake.Interface.Items.Select(i => "TAG_" + i.Name),
            .. core.TagFake.Interface.Actions.Select(a => "ACTION_" + a.Name),
            .. core.ExpressionsFake.Declared.Select(e => "EXP_" + e.Name),
        ];
        plugin.Shutdown();

        string[] named = Directory.EnumerateFiles(PackageFolder(), "*", SearchOption.AllDirectories)
            .SelectMany(file => PluginId().Matches(File.ReadAllText(file)).Select(m => m.Groups["name"].Value))
            .Distinct()
            .ToArray();

        Assert.NotEmpty(named);
        Assert.DoesNotContain(named, n => !registered.Contains(n));
    }

    private static string PackageFolder()
    {
        string folder = Path.Combine(AppContext.BaseDirectory, "Package");
        Assert.True(Directory.Exists(folder), $"the sample's Package folder is not beside the built plugin at {folder}");
        return folder;
    }

    [GeneratedRegex(@"plugin:Sample:(?<name>(TAG|ACTION|EXP)_[^""\]]+)")]
    private static partial Regex PluginId();
}
