using System.Collections.Generic;
using NeoRadar.PluginSdk.Window;

namespace NeoRadar.SamplePlugin.DemoWindow;

/// <summary>The demo form: a heading, a badge, a form, a sortable table and a progress bar, all from window elements.</summary>
internal static class DemoWindowLayout
{
    public const string Title = "Sample Form";
    public const string HiddenNote = "hidden-note";
    public const string Callsign = "callsign";
    public const string Urgent = "urgent";
    public const string Save = "save";
    public const string Close = "close";
    public const string Hint = "hint";
    public const string Traffic = "traffic";
    public const string Progress = "progress";
    public const string Status = "status";

    private static readonly TableColumn[] s_columns =
    [
        new("callsign", "Callsign", "auto"),
        new("type", "Type", "70"),
        new("state", "State", "*", HAlign.Right),
    ];

    public static WindowDefinition Build() => new(
        Title: Title,
        Width: 380,
        Height: 520,
        Icon: "Settings",
        ShowInList: true,
        Elements:
        [
            new() { Type = ElementType.Heading, Text = "A form built from window elements" },
            new() { Type = ElementType.Badge, Text = "DEMO", Style = new() { Role = ElementRole.Accent } },
            new() { Id = HiddenNote, Type = ElementType.Label, Text = "Close this window, then reopen it from the menu." },
            new()
            {
                Type = ElementType.Panel,
                Style = new() { BackgroundColor = (235, 235, 235), Padding = 6 },
                Children = [new() { Type = ElementType.Label, Text = "Explicit black text on a light panel", Style = new() { TextColor = (0, 0, 0) } }],
            },
            new() { Type = ElementType.Divider },
            new()
            {
                Type = ElementType.Panel,
                Id = "banner",
                Style = new() { Role = ElementRole.Accent, Padding = 6, BorderWidth = 1 },
                Children = [new() { Type = ElementType.Label, Id = Hint, Text = "Click this line for a mouse-button event.", Clickable = true }],
            },
            new() { Type = ElementType.TextInput, Id = Callsign, Text = "BAW123", Data = "Callsign", Style = new() { Width = 200 } },
            new() { Type = ElementType.Checkbox, Id = Urgent, Text = "Urgent" },
            new() { Type = ElementType.Table, Id = Traffic, Style = new() { MaxHeight = 90 }, Table = new TableDefinition(s_columns, Rows(byState: false)) },
            new()
            {
                Type = ElementType.Panel,
                Direction = 1,
                Style = new() { Spacing = 8 },
                Children =
                [
                    new() { Type = ElementType.Button, Id = Save, Text = "Save", Style = new() { Variant = ButtonVariant.Success, Width = 80 } },
                    new() { Type = ElementType.Button, Id = Close, Text = "Close", Style = new() { Variant = ButtonVariant.Ghost, Width = 80 } },
                ],
            },
            new() { Type = ElementType.ProgressBar, Id = Progress, Progress = 0 },
            new() { Type = ElementType.Label, Id = Status, Style = new() { FontSize = 11, Role = ElementRole.Muted } },
        ],
        Resizable: true,
        MinWidth: 300,
        MinHeight: 360,
        DefaultButtonId: Save,
        CancelButtonId: Close);

    /// <summary>The host never sorts a table; the plugin sends the rows back in the order it wants.</summary>
    public static IReadOnlyList<TableRow> Rows(bool byState)
    {
        List<TableRow> rows =
        [
            new("BAW123", ["BAW123", "A320", "Cruise"]),
            new("EZY44M", ["EZY44M", "A319", "Descent"], new ElementStyle { Role = ElementRole.Warning }),
            new("RYR8PT", ["RYR8PT", "B738", "Climb"]),
        ];
        if (byState) rows.Sort((left, right) => string.CompareOrdinal(left.Cells[2], right.Cells[2]));
        return rows;
    }
}
