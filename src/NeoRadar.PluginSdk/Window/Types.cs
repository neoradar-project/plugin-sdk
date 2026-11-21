using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Window;

/// <summary>
/// Represents the handle of an open plugin window.
/// </summary>
public readonly record struct WindowHandle(ulong Value)
{
    /// <summary>
    /// Gets the handle that refers to no window.
    /// </summary>
    public static WindowHandle Invalid => new(0UL);

    /// <summary>
    /// Gets a value indicating whether the handle refers to a window.
    /// </summary>
    public bool IsValid => Value != 0UL;
}

/// <summary>
/// Represents the handle of a window element.
/// </summary>
public readonly record struct ElementHandle(ulong Value);

/// <summary>
/// Specifies the kind of control a window element renders as.
/// </summary>
public enum ElementType
{
    /// <summary>
    /// A container that stacks its children and can carry a background and border.
    /// </summary>
    Panel = 0,

    /// <summary>
    /// A single line of text.
    /// </summary>
    Label = 1,

    /// <summary>
    /// A push button that raises <c>click</c> with its <see cref="WindowElement.Data"/>.
    /// </summary>
    Button = 2,

    /// <summary>
    /// A single-line text box that raises <c>change</c>.
    /// </summary>
    TextInput = 3,

    /// <summary>
    /// A check box that raises <c>change</c>.
    /// </summary>
    Checkbox = 4,

    /// <summary>
    /// A horizontal rule.
    /// </summary>
    Divider = 5,

    /// <summary>
    /// Empty space.
    /// </summary>
    Spacer = 6,

    /// <summary>
    /// A container with an optional caption above its children.
    /// </summary>
    Group = 7,

    /// <summary>
    /// A heading.
    /// </summary>
    Heading = 10,

    /// <summary>
    /// Wrapped body text.
    /// </summary>
    Paragraph = 11,

    /// <summary>
    /// A drop-down list of <see cref="WindowElement.Options"/>.
    /// </summary>
    Select = 12,

    /// <summary>
    /// An on and off switch that raises <c>change</c>.
    /// </summary>
    Toggle = 13,

    /// <summary>
    /// A radio button grouped with others that share its <see cref="WindowElement.Data"/>.
    /// </summary>
    Radio = 14,

    /// <summary>
    /// A slider between <see cref="WindowElement.MinValue"/> and <see cref="WindowElement.MaxValue"/>.
    /// </summary>
    Slider = 15,

    /// <summary>
    /// A numeric up-down box between <see cref="WindowElement.MinValue"/> and <see cref="WindowElement.MaxValue"/>.
    /// </summary>
    NumberInput = 16,

    /// <summary>
    /// A progress bar showing <see cref="WindowElement.Progress"/>.
    /// </summary>
    ProgressBar = 17,

    /// <summary>
    /// A small rounded text badge.
    /// </summary>
    Badge = 18,

    /// <summary>
    /// A tab strip whose children are <see cref="TabPage"/> elements.
    /// </summary>
    Tabs = 19,

    /// <summary>
    /// One page of a <see cref="Tabs"/> element.
    /// </summary>
    TabPage = 20,

    /// <summary>
    /// A container that scrolls its children.
    /// </summary>
    ScrollArea = 21,

    /// <summary>
    /// A container that places its children in rows and columns.
    /// </summary>
    Grid = 22,

    /// <summary>
    /// An image loaded from the file path in <see cref="WindowElement.Data"/>.
    /// </summary>
    Image = 23,

    /// <summary>
    /// A scrolling, virtualised table defined by <see cref="WindowElement.Table"/>.
    /// </summary>
    Table = 24,
}

/// <summary>
/// Specifies the weight of an element's font.
/// </summary>
public enum FontWeight : byte
{
    /// <summary>
    /// The host's default weight for the element.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Thin.
    /// </summary>
    Thin = 1,

    /// <summary>
    /// Light.
    /// </summary>
    Light = 2,

    /// <summary>
    /// Regular.
    /// </summary>
    Regular = 3,

    /// <summary>
    /// Medium.
    /// </summary>
    Medium = 4,

    /// <summary>
    /// Semi-bold.
    /// </summary>
    SemiBold = 5,

    /// <summary>
    /// Bold.
    /// </summary>
    Bold = 6,

    /// <summary>
    /// Extra bold.
    /// </summary>
    ExtraBold = 7,
}

/// <summary>
/// Specifies the horizontal alignment of an element.
/// </summary>
public enum HAlign : byte { Stretch = 0, Left = 1, Center = 2, Right = 3 }

/// <summary>
/// Specifies the vertical alignment of an element.
/// </summary>
public enum VAlign : byte { Stretch = 0, Top = 1, Center = 2, Bottom = 3 }

