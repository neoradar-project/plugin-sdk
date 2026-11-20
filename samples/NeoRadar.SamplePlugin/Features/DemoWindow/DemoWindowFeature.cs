using System.Collections.Generic;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.Features.DemoWindow;

internal sealed class DemoWindowFeature : IFeature
{
    private static readonly TableColumn[] s_columns =
    [
        new("callsign", "Callsign", "auto"),
        new("type", "Type", "70"),
        new("state", "State", "*", HAlign.Right),
    ];

    private PluginContext? _context;
    private WindowHandle _window;

    public void Initialize(PluginContext context)
    {
        _context = context;
        _window = context.Window.OpenWindow(new WindowDefinition(
            Title: "Sample Plugin",
            Width: 360,
            Height: 320,
            Icon: "Settings",
            ShowInList: true,
            Elements: new WindowElement[]
            {
                new()
                {
                    Type = ElementType.Heading,
                    Text = "Hello from SamplePlugin",
                    Style = new() { FontSize = 14 },
                },
                new()
                {
                    Type = ElementType.Panel,
                    Id = "banner",
                    Style = new() { Role = ElementRole.Accent, Padding = 6, BorderWidth = 1 },
                    Children = new WindowElement[]
                    {
                        new()
                        {
                            Type = ElementType.Label,
                            Id = "hint",
                            Text = "Click this line for a mouse-button event.",
                            Clickable = true,
                        },
                    },
                },
                new()
                {
                    Type = ElementType.TextInput,
                    Id = "callsign",
                    Data = "BAW123",
                    Style = new() { Width = 200 },
                },
                new()
                {
                    Type = ElementType.Table,
                    Id = "traffic",
                    Style = new() { MaxHeight = 90 },
                    Table = new TableDefinition(
                        Columns: s_columns,
                        Rows: BuildRows(),
                        SelectedIndex: -1),
                },
                new()
                {
                    Type = ElementType.Button,
                    Id = "save",
                    Text = "Save",
                    Style = new() { Variant = ButtonVariant.Success, Width = 80 },
                },
                new()
                {
                    Type = ElementType.Button,
                    Id = "close",
                    Text = "Close",
                    Style = new() { Variant = ButtonVariant.Ghost, Width = 80 },
                },
                new()
                {
                    Type = ElementType.ProgressBar,
                    Id = "progress",
                    Progress = 0,
                },
                new()
                {
                    Type = ElementType.Label,
                    Id = "status",
                    Text = "",
                    Style = new() { FontSize = 11, Role = ElementRole.Muted },
                },
            },
            Resizable: true,
            MinWidth: 300,
            MinHeight: 240,
            DefaultButtonId: "save",
            CancelButtonId: "close"));
    }

    public void Shutdown()
    {
        if (_window.IsValid)
            _context?.Window.CloseWindow(_window);
        _window = WindowHandle.Invalid;
    }

    public void OnWindowEvent(WindowEvent evt)
    {
        IWindowInterface? win = _context?.Window;
        if (win is null || !_window.IsValid) return;

        switch (evt.EventType)
        {
            case "click" when evt.ElementId == "save":
                win.UpdateElement(_window, "progress", "100");
                win.UpdateElement(_window, "status", "Saved");
                break;
            case "click" when evt.ElementId == "close":
                win.CloseWindow(_window);
                _window = WindowHandle.Invalid;
                break;
            case "click" when evt.ElementId == "hint":
                win.UpdateElement(_window, "status", $"Clicked with the {evt.Value} button");
                break;
            case "change" when evt.ElementId == "callsign":
                win.UpdateElement(_window, "status", $"Editing: {evt.Value}");
                break;
            case "select" when evt.ElementId == "traffic":
                win.UpdateElement(_window, "status",
                    evt.Value.Length == 0 ? "Nothing selected" : $"Selected {evt.Value}");
                break;
            case "activate" when evt.ElementId == "traffic":
                win.UpdateElement(_window, "status", $"Opened {evt.Value}");
                break;
            case "sort" when evt.ElementId == "traffic":
                // The host never sorts; the plugin sends the order it wants back.
                win.SetTableRows(_window, "traffic", BuildRows(evt.Value == "state"), -1);
                break;
        }
    }

    private static IReadOnlyList<TableRow> BuildRows(bool byState = false)
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
