using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Window;
using B = NeoRadar.SamplePlugin.Showcase.ShowcaseBrushes;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>Builds one guide page for the detail pane: its title, a card per section, its live readout and what to read next.</summary>
internal static class ShowcasePageLayout
{
    public const string CountsTable = "counts";
    public const string NothingYet = "Nothing yet: run a demo above and what it reports appears here.";

    private const string ButtonPrefix = "demo-";

    private static readonly TableColumn[] s_countColumns =
    [
        new("event", "Event", "*"),
        new("count", "Count", "70", HAlign.Right),
    ];

    /// <param name="readout">The page's last readout lines, empty before any demo reported.</param>
    /// <param name="counts">The event log's last counts, shown only on the page with the counts table.</param>
    public static IReadOnlyList<WindowElement> Build(
        ShowcasePage page, bool demoWritesOpen, IReadOnlyList<string> readout, IReadOnlyList<TableRow> counts)
    {
        List<WindowElement> content = [Header(page)];
        foreach (ShowcaseSection section in page.Sections) content.Add(Card(SectionContent(page, section, demoWritesOpen)));
        if (page.HasReadout || page.HasCountsTable) content.Add(Card(ReadoutContent(page, readout, counts)));
        content.Add(new() { Type = ElementType.Label, Text = $"Read next: Plugins, {page.ReadNext}", Style = new() { FontSize = 11, ThemeFg = B.DimText } });
        return content;
    }

    public static IReadOnlyList<WindowElement> ReadoutLines(IReadOnlyList<string> lines)
    {
        if (lines.Count == 0) return [new() { Type = ElementType.Label, Text = NothingYet, Style = new() { FontSize = 11, ThemeFg = B.DimText } }];

        List<WindowElement> children = new(lines.Count);
        foreach (string line in lines)
            children.Add(new() { Type = ElementType.Paragraph, Text = line, Style = new() { FontSize = 12, ThemeFg = B.Text } });
        return children;
    }

    public static string ButtonId(ShowcasePage page, string demoId) => $"{ButtonPrefix}{page.Id}-{demoId}";

    public static string ReadoutId(string pageId) => $"readout-{pageId}";

    public static string InputId(string pageId) => $"input-{pageId}";

    /// <summary>Reads the demo id a button id names, or null for an id that is not a demo button.</summary>
    public static string? DemoIdOf(string elementId)
    {
        if (!elementId.StartsWith(ButtonPrefix, StringComparison.Ordinal)) return null;
        int dash = elementId.IndexOf('-', ButtonPrefix.Length);
        return dash < 0 || dash == elementId.Length - 1 ? null : elementId[(dash + 1)..];
    }

    private static WindowElement Header(ShowcasePage page) => new()
    {
        Type = ElementType.Panel,
        Style = new() { Spacing = 4 },
        Children =
        [
            new() { Type = ElementType.Heading, Text = page.Title, Style = new() { FontSize = 19, FontWeight = FontWeight.Bold, ThemeFg = B.Text } },
            new() { Type = ElementType.Label, Text = page.Summary, Style = new() { FontSize = 11, ThemeFg = B.DimText } },
        ],
    };

    private static WindowElement Card(List<WindowElement> children) => new()
    {
        Type = ElementType.Panel,
        Style = new() { ThemeBg = B.CardBackground, ThemeBorder = B.CardBorder, BorderWidth = 1, BorderRadius = 0, Padding = 12, Spacing = 8 },
        Children = children,
    };

    private static List<WindowElement> SectionContent(ShowcasePage page, ShowcaseSection section, bool demoWritesOpen)
    {
        List<WindowElement> content = CardHeading(section.Heading, section.SendsToNetwork);
        foreach (string paragraph in section.Paragraphs)
            content.Add(new() { Type = ElementType.Paragraph, Text = paragraph });

        if (section.Buttons.Count > 0)
            content.Add(new() { Type = ElementType.Panel, Direction = 1, Wrap = true, Style = new() { Spacing = 6 }, Children = Buttons(page, section, demoWritesOpen) });

        if (section.Input is PageInput input)
            content.Add(new() { Type = ElementType.TextInput, Id = InputId(page.Id), Data = input.Placeholder, HAlign = HAlign.Left, Style = new() { Width = 220 } });

        return content;
    }

    private static List<WindowElement> ReadoutContent(ShowcasePage page, IReadOnlyList<string> readout, IReadOnlyList<TableRow> counts)
    {
        List<WindowElement> content = CardHeading("Readout", sendsToNetwork: false);
        if (page.HasCountsTable)
        {
            content.Add(new()
            {
                Type = ElementType.Table,
                Id = CountsTable,
                Style = new() { MaxHeight = 220 },
                Table = new TableDefinition(s_countColumns, counts),
            });
        }

        if (page.HasReadout)
            content.Add(new() { Type = ElementType.Panel, Id = ReadoutId(page.Id), Style = new() { Spacing = 2 }, Children = ReadoutLines(readout) });

        return content;
    }

    private static List<WindowElement> CardHeading(string heading, bool sendsToNetwork)
    {
        List<WindowElement> row =
        [
            new() { Type = ElementType.Label, Text = heading.ToUpperInvariant(), VAlign = VAlign.Center, Style = new() { FontSize = 10, FontWeight = FontWeight.Bold, ThemeFg = B.SecondaryText } },
        ];
        if (sendsToNetwork)
        {
            row.Add(new()
            {
                Type = ElementType.Badge,
                Col = 1,
                Text = "Sends to the network",
                VAlign = VAlign.Center,
                Tooltip = "Its red buttons need Demo writes, at the foot of the window",
                Style = new() { FontSize = 10, ThemeBg = B.DangerBackground, ThemeFg = B.DangerText },
            });
        }

        return
        [
            new() { Type = ElementType.Grid, Columns = "*,auto", Children = row },
            new() { Type = ElementType.Divider, Style = new() { ThemeBg = B.SubtleRule, MarginV = 0 } },
        ];
    }

    private static List<WindowElement> Buttons(ShowcasePage page, ShowcaseSection section, bool demoWritesOpen)
    {
        List<WindowElement> buttons = new();
        foreach (DemoButton button in section.Buttons)
        {
            buttons.Add(new()
            {
                Type = ElementType.Button,
                Id = ButtonId(page, button.DemoId),
                Text = button.Label,
                Enabled = !button.SendsToNetwork || demoWritesOpen,
                Tooltip = button.SendsToNetwork ? $"Sends {button.Sends}; needs Demo writes" : "Stays on this client",
                Style = new() { Variant = button.SendsToNetwork ? ButtonVariant.Danger : ButtonVariant.Default },
            });
        }
        return buttons;
    }
}
