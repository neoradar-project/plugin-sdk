using System.Collections.Generic;
using System.Linq;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Showcase;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Showcase;

public sealed class ShowcaseWindowLayoutTests
{
    private static readonly ShowcasePage s_first = ShowcaseGuide.Pages[0];

    private readonly WindowDefinition _window = ShowcaseWindowLayout.Build(
        ShowcaseGuide.Groups, s_first, ShowcasePageLayout.Build(s_first, false, [], []));

    [Fact]
    public void TheWindow_IsListed_WithTheDemoWritesTickUnticked_AndNoTabs()
    {
        Assert.True(_window.ShowInList);
        WindowElement tick = All(_window.Elements!).Single(e => e.Id == ShowcaseWindowLayout.DemoWrites);
        Assert.False(tick.IsChecked);
        Assert.DoesNotContain(All(_window.Elements!), e => e.Type is ElementType.Tabs or ElementType.TabPage);
    }

    [Fact]
    public void TheNav_ListsEveryPageInGuideOrder_UnderItsGroupHeading()
    {
        WindowElement grid = All(_window.Elements!).Single(e => e.Type == ElementType.Grid && e.Columns == "220,1,*");
        WindowElement nav = grid.Children[0].Children[0];

        string[] navTexts = nav.Children.Select(e => e.Type == ElementType.Button ? e.Id : e.Children[0].Text).ToArray();
        string[] expected = ShowcaseGuide.Groups
            .SelectMany(g => g.Pages.Select(p => ShowcaseWindowLayout.NavId(p.Id)).Prepend(g.Heading.ToUpperInvariant()))
            .ToArray();
        Assert.Equal(expected, navTexts);
    }

    [Fact]
    public void TheBody_FillsTheWindow_WithTheNavAndThePageScrollingApart_AboveTheFooter()
    {
        WindowElement body = Assert.Single(_window.Elements!);
        Assert.Equal((byte?)0, _window.Padding);

        Assert.Equal((ElementType.Grid, "*,auto", VAlign.Stretch), (body.Type, body.Rows, body.VAlign));
        Assert.Null(body.Style.Height);
        Assert.DoesNotContain(All(_window.Elements!), e => e.Style.MinHeight is not null);
        WindowElement[] scrollers = body.Children.Where(e => e.Type == ElementType.ScrollArea).ToArray();
        Assert.Equal([0, 2], scrollers.Select(e => e.Col));
        Assert.All(scrollers, e => Assert.Equal(0, e.Row));
        Assert.Contains(All(scrollers[1].Children), e => e.Id == ShowcaseWindowLayout.DetailPane);
        Assert.Contains(All(body.Children.Where(e => e.Row == 1)), e => e.Id == ShowcaseWindowLayout.DemoWrites);
    }

    [Fact]
    public void EveryNavItem_IsSquareAndFullWidth_WithItsLabelFromTheLeft()
    {
        foreach (bool active in new[] { true, false })
        {
            ElementStyle style = ShowcaseWindowLayout.NavItemStyle(active);
            Assert.Equal(HAlign.Left, style.ContentAlign);
            Assert.Equal((byte?)0, style.BorderRadius);
        }

        Assert.All(All(_window.Elements!).Where(e => ShowcaseWindowLayout.PageIdOf(e.Id) is not null), e => Assert.Equal(HAlign.Stretch, e.HAlign));
        WindowElement nav = Assert.Single(_window.Elements!).Children[0].Children[0];
        Assert.Equal(0, nav.Style.Spacing);
        Assert.Equal(string.Empty, ShowcaseWindowLayout.NavItemStyle(active: false).ThemeBg);
        Assert.Equal(ButtonVariant.Ghost, ShowcaseWindowLayout.NavItemStyle(active: false).Variant);
    }

    [Fact]
    public void OnlyTheShownPagesNavItem_IsActive_AndTheDetailPaneHoldsThatPage()
    {
        WindowElement[] navItems = All(_window.Elements!).Where(e => ShowcaseWindowLayout.PageIdOf(e.Id) is not null).ToArray();

        Assert.Equal(ShowcaseGuide.Pages.Count, navItems.Length);
        Assert.Equal([ShowcaseWindowLayout.NavId(s_first.Id)], navItems.Where(e => e.Style.ThemeBg == ShowcaseBrushes.ActiveItem).Select(e => e.Id));
        Assert.All(navItems.Where(e => e.Id != ShowcaseWindowLayout.NavId(s_first.Id)), e => Assert.Equal(ButtonVariant.Ghost, e.Style.Variant));

        WindowElement detail = All(_window.Elements!).Single(e => e.Id == ShowcaseWindowLayout.DetailPane);
        Assert.Contains(All(detail.Children), e => e.Type == ElementType.Heading && e.Text == s_first.Title);
    }

    [Fact]
    public void ANavItemId_ReadsBackAsItsPage_AndOtherIdsAsNone()
    {
        foreach (ShowcasePage page in ShowcaseGuide.Pages)
            Assert.Equal(page.Id, ShowcaseWindowLayout.PageIdOf(ShowcaseWindowLayout.NavId(page.Id)));

        Assert.Null(ShowcaseWindowLayout.PageIdOf(ShowcaseWindowLayout.DemoWrites));
        Assert.Null(ShowcaseWindowLayout.PageIdOf(ShowcaseWindowLayout.DetailPane));
    }

    [Fact]
    public void EveryElementId_IsUnique_WhicheverPageTheDetailPaneShows()
    {
        foreach (ShowcasePage page in ShowcaseGuide.Pages)
        {
            WindowDefinition window = ShowcaseWindowLayout.Build(ShowcaseGuide.Groups, page, ShowcasePageLayout.Build(page, false, [], []));
            string[] ids = All(window.Elements!).Select(e => e.Id).Where(id => id.Length > 0).ToArray();

            Assert.Equal(ids.Length, ids.Distinct().Count());
        }
    }

    [Fact]
    public void EveryColour_IsAShowcaseThemeBrush_NeverAnRgbValue()
    {
        string[] brushes = typeof(ShowcaseBrushes).GetFields().Select(f => (string)f.GetRawConstantValue()!).ToArray();
        IEnumerable<WindowElement> elements = All(_window.Elements!)
            .Concat(ShowcaseGuide.Pages.SelectMany(p => All(ShowcasePageLayout.Build(p, false, ["line"], []))));

        foreach (ElementStyle style in elements.Select(e => e.Style).Append(ShowcaseWindowLayout.NavItemStyle(true)).Append(ShowcaseWindowLayout.NavItemStyle(false)))
        {
            Assert.Null(style.BackgroundColor);
            Assert.Null(style.TextColor);
            Assert.Null(style.BorderColor);
            if (style.ThemeBg.Length > 0) Assert.Contains(style.ThemeBg, brushes);
            if (style.ThemeFg.Length > 0) Assert.Contains(style.ThemeFg, brushes);
            if (style.ThemeBorder.Length > 0) Assert.Contains(style.ThemeBorder, brushes);
        }
    }

    internal static IEnumerable<WindowElement> All(IEnumerable<WindowElement> elements)
        => elements.SelectMany(e => All(e.Children).Prepend(e));
}
