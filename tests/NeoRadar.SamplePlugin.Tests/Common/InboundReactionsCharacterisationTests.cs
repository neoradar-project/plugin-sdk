using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using NeoRadar.PluginSdk.Airport;
using NeoRadar.PluginSdk.Fsd;
using NeoRadar.SamplePlugin.Alerts;
using NeoRadar.SamplePlugin.Alerts.Events;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.RunwayBoard.Events;
using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.WhoIsOnline;
using NeoRadar.SamplePlugin.WhoIsOnline.Events;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Common;

// Each host event the who-is-online, alerts and runway board features react to, with every field set, and what each reported.
public sealed partial class InboundReactionsCharacterisationTests
{
    private const string FixtureName = "inbound-reactions.characterisation.txt";
    private const string UpdateVariable = "NEORADAR_UPDATE_SHOWCASE_FIXTURE";

    private static readonly Type[] s_areas =
    [
        typeof(IControllerEventHandler),
        typeof(IControllerDataEventHandler),
        typeof(IFsdEventHandler),
        typeof(IChatEventHandler),
        typeof(IAirportEventHandler),
        typeof(IFocusEventHandler),
    ];

    [Fact]
    public void EveryReaction_ReportsWhatTheFixtureRecords()
    {
        string actual = Run();
        string path = FixturePath();

        if (Environment.GetEnvironmentVariable(UpdateVariable) == "1") File.WriteAllText(path, actual);

        Assert.Equal(File.ReadAllText(path).ReplaceLineEndings("\n"), actual);
    }

    private static string Run()
    {
        FakeCoreApi core = new();
        core.FsdFake.Connection = new ConnectionInfo("ToController", 1, "Name", default, true, default, default, true, [], [118500]);
        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "Callsign", Position = new PluginSdk.Aircraft.Position { Latitude = 51.5, Longitude = -0.5 } });
        core.AirportFake.Configurations.Add(new AirportConfig("EGLL", ["27L"], ["27R"], AirportStatus.Active));

        using ServiceProvider provider = FeatureRegistrationTests.Compose(core);
        FeatureHost host = provider.GetRequiredService<FeatureHost>();
        host.Initialize();
        provider.GetRequiredService<IAlerts>().SetFollowSelected(true);
        IWhoIsOnline stations = provider.GetRequiredService<IWhoIsOnline>();

        List<string> heard = new();
        IPluginEventBus bus = provider.GetRequiredService<IPluginEventBus>();
        bus.Subscribe<StationsListed>(e => heard.Add($"stations: {e.Lines.Count - 3} recent, newest {e.Lines.ElementAtOrDefault(3)}"));
        bus.Subscribe<AlertActivityRecorded>(e => heard.Add("alerts: " + e.Lines[0]));
        bus.Subscribe<SelectedAircraftFollowed>(e => heard.Add($"followed: {e.Callsign} {e.Latitude} {e.Longitude}"));
        bus.Subscribe<RunwayBoardRefreshed>(e => heard.Add("runways: " + string.Join(" / ", e.Lines)));

        StringBuilder record = new();
        foreach (MethodInfo reaction in s_areas.SelectMany(a => a.GetMethods()).OrderBy(m => m.Name, StringComparer.Ordinal))
        {
            heard.Clear();
            int played = core.SoundFake.Played.Count;
            object payload = Build(reaction.GetParameters()[0].ParameterType);
            typeof(FeatureHost).GetMethod(reaction.Name)!.Invoke(host, [payload]);
            stations.Refresh();

            record.Append("## ").Append(reaction.Name).Append('\n');
            foreach (string line in heard) record.Append(Mask(line)).Append('\n');
            foreach (string sound in core.SoundFake.Played.Skip(played)) record.Append("sound: ").Append(Path.GetFileName(sound)).Append('\n');
        }

        host.Shutdown();
        return record.ToString();
    }

    private static object Build(Type type)
    {
        ConstructorInfo constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
        return constructor.Invoke(constructor.GetParameters().Select(p => Value(p.ParameterType, p.Name!)).ToArray());
    }

    private static object? Value(Type type, string name)
    {
        Type inner = Nullable.GetUnderlyingType(type) ?? type;
        if (inner == typeof(string)) return char.ToUpperInvariant(name[0]) + name[1..];
        if (inner == typeof(int)) return name.Length;
        if (inner == typeof(long)) return (long)name.Length;
        if (inner == typeof(double)) return name.Length + 0.5;
        if (inner == typeof(bool)) return true;
        if (inner.IsEnum) return Enum.GetValues(inner).GetValue(Enum.GetValues(inner).Length - 1);
        if (inner.IsGenericType && inner.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)) return Array.CreateInstance(inner.GetGenericArguments()[0], 2);
        return inner.IsValueType ? Activator.CreateInstance(inner) : null;
    }

    private static string FixturePath() => FixtureFiles.InTestProject(Path.Combine("Common", FixtureName));

    private static string Mask(string text) => TimeOfDay().Replace(text, "hh:mm:ss");

    [GeneratedRegex(@"\d{2}:\d{2}:\d{2}")]
    private static partial Regex TimeOfDay();
}
