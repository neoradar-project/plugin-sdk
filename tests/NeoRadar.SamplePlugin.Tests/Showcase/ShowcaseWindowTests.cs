using System.Collections.Generic;
using System.Linq;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.EventLog.Events;
using NeoRadar.SamplePlugin.Showcase;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;
using static NeoRadar.SamplePlugin.Tests.Showcase.ShowcaseWindowLayoutTests;

namespace NeoRadar.SamplePlugin.Tests.Showcase;

public sealed class ShowcaseWindowTests
{
    private readonly FakeWindowInterface _windows = new();
    private readonly DemoWritesGate _gate = new();
    private readonly ShowcaseWindow _window;
    private readonly WindowHandle _handle;

    public ShowcaseWindowTests()
    {
        _window = new ShowcaseWindow(_windows, _gate);
        _handle = _window.Open();
        _windows.Calls.Clear();
    }

    [Fact]
    public void Open_StartsOnTheFirstPage_WithDemoWritesClosed()
    {
        _gate.Set(true);

        WindowHandle reopened = _window.Open();

        Assert.True(reopened.IsValid);
        Assert.Contains(All(_windows.Open[reopened].Elements ?? []), e => e.Type == ElementType.Heading && e.Text == ShowcaseGuide.Pages[0].Title);
        Assert.False(_gate.IsOpen);
    }

    [Fact]
    public void ShowPage_ReplacesTheDetailPaneOnce_AndMovesTheActiveMarkToItsNavItem()
    {
        ShowcasePage airports = Page(ShowcaseGuide.Airports);

        _window.ShowPage(airports);

        Assert.Equal(
            [
                $"style {ShowcaseWindowLayout.NavId(ShowcaseGuide.Start)}",
                $"style {ShowcaseWindowLayout.NavId(ShowcaseGuide.Airports)}",
                $"children {ShowcaseWindowLayout.DetailPane} {ShowcasePageLayout.Build(airports, false, [], []).Count}",
            ],
            _windows.Calls);
        Assert.Equal(ShowcaseBrushes.ActiveItem, _windows.Styles[(_handle, ShowcaseWindowLayout.NavId(ShowcaseGuide.Airports))].ThemeBg);
        Assert.Equal(ButtonVariant.Ghost, _windows.Styles[(_handle, ShowcaseWindowLayout.NavId(ShowcaseGuide.Start))].Variant);
        Assert.Contains(Detail(), e => e.Type == ElementType.Heading && e.Text == airports.Title);
    }

    [Fact]
    public void ShowPage_OfThePageAlreadyShown_SendsNothing()
    {
        _window.ShowPage(ShowcaseGuide.Pages[0]);

        Assert.Empty(_windows.Calls);
    }

    [Fact]
    public void AReadoutForTheShownPage_ReplacesOnlyThatReadout()
    {
        _window.ShowPage(Page(ShowcaseGuide.FlightPlans));
        _windows.Calls.Clear();

        _window.ShowReadout(ShowcaseGuide.FlightPlans, ["BAW1 EGLL EGKK"]);

        Assert.Equal([$"children {ShowcasePageLayout.ReadoutId(ShowcaseGuide.FlightPlans)} 1"], _windows.Calls);
    }

    [Fact]
    public void AReadoutForAPageNotShown_SendsNothing_AndIsShownWhenItsPageOpens()
    {
        _window.ShowReadout(ShowcaseGuide.FlightPlans, ["BAW1 EGLL EGKK"]);
        Assert.Empty(_windows.Calls);

        _window.ShowPage(Page(ShowcaseGuide.FlightPlans));

        WindowElement readout = Detail().Single(e => e.Id == ShowcasePageLayout.ReadoutId(ShowcaseGuide.FlightPlans));
        Assert.Equal(["BAW1 EGLL EGKK"], readout.Children.Select(e => e.Text));
    }

    [Fact]
    public void SetDemoWrites_EnablesOnlyTheShownPagesNetworkButtons_AndAPageOpenedLaterHasThemEnabled()
    {
        ShowcasePage controllerData = Page(ShowcaseGuide.ControllerData);
        ShowcasePage airports = Page(ShowcaseGuide.Airports);
        _window.ShowPage(controllerData);
        _windows.Calls.Clear();

        _window.SetDemoWrites(true);

        Assert.True(_gate.IsOpen);
        Assert.Equal(controllerData.Buttons.Count(b => b.SendsToNetwork), _windows.Calls.Count);
        Assert.All(_windows.EnabledStates, entry => Assert.True(entry.Value));

        _window.ShowPage(airports);
        Assert.All(
            Detail().Where(e => e.Type == ElementType.Button),
            button => Assert.True(button.Enabled));
    }

    [Fact]
    public void Counts_AreKeptWhileTheEventLogIsNotShown_AndFillItsTableWhenItOpens()
    {
        EventsCounted counted = new([new EventCount("OnTimer", 3)], ["OnTimer on thread 4"]);

        _window.ShowCounts(counted);
        Assert.Empty(_windows.Calls);

        _window.ShowPage(Page(ShowcaseGuide.Events));
        WindowElement table = Detail().Single(e => e.Id == ShowcasePageLayout.CountsTable);
        Assert.Equal(["OnTimer"], table.Table!.Rows.Select(r => r.Id));

        _windows.Calls.Clear();
        _window.ShowCounts(counted);
        Assert.Equal([$"rows {ShowcasePageLayout.CountsTable} 1", $"children {ShowcasePageLayout.ReadoutId(ShowcaseGuide.Events)} 1"], _windows.Calls);
    }

    [Fact]
    public void AfterClose_NothingIsSent()
    {
        _window.Close();
        _windows.Calls.Clear();

        _window.ShowPage(Page(ShowcaseGuide.Airports));
        _window.ShowReadout(ShowcaseGuide.Start, ["line"]);
        _window.SetDemoWrites(true);

        Assert.Empty(_windows.Calls);
        Assert.False(_window.Handle.IsValid);
    }

    private IEnumerable<WindowElement> Detail() => All(_windows.Children[(_handle, ShowcaseWindowLayout.DetailPane)]);

    private static ShowcasePage Page(string id) => ShowcaseGuide.Pages.Single(p => p.Id == id);
}
