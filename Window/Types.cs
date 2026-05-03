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
}

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

    public string Options { get; init; } = string.Empty;
    public int SelectedIndex { get; init; } = -1;

    public double MinValue { get; init; }
    public double MaxValue { get; init; }
    public double Step { get; init; }
    public double Value { get; init; }
    public double Progress { get; init; }

    public string Tooltip { get; init; } = string.Empty;
}

public sealed record WindowDefinition(
    string Title,
    int Width = 300,
    int Height = 200,
    string Icon = "",
    bool ShowInList = false,
    IReadOnlyList<WindowElement>? Elements = null);

public sealed record WindowEvent(
    WindowHandle Window,
    string ElementId,
    string EventType,
    string Value);
