using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using NeoRadar.PluginSdk.Airport;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.Tests.Common;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Showcase;

// Every Showcase button and input, run in nav order with Demo writes closed then open, and what each did.
public sealed partial class ShowcaseDemosCharacterisationTests
{
    private const string FixtureName = "showcase-demos.characterisation.txt";
    private const string UpdateVariable = "NEORADAR_UPDATE_SHOWCASE_FIXTURE";

    [Fact]
    public void EveryDemo_DoesWhatTheFixtureRecords()
    {
        string actual = Run(demoWrites: false) + Run(demoWrites: true);
        string path = FixturePath();

        if (Environment.GetEnvironmentVariable(UpdateVariable) == "1") File.WriteAllText(path, actual);

        Assert.Equal(File.ReadAllText(path).ReplaceLineEndings("\n"), actual);
    }

    private static string Run(bool demoWrites)
    {
        FakeCoreApi core = new();
        core.FocusFake.Selected = "BAW1";
        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "BAW1" });
        core.AircraftFake.All.Add(new PluginSdk.Aircraft.Aircraft { Callsign = "EZY2" });
        core.AirportFake.Configurations.Add(new AirportConfig("EGLL", ["27L"], ["27R"], AirportStatus.Active));

        using ServiceProvider provider = FeatureRegistrationTests.Compose(core);
        FeatureHost host = provider.GetRequiredService<FeatureHost>();
        host.Initialize();
        FakeWindowInterface windows = core.WindowFake.Interface;
        WindowHandle showcase = windows.Titled(ShowcaseWindowLayout.Title);
        host.OnWindowEvent(new WindowEvent(showcase, ShowcaseWindowLayout.DemoWrites, "change", demoWrites ? "true" : "false"));

        StringBuilder record = new();
        record.Append("# Demo writes ").Append(demoWrites ? "open" : "closed").Append('\n');
        Recorder recorder = new(core, showcase);
        foreach (ShowcasePage page in ShowcaseGuide.Pages)
        {
            host.OnWindowEvent(new WindowEvent(showcase, ShowcaseWindowLayout.NavId(page.Id), "click", "left"));
            foreach (DemoButton button in page.Buttons)
            {
                string id = ShowcasePageLayout.ButtonId(page, button.DemoId);
                recorder.Start();
                host.OnWindowEvent(new WindowEvent(showcase, id, "click", string.Empty));
                recorder.Write(record, id);
            }

            if (page.Input is not null)
            {
                string id = ShowcasePageLayout.InputId(page.Id);
                recorder.Start();
                host.OnWindowEvent(new WindowEvent(showcase, id, "submit", "LON"));
                recorder.Write(record, id);
            }
        }

        host.Shutdown();
        return record.ToString();
    }

    private static string FixturePath() => FixtureFiles.InTestProject(Path.Combine("Showcase", FixtureName));

    private static string Mask(string text)
    {
        text = TimeOfDay().Replace(text, "hh:mm:ss");
        text = Date().Replace(text, "yyyy-mm-dd");
        text = ThreadId().Replace(text, "thread N");
        return Microseconds().Replace(text, "N microseconds");
    }

    [GeneratedRegex(@"\d{2}:\d{2}:\d{2}")]
    private static partial Regex TimeOfDay();

    [GeneratedRegex(@"\d{4}-\d{2}-\d{2}")]
    private static partial Regex Date();

    [GeneratedRegex(@"thread \d+")]
    private static partial Regex ThreadId();

    [GeneratedRegex(@"\d+ microseconds")]
    private static partial Regex Microseconds();

    private sealed class Recorder(FakeCoreApi core, WindowHandle showcase)
    {
        private readonly Dictionary<string, IReadOnlyList<string>> _before = new();

        public void Start()
        {
            core.WindowFake.Interface.Calls.Clear();
            foreach ((string name, IReadOnlyList<string> lines) in Sources()) _before[name] = lines.ToArray();
        }

        public void Write(StringBuilder record, string id)
        {
            record.Append("## ").Append(id).Append('\n');
            foreach (string call in core.WindowFake.Interface.Calls)
            {
                record.Append("window: ").Append(call).Append('\n');
                string[] parts = call.Split(' ');
                if (parts[0] != "children") continue;
                foreach (WindowElement child in core.WindowFake.Interface.Children[(showcase, parts[1])])
                    record.Append("  | ").Append(Mask(child.Text)).Append('\n');
            }

            foreach ((string name, IReadOnlyList<string> lines) in Sources())
            {
                IReadOnlyList<string> before = _before[name];
                if (lines.Take(before.Count).SequenceEqual(before))
                {
                    foreach (string line in lines.Skip(before.Count)) record.Append(name).Append(": ").Append(Mask(line)).Append('\n');
                }
                else
                {
                    record.Append(name).Append(" now: ").Append(Mask(string.Join(", ", lines))).Append('\n');
                }
            }
        }

        private IEnumerable<(string Name, IReadOnlyList<string> Lines)> Sources()
        {
            yield return ("controller data", core.ControllerDataFake.Writes);
            yield return ("airport", core.AirportFake.Calls);
            yield return ("chat", core.ChatFake.Sent.Select(m => $"[{m.Channel}] {m.SentFrom}: {m.Message}").ToList());
            yield return ("expressions", core.ExpressionsFake.Pushes);
            yield return ("tag", core.TagFake.Interface.Pushes.Select(p => $"{p.TagId} {p.Value} {p.Context.Callsign}").ToList());
            yield return ("feedback", core.FeedbackFake.Shown.Select(s => $"{s.Kind} {s.Text}").ToList());
            yield return ("focus", core.FocusFake.SelectCalls);
            yield return ("squawk providers", core.SquawkFake.Providers.Select(p => $"{p.ProviderName} priority {p.Priority}").ToList());
            yield return ("squawk active", core.SquawkFake.Active is null ? [] : [core.SquawkFake.Active]);
            yield return ("settings", core.SettingsFake.Global.Keys.Concat(core.SettingsFake.PerProfile.Keys.Select(k => $"{k.Profile}/{k.Key}")).ToList());
            yield return ("screens", core.RenderingFake.Interface.Enabled.Select(e => $"{core.RenderingFake.Interface.Screens[e.Key].Definition.Name} {e.Value}").ToList());
            yield return ("windows", core.WindowFake.Interface.Open.Values.Select(d => d.Title).ToList());
        }
    }
}