/// <summary>
/// Specifies the visual variant of a button.
/// </summary>
public enum ButtonVariant : byte
{
    /// <summary>
    /// The standard button.
    /// </summary>
    Default = 0,

    /// <summary>
    /// A button coloured for a positive action.
    /// </summary>
    Success = 1,

    /// <summary>
    /// A button coloured for a destructive action.
    /// </summary>
    Danger = 2,

    /// <summary>
    /// A button in the theme accent colour.
    /// </summary>
    Accent = 3,

    /// <summary>
    /// A button with a transparent background.
    /// </summary>
    Ghost = 4,
}

/// <summary>
/// Specifies a theme colour slot so an element follows a theme switch without hard-coded colours.
/// </summary>
/// <remarks>An explicit background or text colour on the style takes precedence over the role.</remarks>
public enum ElementRole : byte
{
    /// <summary>
    /// No role, so the element keeps the host's default colours.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The window and panel body.
    /// </summary>
    Surface = 1,

    /// <summary>
    /// A surface raised above the body, such as a cell, row or button.
    /// </summary>
    SurfaceRaised = 2,

    /// <summary>
    /// De-emphasised text and containers.
    /// </summary>
    Muted = 3,

    /// <summary>
    /// The theme accent, which colours text and gives containers a subtle background.
    /// </summary>
    Accent = 4,

    /// <summary>
    /// Success colouring, which colours text and gives containers a subtle background.
    /// </summary>
    Success = 5,

    /// <summary>
    /// Warning colouring, which colours text and gives containers a subtle background.
    /// </summary>
    Warning = 6,

    /// <summary>
    /// Danger colouring, which colours text and gives containers a subtle background.
    /// </summary>
    Danger = 7,
}

/// <summary>
/// Represents the visual style of a window element; an unset field takes the host default.
/// </summary>
public sealed record ElementStyle
{
    /// <summary>
    /// Gets the background colour, or <c>null</c> for the role or theme default.
    /// </summary>
    public (byte R, byte G, byte B)? BackgroundColor { get; init; }

    /// <summary>
    /// Gets the background alpha from 0 to 255, or <c>null</c> for opaque.
    /// </summary>
    public byte? BackgroundAlpha { get; init; }

    /// <summary>
    /// Gets the text colour, or <c>null</c> for the role or theme default.
    /// </summary>
    public (byte R, byte G, byte B)? TextColor { get; init; }

    /// <summary>
    /// Gets the font size in pixels, or <c>null</c> for the host default.
    /// </summary>
    public int? FontSize { get; init; }

    /// <summary>
    /// Gets the font weight.
    /// </summary>
    public FontWeight FontWeight { get; init; } = FontWeight.Default;

    /// <summary>
    /// Gets the fixed width in pixels.
    /// </summary>
    public int? Width { get; init; }

    /// <summary>
    /// Gets the fixed height in pixels.
    /// </summary>
    public int? Height { get; init; }

    /// <summary>
    /// Gets the minimum width in pixels.
    /// </summary>
    public int? MinWidth { get; init; }

    /// <summary>
    /// Gets the minimum height in pixels.
    /// </summary>
    public int? MinHeight { get; init; }

    /// <summary>
    /// Gets the maximum width in pixels.
    /// </summary>
    public int? MaxWidth { get; init; }

    /// <summary>
    /// Gets the maximum height in pixels.
    /// </summary>
    public int? MaxHeight { get; init; }

    /// <summary>
    /// Gets the padding on every side in pixels.
    /// </summary>
    public int? Padding { get; init; }

    /// <summary>
    /// Gets the left and right padding in pixels, which overrides <see cref="Padding"/>.
    /// </summary>
    public int? PaddingH { get; init; }

    /// <summary>
    /// Gets the top and bottom padding in pixels, which overrides <see cref="Padding"/>.
    /// </summary>
    public int? PaddingV { get; init; }

    /// <summary>
    /// Gets the margin on every side in pixels.
    /// </summary>
    public int? Margin { get; init; }

    /// <summary>
    /// Gets the left and right margin in pixels, which overrides <see cref="Margin"/>.
    /// </summary>
    public int? MarginH { get; init; }

    /// <summary>
    /// Gets the top and bottom margin in pixels, which overrides <see cref="Margin"/>.
    /// </summary>
    public int? MarginV { get; init; }

    /// <summary>
    /// Gets the space between a container's children in pixels.
    /// </summary>
    public int? Spacing { get; init; }

    /// <summary>
    /// Gets the border colour.
    /// </summary>
    public (byte R, byte G, byte B)? BorderColor { get; init; }

    /// <summary>
    /// Gets the border width in pixels.
    /// </summary>
    public byte? BorderWidth { get; init; }

    /// <summary>
    /// Gets the corner radius in pixels.
    /// </summary>
    public byte? BorderRadius { get; init; }

