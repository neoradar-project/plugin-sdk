using System.Collections.Generic;
using System.Linq;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Showcase;
using Xunit;
using static NeoRadar.SamplePlugin.Tests.Showcase.ShowcaseWindowLayoutTests;

namespace NeoRadar.SamplePlugin.Tests.Showcase;

public sealed class ShowcasePageLayoutTests
{
    [Fact]
    public void APage_OpensWithItsTitleAndSummary_ThenACardPerSection_ThenItsReadout()
    {
        ShowcasePage page = Page(ShowcaseGuide.ControllerData);

        IReadOnlyList<WindowElement> content = ShowcasePageLayout.Build(page, false, [], []);

        Assert.Equal([page.Title, page.Summary], content[0].Children.Select(e => e.Text));
        WindowElement[] cards = content.Where(e => e.Style.ThemeBg == ShowcaseBrushes.CardBackground).ToArray();
        Assert.Equal(page.Sections.Select(s => s.Heading.ToUpperInvariant()).Append("READOUT"), cards.Select(c => c.Children[0].Children[0].Text));
        Assert.All(cards, c => Assert.Equal((ShowcaseBrushes.CardBorder, (byte?)1, (byte?)0), (c.Style.ThemeBorder, c.Style.BorderWidth, c.Style.BorderRadius)));
        Assert.Equal($"Read next: Plugins, {page.ReadNext}", content[^1].Text);
    }

    [Fact]
    public void APagesTextInput_StartsAtTheLeftOfItsCard()
    {
        WindowElement[] inputs = ShowcaseGuide.Pages
            .SelectMany(p => All(ShowcasePageLayout.Build(p, false, [], [])))
            .Where(e => e.Type == ElementType.TextInput)
            .ToArray();

        Assert.NotEmpty(inputs);
        Assert.All(inputs, e => Assert.Equal(HAlign.Left, e.HAlign));
    }

    [Fact]
    public void ANetworkButton_IsDisabledUntilDemoWritesIsOpen_AndALocalOneAlwaysEnabled()
    {
        foreach (ShowcasePage page in ShowcaseGuide.Pages)
        {
            Dictionary<string, WindowElement> closed = Buttons(page, demoWritesOpen: false);
            Dictionary<string, WindowElement> open = Buttons(page, demoWritesOpen: true);

            foreach (DemoButton button in page.Buttons)
            {
                string id = ShowcasePageLayout.ButtonId(page, button.DemoId);
                Assert.Equal(!button.SendsToNetwork, closed[id].Enabled);
                Assert.True(open[id].Enabled);
            }
        }
    }

    [Fact]
    public void ASectionWithNetworkButtons_CarriesTheSendsBadge_AndNoOtherDoes()
    {
        foreach (ShowcasePage page in ShowcaseGuide.Pages)
        {
            IReadOnlyList<WindowElement> content = ShowcasePageLayout.Build(page, false, [], []);
            int badges = All(content).Count(e => e.Type == ElementType.Badge);

            Assert.Equal(page.Sections.Count(s => s.SendsToNetwork), badges);
        }
    }

    [Fact]
    public void AReadout_ShowsItsLines_OrSaysNothingYetBeforeAnyDemoReported()
    {
        ShowcasePage page = Page(ShowcaseGuide.FlightPlans);

        WindowElement empty = Readout(ShowcasePageLayout.Build(page, false, [], []), page);
        WindowElement filled = Readout(ShowcasePageLayout.Build(page, false, ["BAW1 EGLL EGKK"], []), page);

        Assert.Equal([ShowcasePageLayout.NothingYet], empty.Children.Select(e => e.Text));
        Assert.Equal(["BAW1 EGLL EGKK"], filled.Children.Select(e => e.Text));
    }

    [Fact]
    public void TheEventLogPage_CarriesTheCountsTable_WithTheCountsItWasGiven()
    {
        TableRow[] counts = [new("OnTimer", ["OnTimer", "3"])];

        IReadOnlyList<WindowElement> content = ShowcasePageLayout.Build(Page(ShowcaseGuide.Events), false, [], counts);

        WindowElement table = All(content).Single(e => e.Id == ShowcasePageLayout.CountsTable);
        Assert.Equal(counts, table.Table!.Rows);
    }

    [Fact]
    public void AButtonId_ReadsBackAsItsDemo_AndOtherIdsAsNone()
    {
        foreach (ShowcasePage page in ShowcaseGuide.Pages)
            foreach (DemoButton button in page.Buttons)
                Assert.Equal(button.DemoId, ShowcasePageLayout.DemoIdOf(ShowcasePageLayout.ButtonId(page, button.DemoId)));

        Assert.Null(ShowcasePageLayout.DemoIdOf(ShowcaseWindowLayout.DemoWrites));
        Assert.Null(ShowcasePageLayout.DemoIdOf(ShowcaseWindowLayout.NavId(ShowcaseGuide.Start)));
        Assert.Null(ShowcasePageLayout.DemoIdOf(ShowcasePageLayout.ReadoutId(ShowcaseGuide.Start)));
    }

    private static Dictionary<string, WindowElement> Buttons(ShowcasePage page, bool demoWritesOpen)
        => All(ShowcasePageLayout.Build(page, demoWritesOpen, [], [])).Where(e => e.Type == ElementType.Button).ToDictionary(e => e.Id);

    private static WindowElement Readout(IReadOnlyList<WindowElement> content, ShowcasePage page)
        => All(content).Single(e => e.Id == ShowcasePageLayout.ReadoutId(page.Id));

    private static ShowcasePage Page(string id) => ShowcaseGuide.Pages.Single(p => p.Id == id);
}
