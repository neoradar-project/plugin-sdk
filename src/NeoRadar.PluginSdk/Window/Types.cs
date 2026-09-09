using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Window;

public readonly record struct WindowHandle(ulong Value)
{
    public static WindowHandle Invalid => new(0UL);

    public bool IsValid => Value != 0UL;
}

public readonly record struct ElementHandle(ulong Value);

public enum ElementType
{
    Panel = 0,
    Label = 1,
    Button = 2,
    TextInput = 3,
    Checkbox = 4,
    Divider = 5,
    Spacer = 6,
    Group = 7,

    Heading = 10,
    Paragraph = 11,
    Select = 12,
    Toggle = 13,
    Radio = 14,
    Slider = 15,
    NumberInput = 16,
    ProgressBar = 17,
    Badge = 18,
    Tabs = 19,
    TabPage = 20,
    ScrollArea = 21,
    Grid = 22,
    Image = 23,

    /// <summary>Scrolling, virtualised rows of cells; the definition lives in
    /// <see cref="WindowElement.Table"/>.</summary>
    Table = 24,
}

public enum FontWeight : byte
{
    Default = 0,
    Thin = 1,
    Light = 2,
    Regular = 3,
    Medium = 4,
    SemiBold = 5,
    Bold = 6,
    ExtraBold = 7,
}

public enum HAlign : byte { Stretch = 0, Left = 1, Center = 2, Right = 3 }

public enum VAlign : byte { Stretch = 0, Top = 1, Center = 2, Bottom = 3 }

public enum ButtonVariant : byte
{
    Default = 0,
    Success = 1,
    Danger = 2,
    Accent = 3,
    Ghost = 4,
}

/// <summary>
/// A semantic slot the host resolves against the client theme, so a plugin follows a theme switch
/// without hard-coding RGB. An explicit BackgroundColor or TextColor still wins over the role.
/// </summary>
public enum ElementRole : byte
{
    /// <summary>No role; the element keeps the host's default brushes.</summary>
    Default = 0,

    /// <summary>Window and panel body.</summary>
    Surface = 1,

    /// <summary>A surface lifted off the body: cells, rows, buttons.</summary>
    SurfaceRaised = 2,

    /// <summary>De-emphasised text and containers.</summary>
    Muted = 3,

    /// <summary>The theme accent; colours text and gives containers a subtle background.</summary>
    Accent = 4,

    /// <summary>Success colouring; text takes the colour, containers a subtle background.</summary>
    Success = 5,

    /// <summary>Warning colouring; text takes the colour, containers a subtle background.</summary>
    Warning = 6,

    /// <summary>Danger colouring; text takes the colour, containers a subtle background.</summary>
    Danger = 7,
}

public sealed record ElementStyle
{
    public (byte R, byte G, byte B)? BackgroundColor { get; init; }
    public byte? BackgroundAlpha { get; init; }
    public (byte R, byte G, byte B)? TextColor { get; init; }

    public int? FontSize { get; init; }
    public FontWeight FontWeight { get; init; } = FontWeight.Default;

    public int? Width { get; init; }
    public int? Height { get; init; }
    public int? MinWidth { get; init; }
    public int? MinHeight { get; init; }
    public int? MaxWidth { get; init; }
    public int? MaxHeight { get; init; }

    public int? Padding { get; init; }
    public int? PaddingH { get; init; }
    public int? PaddingV { get; init; }
    public int? Margin { get; init; }
    public int? MarginH { get; init; }
    public int? MarginV { get; init; }
    public int? Spacing { get; init; }

    public (byte R, byte G, byte B)? BorderColor { get; init; }
    public byte? BorderWidth { get; init; }
    public byte? BorderRadius { get; init; }
    public byte? Opacity { get; init; }

    public string ThemeBg { get; init; } = string.Empty;
    public string ThemeFg { get; init; } = string.Empty;

    public ButtonVariant Variant { get; init; } = ButtonVariant.Default;

    /// <summary>Theme slot this element takes its colours from when no explicit colour is set.</summary>
    public ElementRole Role { get; init; } = ElementRole.Default;
}

/// <summary>What a column's cells render as.</summary>
public enum TableColumnKind : byte
{
    /// <summary>The cell text, shown as is.</summary>
    Text = 0,

    /// <summary>A check box; the cell text is <c>true</c> or <c>false</c>.</summary>
    Check = 1,

    /// <summary>A button; the cell text is its caption.</summary>
    Button = 2,
}

/// <summary>One column of an <see cref="ElementType.Table"/>.</summary>
/// <param name="Id">Stable identifier; a header click reports it as the <c>sort</c> value, and a
/// <c>toggle</c> or <c>cell</c> event names it. Ids must not contain <c>|</c>.</param>
/// <param name="Title">Header text.</param>
/// <param name="Width">Pixels ("120"), "auto", or star ("*", "2*"), the same grammar as Grid
/// tracks.</param>
/// <param name="Align">Horizontal alignment of the header and the column's cells.</param>
/// <param name="Kind">What the column's cells render as; <see cref="TableColumnKind.Text"/> by
/// default.</param>
public sealed record TableColumn(
    string Id,
    string Title,
    string Width = "*",
    HAlign Align = HAlign.Left,
    TableColumnKind Kind = TableColumnKind.Text);

