using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.PluginSdk.Timer;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests;

public sealed class SamplePluginTests
{
    private static readonly ClientInformation s_client = new("Test", "0.0.0", @"C:\NeoRadar");

    [Fact]
    public void EveryBasePluginEvent_IsForwarded_BySamplePlugin()
    {
        MethodInfo[] events = BasePluginEvents();

        string[] notForwarded = events
            .Where(e => typeof(SamplePlugin).GetMethod(e.Name, e.GetParameters().Select(p => p.ParameterType).ToArray())!.DeclaringType != typeof(SamplePlugin))
            .Select(e => e.Name)
            .ToArray();

        Assert.Equal(53, events.Length);
        Assert.Empty(notForwarded);
    }

    [Fact]
    public void EveryEvent_ReachesTheEventLog_AndItsCountShowsOnTheShowcase()
    {
        FakeCoreApi core = new();
        SamplePlugin plugin = new();
        plugin.Initialize(plugin.Manifest, core, s_client);
        WindowHandle showcase = core.WindowFake.Interface.Titled(ShowcaseWindowLayout.Title);
        plugin.OnWindowEvent(new WindowEvent(showcase, ShowcaseWindowLayout.NavId(ShowcaseGuide.Events), "click", "left"));

        foreach (MethodInfo evt in BasePluginEvents())
        {
            object payload = RuntimeHelpers.GetUninitializedObject(evt.GetParameters()[0].ParameterType);
            evt.Invoke(plugin, [payload]);
        }
        plugin.OnTimer(new TimerEvent(1));

        Assert.Contains($"rows {ShowcasePageLayout.CountsTable} 53", core.WindowFake.Interface.Calls);
        plugin.Shutdown();
    }

    [Fact]
    public void OnLoad_ThePluginSendsNothing_ToTheNetworkOrOtherUsers_AndOpensTheShowcase()
    {
        FakeCoreApi core = new();
        SamplePlugin plugin = new();

        plugin.Initialize(plugin.Manifest, core, s_client);

        Assert.Empty(core.ControllerDataFake.Writes);
        Assert.Empty(core.AirportFake.Calls);
        Assert.Empty(core.SquawkFake.Providers);
        Assert.Empty(core.ChatFake.Sent);
        Assert.Empty(core.ExpressionsFake.Pushes);
        Assert.True(core.WindowFake.Interface.Titled(ShowcaseWindowLayout.Title).IsValid);
        plugin.Shutdown();
    }

    [Fact]
    public void Shutdown_LeavesNoScreenCommandWindowOrMenuEntry_AndLogsNoError()
    {
        FakeCoreApi core = new();
        SamplePlugin plugin = new();

        plugin.Initialize(plugin.Manifest, core, s_client);
        Assert.NotEmpty(core.RenderingFake.Interface.Screens);
        plugin.Shutdown();

        Assert.Empty(core.RenderingFake.Interface.Screens);
        Assert.Empty(core.ChatFake.Commands);
        Assert.Empty(core.WindowFake.Interface.Open);
        Assert.DoesNotContain(core.LoggerFake.Lines, line => line.Level <= LogLevel.Error);
    }

    [Fact]
    public void TheManifest_NamesAWebsite_AndSaysWhatThePluginIs()
    {
        PluginManifest manifest = new SamplePlugin().Manifest;

        Assert.Equal("Sample", manifest.Name);
        Assert.StartsWith("https://", manifest.Website);
        Assert.False(string.IsNullOrWhiteSpace(manifest.Description));
    }

    private static MethodInfo[] BasePluginEvents()
        => typeof(BasePlugin)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.IsVirtual && m.Name.StartsWith("On"))
            .ToArray();
}
