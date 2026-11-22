using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Focus;
using NeoRadar.PluginSdk.Menu;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.Tests.Common;
using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.TrackWrites;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Showcase;

public sealed class ShowcaseFeatureTests : IDisposable
{
    private readonly FakeCoreApi _core = new();
    private readonly ServiceProvider _provider;
    private readonly FeatureHost _host;

    public ShowcaseFeatureTests()
    {
        _core.FocusFake.Selected = "BAW1";
        _provider = FeatureRegistrationTests.Compose(_core);
        _host = _provider.GetRequiredService<FeatureHost>();
        _host.Initialize();
    }

    private FakeWindowInterface Windows => _core.WindowFake.Interface;

    private WindowHandle Showcase => Windows.Titled(ShowcaseWindowLayout.Title);

    [Fact]
    public void OnLoad_TheShowcaseOpens_AndTheMenuEntryIsRegistered()
    {
        Assert.True(Showcase.IsValid);
        Assert.Single(_core.MenuFake.Items);
    }

    [Fact]
    public void ANetworkDemo_SendsNothing_UntilDemoWritesIsTicked()
    {
        Navigate(ShowcaseGuide.ControllerData);
        Click(ShowcaseGuide.ControllerData, "Assume");
        Assert.Empty(_core.ControllerDataFake.Writes);

        _host.OnWindowEvent(new WindowEvent(Showcase, ShowcaseWindowLayout.DemoWrites, "change", "true"));
        Click(ShowcaseGuide.ControllerData, "Assume");

        Assert.Equal(["assume BAW1"], _core.ControllerDataFake.Writes);
        Assert.True(Windows.EnabledStates[(Showcase, ShowcasePageLayout.ButtonId(Page(ShowcaseGuide.ControllerData), "Assume"))]);
    }

    [Fact]
    public void ClickingANavItem_SendsOneUpdateReplacingTheDetailPane_AndMarksThatItemActive()
    {
        Windows.Calls.Clear();

        Navigate(ShowcaseGuide.Squawk);

        Assert.Single(Windows.Calls, c => c.StartsWith("children ", StringComparison.Ordinal));
        Assert.StartsWith($"children {ShowcaseWindowLayout.DetailPane} ", Windows.Calls[^1], StringComparison.Ordinal);
        Assert.Equal(ShowcaseBrushes.ActiveItem, Windows.Styles[(Showcase, ShowcaseWindowLayout.NavId(ShowcaseGuide.Squawk))].ThemeBg);
    }

    [Fact]
    public void EveryPage_IsReachableFromTheNav()
    {
        foreach (ShowcasePage page in ShowcaseGuide.Pages.Reverse())
        {
            Navigate(page.Id);

            WindowElement title = Windows.Children[(Showcase, ShowcaseWindowLayout.DetailPane)][0].Children[0];
            Assert.Equal(page.Title, title.Text);
        }
    }

    [Fact]
    public void ANavItemNamingNoPage_IsLogged_AndChangesNothing()
    {
        Windows.Calls.Clear();

        _host.OnWindowEvent(new WindowEvent(Showcase, ShowcaseWindowLayout.NavId("nowhere"), "click", "left"));

        Assert.Empty(Windows.Calls);
        Assert.Contains(_core.LoggerFake.Lines, line => line.Message.Contains("no page nowhere", StringComparison.Ordinal));
    }

    [Fact]
    public void EveryDemo_Runs_WithoutThrowing()
    {
        _host.OnWindowEvent(new WindowEvent(Showcase, ShowcaseWindowLayout.DemoWrites, "change", "true"));

        foreach (ShowcasePage page in ShowcaseGuide.Pages)
            foreach (DemoButton button in page.Buttons)
                Click(page.Id, button.DemoId);

        // FeatureHost and the bus log a caught exception as [Type.Method]; the log-level demo writes its own Error line.
        Assert.DoesNotContain(_core.LoggerFake.Lines, line => line.Message.StartsWith('['));
    }

    [Fact]
    public void TheMenuEntry_OpensTheShowcaseAfresh_WithDemoWritesClosed()
    {
        _host.OnWindowEvent(new WindowEvent(Showcase, ShowcaseWindowLayout.DemoWrites, "change", "true"));
        WindowHandle first = Showcase;

        _host.OnMenuItemClicked(new MenuItemClickedEvent(_core.MenuFake.Items.Keys.Single(), false));

        Assert.NotEqual(first, Showcase);
        Assert.False(_provider.GetRequiredService<DemoWritesGate>().IsOpen);
    }