    /// <summary>
    /// Gets the opacity from 1 to 255, where 0 or <c>null</c> means fully opaque.
    /// </summary>
    public byte? Opacity { get; init; }

    /// <summary>
    /// Gets the theme brush key for the background, which overrides the colour and role when set.
    /// </summary>
    public string ThemeBg { get; init; } = string.Empty;

    /// <summary>
    /// Gets the theme brush key for the text, which overrides the colour and role when set.
    /// </summary>
    public string ThemeFg { get; init; } = string.Empty;

    /// <summary>
    /// Gets the variant of a button.
    /// </summary>
    public ButtonVariant Variant { get; init; } = ButtonVariant.Default;

    /// <summary>
    /// Gets the theme slot the element takes its colours from when no explicit colour is set.
    /// </summary>
    public ElementRole Role { get; init; } = ElementRole.Default;
}

/// <summary>
/// Specifies what the cells of a table column render as.
/// </summary>
public enum TableColumnKind : byte
{
    /// <summary>
    /// The cell text, shown as it is.
    /// </summary>
    Text = 0,

    /// <summary>
    /// A check box whose cell text is <c>true</c> or <c>false</c>.
    /// </summary>
    Check = 1,

    /// <summary>
    /// A button whose cell text is its caption.
    /// </summary>
    Button = 2,
}

/// <summary>
/// Represents one column of an <see cref="ElementType.Table"/>.
/// </summary>
/// <param name="Id">The column id reported by <c>sort</c>, <c>toggle</c> and <c>cell</c>; it must not contain <c>|</c>.</param>
/// <param name="Title">The header text.</param>
/// <param name="Width">Pixels (<c>120</c>), <c>auto</c>, or a star size (<c>*</c>, <c>2*</c>).</param>
/// <param name="Align">The horizontal alignment of the header and cells.</param>
/// <param name="Kind">What the cells render as.</param>
public sealed record TableColumn(
    string Id,
    string Title,
    string Width = "*",
    HAlign Align = HAlign.Left,
    TableColumnKind Kind = TableColumnKind.Text);

/// <summary>
/// Represents one row of an <see cref="ElementType.Table"/>.
/// </summary>
/// <param name="Id">The row id reported by <c>select</c>, <c>activate</c>, <c>toggle</c> and <c>cell</c>; it must not contain <c>|</c>.</param>
/// <param name="Cells">The cell text by column index; a missing cell renders empty.</param>
/// <param name="Style">The row's colours; the host's selection highlight is drawn over them.</param>
public sealed record TableRow(string Id, IReadOnlyList<string> Cells, ElementStyle? Style = null);

/// <summary>
/// Represents the content of an <see cref="ElementType.Table"/>, which scrolls and virtualises its rows.
/// </summary>
/// <param name="Columns">The columns from left to right.</param>
/// <param name="Rows">The rows in display order; the host never sorts them.</param>
/// <param name="SelectedIndex">The row selected when built, or -1 for none.</param>
/// <param name="ShowHeader">Whether the header strip is shown.</param>
/// <remarks>The element's style <c>Height</c> and <c>MaxHeight</c> bound the table.</remarks>
public sealed record TableDefinition(
    IReadOnlyList<TableColumn> Columns,
    IReadOnlyList<TableRow> Rows,
    int SelectedIndex = -1,
    bool ShowHeader = true);

/// <summary>
/// Represents one element of a plugin window and its children.
/// </summary>
public sealed record WindowElement
{
    /// <summary>
    /// Gets the id events report and update methods address.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the kind of control the element renders as.
    /// </summary>
    public ElementType Type { get; init; } = ElementType.Label;

    /// <summary>
    /// Gets the element's text, caption or initial input value.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Gets the per-type extra: a button's click value, an input's placeholder, a radio group or an image path.
    /// </summary>
    public string Data { get; init; } = string.Empty;

    /// <summary>
    /// Gets the element's style.
    /// </summary>
    public ElementStyle Style { get; init; } = new();

    /// <summary>
    /// Gets the child elements of a container.
    /// </summary>
    public IReadOnlyList<WindowElement> Children { get; init; } = System.Array.Empty<WindowElement>();

    /// <summary>
    /// Gets the layout direction of a container: 0 for vertical, 1 for horizontal.
    /// </summary>
    public byte Direction { get; init; }

    /// <summary>
    /// Gets the horizontal alignment.
    /// </summary>
    public HAlign HAlign { get; init; } = HAlign.Stretch;

    /// <summary>
    /// Gets the vertical alignment.
    /// </summary>
    public VAlign VAlign { get; init; } = VAlign.Stretch;

    /// <summary>
    /// Gets a value indicating whether a horizontal container wraps its children onto new lines.
    /// </summary>
    public bool Wrap { get; init; }