/// <summary>One row of an <see cref="ElementType.Table"/>.</summary>
/// <param name="Id">Stable identifier reported as the <c>select</c> and <c>activate</c> value, and
/// named by a <c>toggle</c> or <c>cell</c> event. Ids must not contain <c>|</c>.</param>
/// <param name="Cells">Cell text, aligned with the columns by index; a missing cell renders empty.
/// A <see cref="TableColumnKind.Check"/> cell's text is <c>true</c> or <c>false</c>, a
/// <see cref="TableColumnKind.Button"/> cell's text is its caption, and a
/// <see cref="TableColumnKind.Text"/> cell is shown as is.</param>
/// <param name="Style">Background and foreground for the whole row; the host keeps its own
/// selection highlight on top.</param>
public sealed record TableRow(string Id, IReadOnlyList<string> Cells, ElementStyle? Style = null);

/// <summary>
/// The content of an <see cref="ElementType.Table"/> element. The table scrolls vertically inside
/// its own bounds and virtualises rows; Style.Height and Style.MaxHeight bound it.
/// </summary>
/// <param name="Columns">Column definitions, left to right.</param>
/// <param name="Rows">Rows in the order they are shown; the host never sorts them.</param>
/// <param name="SelectedIndex">Row index selected at build time; -1 for no selection.</param>
/// <param name="ShowHeader">False to drop the header strip.</param>
public sealed record TableDefinition(
    IReadOnlyList<TableColumn> Columns,
    IReadOnlyList<TableRow> Rows,
    int SelectedIndex = -1,
    bool ShowHeader = true);

public sealed record WindowElement
{
    public string Id { get; init; } = string.Empty;
    public ElementType Type { get; init; } = ElementType.Label;
    public string Text { get; init; } = string.Empty;
    public string Data { get; init; } = string.Empty;
    public ElementStyle Style { get; init; } = new();
    public IReadOnlyList<WindowElement> Children { get; init; } = System.Array.Empty<WindowElement>();

    /// <summary>0 = vertical, 1 = horizontal.</summary>
    public byte Direction { get; init; }
    public HAlign HAlign { get; init; } = HAlign.Stretch;
    public VAlign VAlign { get; init; } = VAlign.Stretch;
    public bool Wrap { get; init; }

    public int Col { get; init; }
    public int Row { get; init; }
    public int ColSpan { get; init; }
    public int RowSpan { get; init; }

    public string Columns { get; init; } = string.Empty;
    public string Rows { get; init; } = string.Empty;

    public bool Enabled { get; init; } = true;
    public bool Visible { get; init; } = true;
    public bool IsChecked { get; init; }
    public bool IsPassword { get; init; }

    /// <summary>
    /// True to make a Panel, Group, Grid, Label, Paragraph, Badge or Image raise a <c>click</c>
    /// event whose value is "left", "right" or "middle".
    /// </summary>
    public bool Clickable { get; init; }

    /// <summary>Choices for a Select or Radio element.</summary>
    public IReadOnlyList<string> Options { get; init; } = System.Array.Empty<string>();
    public int SelectedIndex { get; init; } = -1;

    /// <summary>Columns and rows of an <see cref="ElementType.Table"/>; ignored by every other
    /// element type.</summary>
    public TableDefinition? Table { get; init; }

    public double MinValue { get; init; }
    public double MaxValue { get; init; }
    public double Step { get; init; }
    public double Value { get; init; }
    public double Progress { get; init; }

    public string Tooltip { get; init; } = string.Empty;
}

/// <summary>A plugin window: its frame, its sizing rules and the element tree it opens with.</summary>
/// <param name="Title">Window title; the host keys the remembered position and size on it.</param>
/// <param name="Width">Width in pixels at first open.</param>
/// <param name="Height">Height in pixels at first open.</param>
/// <param name="Icon">Icon name shown in the title bar and the plugin list.</param>
/// <param name="ShowInList">True to list the window in the client's plugin list.</param>
/// <param name="Elements">Root elements; null opens an empty window.</param>
/// <param name="Resizable">True to let the controller resize the window; the size they leave it at
/// is remembered like its position.</param>
/// <param name="MinWidth">Smallest width a resize may reach; 0 for the host's own minimum.</param>
/// <param name="MinHeight">Smallest height a resize may reach; 0 for the host's own minimum.</param>
/// <param name="DefaultButtonId">Element id Enter raises <c>click</c> on, when the button is
/// enabled.</param>
/// <param name="CancelButtonId">Element id Escape raises <c>click</c> on, when the button is
/// enabled.</param>
public sealed record WindowDefinition(
    string Title,
    int Width = 300,
    int Height = 200,
    string Icon = "",
    bool ShowInList = false,
    IReadOnlyList<WindowElement>? Elements = null,
    bool Resizable = false,
    int MinWidth = 0,
    int MinHeight = 0,
    string DefaultButtonId = "",
    string CancelButtonId = "");

/// <summary>
/// One interaction with a plugin window, delivered to <see cref="BasePlugin.OnWindowEvent"/>.
/// </summary>
/// <param name="Window">Handle of the window the event came from.</param>
/// <param name="ElementId">Id of the element that raised it; empty for <c>close</c>.</param>
/// <param name="EventType">One of <c>click</c> (buttons and clickable elements), <c>change</c>
/// (inputs, selects, checkboxes, toggles, radios, sliders), <c>select</c>, <c>activate</c>,
/// <c>sort</c>, <c>toggle</c> and <c>cell</c> (tables), or <c>close</c> (the window closed).</param>
/// <param name="Value">The button name or mouse button for <c>click</c>, the new content for
/// <c>change</c>, the row id for <c>select</c> and <c>activate</c>, the column id for <c>sort</c>,
/// <c>rowId|columnId|true</c> or <c>rowId|columnId|false</c> for <c>toggle</c>,
/// <c>rowId|columnId</c> for <c>cell</c>, empty for <c>close</c>. <c>|</c> is therefore reserved in
/// row and column ids.</param>
public sealed record WindowEvent(
    WindowHandle Window,
    string ElementId,
    string EventType,
    string Value);