    [Fact]
    public void TheRateLimitBurst_UsesCallsThatNameNoAircraft_SoTheHostsBurstOfFiveShows()
    {
        _core.ChatFake.AllowedMessages = 5;
        Navigate(ShowcaseGuide.RateLimits);

        Click(ShowcaseGuide.RateLimits, "ChatBurst");

        Assert.Equal(5, _core.ChatFake.Sent.Count);
        WindowElement summary = Windows.Children[(Showcase, ShowcasePageLayout.ReadoutId(ShowcaseGuide.RateLimits))][0];
        Assert.Equal("Posted 5 of 7 lines at once; the host refused 2", summary.Text);
    }

    [Fact]
    public void TheScratchpadBurst_SaysOnlyOneIsSent_AsTheHostTakesOneASecondForTheSameAircraft()
    {
        DemoButton burst = Page(ShowcaseGuide.ControllerData).Buttons.Single(b => b.DemoId == "BurstScratchpad");

        Assert.StartsWith("one scratchpad update", burst.Sends);
    }

    [Fact]
    public void ANetworkButton_RunsNothing_BeforeDemoWrites_EvenWhereItsFeatureDoesNotCheck()
    {
        FakeCoreApi core = new();
        ProbeDemos assume = new("Assume");
        using ServiceProvider provider = ComposeWith(core, assume, replacing: typeof(TrackWritesDemos));
        FeatureHost host = provider.GetRequiredService<FeatureHost>();
        host.Initialize();
        WindowHandle showcase = core.WindowFake.Interface.Titled(ShowcaseWindowLayout.Title);
        string assumeButton = ShowcasePageLayout.ButtonId(Page(ShowcaseGuide.ControllerData), "Assume");
        host.OnWindowEvent(new WindowEvent(showcase, ShowcaseWindowLayout.NavId(ShowcaseGuide.ControllerData), "click", "left"));

        host.OnWindowEvent(new WindowEvent(showcase, assumeButton, "click", string.Empty));
        Assert.Equal(0, assume.Runs);

        host.OnWindowEvent(new WindowEvent(showcase, ShowcaseWindowLayout.DemoWrites, "change", "true"));
        host.OnWindowEvent(new WindowEvent(showcase, assumeButton, "click", string.Empty));
        Assert.Equal(1, assume.Runs);
        host.Shutdown();
    }

    [Fact]
    public void Shutdown_DisposesEveryReportSubscription()
    {
        ProbeDemos probe = new("Probe");
        using ServiceProvider provider = ComposeWith(new FakeCoreApi(), probe, replacing: null);
        FeatureHost host = provider.GetRequiredService<FeatureHost>();
        host.Initialize();

        host.Shutdown();

        Assert.True(probe.ReportsDisposed);
    }

    [Fact]
    public void ASelectionChange_FillsTheFlightPlanReadout()
    {
        Navigate(ShowcaseGuide.FlightPlans);

        _host.OnSelectedAircraftChanged(new SelectedAircraftChangedEvent("BAW1"));

        Assert.True(Windows.Children.ContainsKey((Showcase, ShowcasePageLayout.ReadoutId(ShowcaseGuide.FlightPlans))));
    }

    public void Dispose()
    {
        _host.Shutdown();
        _provider.Dispose();
    }

    private void Navigate(string pageId)
        => _host.OnWindowEvent(new WindowEvent(Showcase, ShowcaseWindowLayout.NavId(pageId), "click", "left"));

    private void Click(string pageId, string demoId)
        => _host.OnWindowEvent(new WindowEvent(Showcase, ShowcasePageLayout.ButtonId(Page(pageId), demoId), "click", string.Empty));

    private static ShowcasePage Page(string id) => ShowcaseGuide.Pages.Single(p => p.Id == id);

    private static ServiceProvider ComposeWith(FakeCoreApi core, IShowcaseDemos extra, Type? replacing)
    {
        ServiceCollection services = new();
        services.AddSamplePlugin(core, new ClientInformation("Test", "0.0.0", "C:/NeoRadar"));
        if (replacing is not null) services.Remove(services.Single(d => d.ImplementationType == replacing));
        services.AddSingleton(extra);
        return services.BuildServiceProvider();
    }

    private sealed class ProbeDemos(string id) : IShowcaseDemos, IDisposable
    {
        public int Runs { get; private set; }

        public bool ReportsDisposed { get; private set; }

        public IReadOnlyList<ShowcaseDemo> Demos => [new(id, () => Runs++)];

        public IReadOnlyList<IDisposable> ShowReports() => [this];

        public void Dispose() => ReportsDisposed = true;
    }
}
