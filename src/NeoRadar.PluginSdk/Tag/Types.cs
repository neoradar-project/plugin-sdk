using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Tag;

/// <summary>The kind of value a tag item resolves to, so hosts can offer a typed editor / sample input.</summary>
public enum TagValueType
{
    String,
    Int,
    Double,
    Bool,
    Enum,
}

public sealed record TagItemDefinition(
    string Name,
    string DefaultValue,
    IReadOnlyList<string> AllowedActions)
{
    // Optional presentation metadata. Defaulted so existing 3-arg construction stays source- and
    // binary-compatible; the host falls back to Name when Title is empty.
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TagValueType ValueType { get; init; } = TagValueType.String;
}

public sealed record TagActionDefinition(string Name, bool RequiresInput, string Description)
{
    public string Title { get; init; } = string.Empty;
}

public sealed record TagContext(
    string Callsign,
    string ListId,
    (byte R, byte G, byte B)? Colour = null,
    (byte R, byte G, byte B)? BackgroundColour = null,
    IReadOnlyDictionary<string, string>? Data = null);

public sealed record TagActionEvent(
    string ActionId,
    string TagId,
    string Callsign,
    int Button,
    string? UserInput,
    IReadOnlyDictionary<string, string> Data);

public enum DropdownComponentType
{
    Button,
    Divider,
    Label,
    Toggle,
    Radio,
    Group,
    ScrollArea,
    InputArea,
}

public enum DropdownAlignmentType { Center, Right, Left }

public sealed record DropdownComponentStyle(
    (byte R, byte G, byte B)? BackgroundColor = null,
    (byte R, byte G, byte B)? TextColor = null,
    int? FontSize = null,
    string? FontWeight = null,
    int? Width = null,
    int? Height = null,
    bool? Border = null,
    string? ClassName = null,
    DropdownAlignmentType? TextAlign = null);

public sealed record DropdownComponent
{
    public string Id { get; init; } = string.Empty;
    public DropdownComponentType Type { get; init; }
    public string Text { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> Data { get; init; } = new Dictionary<string, string>();
    public DropdownComponentStyle Style { get; init; } = new();
    public bool RequiresInput { get; init; }
    public IReadOnlyList<DropdownComponent> Children { get; init; } = System.Array.Empty<DropdownComponent>();
}

public sealed record DropdownDefinition(
    string Title,
    int Width,
    int MaxHeight,
    IReadOnlyList<DropdownComponent> Components);

public sealed record DropdownActionEvent(
    string ActionId,
    string ComponentId,
    string TagId,
    string Callsign,
    string? UserInput,
    IReadOnlyDictionary<string, string> Data);
