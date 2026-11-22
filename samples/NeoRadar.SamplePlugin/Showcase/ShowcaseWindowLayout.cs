using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Window;
using B = NeoRadar.SamplePlugin.Showcase.ShowcaseBrushes;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>Builds the Showcase window as Package Settings is laid out: a grouped nav beside the page, a footer below.</summary>
internal static class ShowcaseWindowLayout
{
    public const string Title = "Sample Plugin Showcase";
    public const string DemoWrites = "demo-writes";
    public const string DetailPane = "detail";

    private const string NavPrefix = "nav-";

    /// <param name="shown">The page the detail pane opens on, marked active in the nav.</param>
    public static WindowDefinition Build(IReadOnlyList<ShowcaseNavGroup> groups, ShowcasePage shown, IReadOnlyList<WindowElement> shownContent)
    {
        return new WindowDefinition(
            Title: Title,
            Width: 860,
            Height: 680,
            Icon: "BookOpen",
            ShowInList: true,
            Elements: [Body(groups, shown, shownContent)],
            Resizable: true,
            MinWidth: 640,
            MinHeight: 420)
        {
            Padding = 0,
        };
    }

    public static string NavId(string pageId) => NavPrefix + pageId;

    /// <summary>Reads the page a nav item id names, or null for an id that is not a nav item.</summary>
    public static string? PageIdOf(string elementId)
        => elementId.StartsWith(NavPrefix, StringComparison.Ordinal) ? elementId[NavPrefix.Length..] : null;

    // Full width, square, the label from the left: a Package Settings nav row.
    private static readonly ElementStyle s_navItem = new()
    {
        Height = 30,
        FontSize = 12,
        PaddingH = 10,
        PaddingV = 0,
        BorderRadius = 0,
        ContentAlign = HAlign.Left,
    };

    public static ElementStyle NavItemStyle(bool active) => active
        ? s_navItem with { ThemeBg = B.ActiveItem, ThemeFg = B.Text }
        : s_navItem with { Variant = ButtonVariant.Ghost, ThemeFg = B.SecondaryText };

    // The star row fills the window, so the nav and the page each scroll on their own above the footer.
    private static WindowElement Body(IReadOnlyList<ShowcaseNavGroup> groups, ShowcasePage shown, IReadOnlyList<WindowElement> shownContent) => new()
    {
        Type = ElementType.Grid,
        Columns = "220,1,*",
        Rows = "*,auto",
        Style = new() { ThemeBg = B.PageBackground },
        Children =
        [
            new() { Type = ElementType.ScrollArea, Style = new() { ThemeBg = B.NavBackground }, Children = [Nav(groups, shown)] },
            new() { Type = ElementType.Panel, Col = 1, Style = new() { ThemeBg = B.Rule } },
            new()
            {
                Type = ElementType.ScrollArea,
                Col = 2,
                Children =
                [
                    new()
                    {
                        Type = ElementType.Panel,
                        Id = DetailPane,
                        Style = new() { PaddingH = 24, PaddingV = 18, Spacing = 14 },
                        Children = shownContent,
                    },
                ],
            },
            Footer(),
        ],
    };

    private static WindowElement Nav(IReadOnlyList<ShowcaseNavGroup> groups, ShowcasePage shown)
    {
        List<WindowElement> items = new();
        foreach (ShowcaseNavGroup group in groups)
        {
            items.Add(GroupHeading(group.Heading));
            foreach (ShowcasePage page in group.Pages)
            {
                items.Add(new()
                {
                    Type = ElementType.Button,
                    Id = NavId(page.Id),
                    Text = page.Title,
                    Tooltip = page.Summary,
                    Style = NavItemStyle(page.Id == shown.Id),
                });
            }
        }

        return new() { Type = ElementType.Panel, Style = new() { PaddingH = 8, PaddingV = 6, Spacing = 0 }, Children = items };
    }

    private static WindowElement GroupHeading(string heading) => new()
    {
        Type = ElementType.Grid,
        Columns = "auto,*",
        Style = new() { PaddingH = 8, PaddingV = 6 },
        Children =
        [
            new() { Type = ElementType.Label, Text = heading.ToUpperInvariant(), VAlign = VAlign.Center, Style = new() { FontSize = 9, FontWeight = FontWeight.SemiBold, ThemeFg = B.DimText } },
            new() { Type = ElementType.Divider, Col = 1, VAlign = VAlign.Center, Style = new() { ThemeBg = B.SubtleRule, MarginH = 6 } },
        ],
    };

    private static WindowElement Footer() => new()
    {
        Type = ElementType.Panel,
        Row = 1,
        ColSpan = 3,
        Style = new() { ThemeBg = B.NavBackground },
        Children =
        [
            new() { Type = ElementType.Divider, Style = new() { ThemeBg = B.Rule, MarginV = 0 } },
            new()
            {
                Type = ElementType.Grid,
                Columns = "*,auto",
                Style = new() { PaddingH = 14, PaddingV = 8 },
                Children =
                [
                    new()
                    {
                        Type = ElementType.Checkbox,
                        Id = DemoWrites,
                        Text = "Demo writes: allow the buttons that send to the network",
                        Tooltip = "Unticked at every load and never remembered",
                        VAlign = VAlign.Center,
                    },
                    new()
                    {
                        Type = ElementType.Label,
                        Col = 1,
                        Text = $"Sample plugin · SDK {PluginSdkVersion.Major}.{PluginSdkVersion.Minor}.{PluginSdkVersion.Patch}",
                        VAlign = VAlign.Center,
                        Style = new() { FontSize = 11, ThemeFg = B.DimText },
                    },
                ],
            },
        ],
    };
}