    /// <summary>
    /// Gets the column of the element within a parent grid.
    /// </summary>
    public int Col { get; init; }

    /// <summary>
    /// Gets the row of the element within a parent grid.
    /// </summary>
    public int Row { get; init; }

    /// <summary>
    /// Gets the number of grid columns the element spans.
    /// </summary>
    public int ColSpan { get; init; }

    /// <summary>
    /// Gets the number of grid rows the element spans.
    /// </summary>
    public int RowSpan { get; init; }

    /// <summary>
    /// Gets the column tracks of a grid: a count, or a comma list of pixels, <c>auto</c>, <c>*</c> or <c>Nfr</c>.
    /// </summary>
    public string Columns { get; init; } = string.Empty;

    /// <summary>
    /// Gets the row tracks of a grid: a count, or a comma list of pixels, <c>auto</c>, <c>*</c> or <c>Nfr</c>.
    /// </summary>
    public string Rows { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the element accepts input.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the element is shown.
    /// </summary>
    public bool Visible { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether a Checkbox, Toggle or Radio starts checked.
    /// </summary>
    public bool IsChecked { get; init; }

    /// <summary>
    /// Gets a value indicating whether a TextInput masks its text.
    /// </summary>
    public bool IsPassword { get; init; }

    /// <summary>
    /// Gets a value indicating whether a non-button element raises <c>click</c> with the mouse button pressed.
    /// </summary>
    /// <remarks>Applies to Panel, Group, Grid, Label, Paragraph, Badge and Image; the value is left, right or middle.</remarks>
    public bool Clickable { get; init; }

    /// <summary>
    /// Gets the choices of a Select or Radio element.
    /// </summary>
    public IReadOnlyList<string> Options { get; init; } = System.Array.Empty<string>();

    /// <summary>
    /// Gets the initially selected option index, or -1 for none.
    /// </summary>
    public int SelectedIndex { get; init; } = -1;

    /// <summary>
    /// Gets the content of a Table element, ignored by every other type.
    /// </summary>
    public TableDefinition? Table { get; init; }

    /// <summary>
    /// Gets the minimum value of a Slider or NumberInput.
    /// </summary>
    public double MinValue { get; init; }

    /// <summary>
    /// Gets the maximum value of a Slider or NumberInput.
    /// </summary>
    public double MaxValue { get; init; }

    /// <summary>
    /// Gets the increment of a Slider or NumberInput, where 0 means 1.
    /// </summary>
    public double Step { get; init; }

    /// <summary>
    /// Gets the initial value of a Slider or NumberInput.
    /// </summary>
    public double Value { get; init; }

    /// <summary>
    /// Gets the fill of a ProgressBar from 0 to 1, where a negative value shows it as indeterminate.
    /// </summary>
    public double Progress { get; init; }

    /// <summary>
    /// Gets the tooltip text, or empty for none.
    /// </summary>
    public string Tooltip { get; init; } = string.Empty;
}

/// <summary>
/// Represents a plugin window: its frame, sizing rules and initial elements.
/// </summary>
/// <param name="Title">The window title, which also keys the remembered position and size.</param>
/// <param name="Width">The width in pixels at first open.</param>
/// <param name="Height">The height in pixels at first open.</param>
/// <param name="Icon">A Lucide icon name, shown before the entry in the Insets &amp; Plugin Windows menu; an unknown name shows none.</param>
/// <param name="ShowInList">Whether the window is listed in the client's plugin list.</param>
/// <param name="Elements">The root elements, or <c>null</c> for an empty window.</param>
/// <param name="Resizable">Whether the controller can resize the window; the size is remembered like the position.</param>
/// <param name="MinWidth">The smallest width a resize may reach, or 0 for the host minimum.</param>
/// <param name="MinHeight">The smallest height a resize may reach, or 0 for the host minimum.</param>
/// <param name="DefaultButtonId">The id of the enabled button Enter clicks.</param>
/// <param name="CancelButtonId">The id of the enabled button Escape clicks.</param>
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
/// Represents one interaction with a plugin window, delivered to <see cref="BasePlugin.OnWindowEvent"/>.
/// </summary>
/// <param name="Window">The window the event came from.</param>
/// <param name="ElementId">The element that raised it, or empty for <c>close</c>.</param>
/// <param name="EventType">One of <c>click</c>, <c>change</c>, <c>select</c>, <c>activate</c>, <c>sort</c>, <c>toggle</c>, <c>cell</c> or <c>close</c>.</param>
/// <param name="Value">The click value, new content, row id (select, activate), column id (sort), <c>row|col|bool</c> (toggle), <c>row|col</c> (cell) or empty.</param>
public sealed record WindowEvent(
    WindowHandle Window,
    string ElementId,
    string EventType,
    string Value);
