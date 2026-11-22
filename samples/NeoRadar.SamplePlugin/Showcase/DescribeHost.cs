using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Package;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>The Showcase's own demos about the host: what it told the plugin, what one call costs, the features.</summary>
internal sealed class DescribeHost
{
    private readonly ClientInformation _client;
    private readonly IPackageApi _package;
    private readonly IAircraftApi _aircraft;
    private readonly RegisteredFeatures _features;

    public DescribeHost(ClientInformation client, IPackageApi package, IAircraftApi aircraft, RegisteredFeatures features)
    {
        _client = client;
        _package = package;
        _aircraft = aircraft;
        _features = features;
    }

    public IReadOnlyList<string> PluginInfo() =>
    [
        $"Client: {_client.ClientName} {_client.Version}",
        $"Data root: {_client.DocumentsPath}",
        $"Active package: {_package.GetPackagePath() ?? "none"}",
        $"Built against SDK {PluginSdkVersion.Major}.{PluginSdkVersion.Minor}.{PluginSdkVersion.Patch}",
    ];

    public IReadOnlyList<string> TimeOneHostCall()
    {
        Stopwatch watch = Stopwatch.StartNew();
        int count = _aircraft.GetAll().Count;
        watch.Stop();
        double microseconds = watch.Elapsed.TotalMilliseconds * 1000;
        return [$"Aircraft.GetAll returned {count} aircraft in {microseconds.ToString("0", CultureInfo.InvariantCulture)} microseconds"];
    }

    public IReadOnlyList<string> Features()
    {
        List<string> lines = [$"{_features.Names.Count} features, in Initialize order:"];
        foreach (string name in _features.Names) lines.Add("  " + name);
        return lines;
    }

    public static string Thread(string where) => $"{where}: thread {Environment.CurrentManagedThreadId}";
}
