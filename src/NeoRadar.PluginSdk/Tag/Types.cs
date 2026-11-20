using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Tag;

/// <summary>
/// Specifies the kind of value a tag item resolves to, so hosts can offer a typed editor.
/// </summary>
public enum TagValueType
{
    /// <summary>
    /// A text value.
    /// </summary>
    String,

    /// <summary>
    /// A whole number.
    /// </summary>
    Int,

    /// <summary>
    /// A decimal number.
    /// </summary>
    Double,

    /// <summary>
    /// A true or false value.
    /// </summary>
    Bool,

    /// <summary>
    /// One of a fixed set of values.
    /// </summary>
    Enum,
}

/// <summary>
/// Represents a tag item a plugin registers for packages to place in labels.
/// </summary>
/// <param name="Name">The identifier within the plugin that forms the tag id.</param>
/// <param name="DefaultValue">The value shown for an aircraft the plugin has pushed nothing for.</param>
/// <param name="AllowedActions">The tag actions a package may bind to this item.</param>
public sealed record TagItemDefinition(
    string Name,
    string DefaultValue,
    IReadOnlyList<string> AllowedActions)
{
    /// <summary>
    /// Gets the display title, or empty to show <see cref="Name"/>.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description shown to a package author.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the kind of value the item resolves to.
    /// </summary>
    public TagValueType ValueType { get; init; } = TagValueType.String;
}

/// <summary>
/// Represents a tag action a plugin registers for packages to bind to clicks.
/// </summary>
/// <param name="Name">The identifier within the plugin that forms the action id.</param>
/// <param name="RequiresInput">True if the action asks the controller for text before it runs.</param>
/// <param name="Description">The description shown to a package author.</param>
public sealed record TagActionDefinition(string Name, bool RequiresInput, string Description)
{
    /// <summary>
    /// Gets the display title, or empty to show <see cref="Name"/>.
    /// </summary>
    public string Title { get; init; } = string.Empty;
}

/// <summary>
/// Represents the aircraft and styling a tag value is pushed for.
/// </summary>
/// <param name="Callsign">The callsign of the aircraft the value is for.</param>
/// <param name="ListId">The identifier of the list the value is for.</param>
/// <param name="Colour">The text colour, or null to keep the label's own.</param>
/// <param name="BackgroundColour">The background colour, or null to keep the label's own.</param>
/// <param name="Data">Extra values passed with the update, or null for none.</param>
public sealed record TagContext(
    string Callsign,
    string ListId,
    (byte R, byte G, byte B)? Colour = null,
    (byte R, byte G, byte B)? BackgroundColour = null,
    IReadOnlyDictionary<string, string>? Data = null);

/// <summary>
/// Represents a plugin tag action run on a radar track.
/// </summary>
/// <param name="ActionId">The id of the action that ran.</param>
/// <param name="TagId">The id of the tag item that was clicked.</param>
/// <param name="Callsign">The callsign of the radar track the action was run on.</param>
/// <param name="Button">The mouse button, 0 left, 1 middle, 2 right or 3 double click, and 0 for submitted input.</param>
/// <param name="UserInput">The text the controller entered, or null when the action asks for none.</param>
/// <param name="Data">Extra values passed with the action.</param>
public sealed record TagActionEvent(
    string ActionId,
    string TagId,
    string Callsign,
    int Button,
    string? UserInput,
    IReadOnlyDictionary<string, string> Data)
{
    /// <summary>
    /// Gets the callsign of the flight plan the track stands for, which may be one correlated by hand.
    /// </summary>
    /// <remarks>Empty for a track whose own plan another track holds by hand.</remarks>
    public string PlanCallsign { get; init; } = "";
}

/// <summary>
/// Represents a plugin dropdown about to open on a radar track, which the owning plugin may veto.
/// </summary>
/// <remarks>Fields added later arrive as optional init properties.</remarks>
public sealed record TagShowDropdownEvent
{
    /// <summary>
    /// Gets the action whose dropdown is opening.
    /// </summary>
    public required string ActionId { get; init; }

    /// <summary>
    /// Gets the callsign of the radar track the dropdown was opened on.
    /// </summary>
    public required string Callsign { get; init; }

    /// <summary>
    /// Gets the callsign of the flight plan the track stands for, which may be one correlated by hand.
    /// </summary>
    /// <remarks>Empty for a track whose own plan another track holds by hand.</remarks>
    public required string PlanCallsign { get; init; }
}

/// <summary>
/// Specifies the kind of a dropdown component.
/// </summary>
public enum DropdownComponentType
{
    /// <summary>
    /// A pressable button.
    /// </summary>
    Button,

    /// <summary>
    /// A separator line.
    /// </summary>
    Divider,

    /// <summary>
    /// Static text.
    /// </summary>
    Label,

    /// <summary>
    /// A checkbox that can be switched on and off.
    /// </summary>
    Toggle,

    /// <summary>
    /// One choice of a group of exclusive options.
    /// </summary>
    Radio,

    /// <summary>
    /// A container for child components.
    /// </summary>
    Group,

    /// <summary>
    /// A scrollable container for child components.
    /// </summary>
    ScrollArea,

    /// <summary>
    /// A text input box.
    /// </summary>
    InputArea,
}

/// <summary>
/// Specifies the horizontal alignment of dropdown text.
/// </summary>
public enum DropdownAlignmentType
{
    /// <summary>
    /// Centred text.
    /// </summary>
    Center,

    /// <summary>
    /// Right-aligned text.
    /// </summary>
    Right,

    /// <summary>
    /// Left-aligned text.
    /// </summary>
    Left
}

/// <summary>
/// Represents optional styling for a dropdown component, where null keeps the host's default.
/// </summary>
/// <param name="BackgroundColor">The background colour.</param>
/// <param name="TextColor">The text colour.</param>
/// <param name="FontSize">The font size.</param>
/// <param name="FontWeight">The font weight.</param>
/// <param name="Width">The component width.</param>
/// <param name="Height">The component height.</param>
/// <param name="Border">True to draw a border.</param>
/// <param name="ClassName">A style class name for the host.</param>
/// <param name="TextAlign">The horizontal text alignment.</param>
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

/// <summary>
/// Represents one component of a plugin dropdown.
/// </summary>
public sealed record DropdownComponent
{
    /// <summary>
    /// Gets the identifier reported when the component is pressed.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the kind of component.
    /// </summary>
    public DropdownComponentType Type { get; init; }

    /// <summary>
    /// Gets the text the component shows.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Gets the values passed back to the plugin when the component is pressed.
    /// </summary>
    public IReadOnlyDictionary<string, string> Data { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets the component's styling.
    /// </summary>
    public DropdownComponentStyle Style { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether pressing the component asks the controller for text.
    /// </summary>
    public bool RequiresInput { get; init; }

    /// <summary>
    /// Gets the components nested inside a group or scroll area.
    /// </summary>
    public IReadOnlyList<DropdownComponent> Children { get; init; } = System.Array.Empty<DropdownComponent>();
}

/// <summary>
/// Represents a dropdown a plugin attaches to a tag action.
/// </summary>
/// <param name="Title">The title shown at the top of the dropdown.</param>
/// <param name="Width">The dropdown width.</param>
/// <param name="MaxHeight">The height beyond which the dropdown scrolls.</param>
/// <param name="Components">The components in display order.</param>
public sealed record DropdownDefinition(
    string Title,
    int Width,
    int MaxHeight,
    IReadOnlyList<DropdownComponent> Components);

/// <summary>
/// Represents a component pressed in a plugin dropdown opened on a radar track.
/// </summary>
/// <param name="ActionId">The id of the action whose dropdown was open.</param>
/// <param name="ComponentId">The id of the component that was pressed.</param>
/// <param name="TagId">The id of the tag item the dropdown was opened from.</param>
/// <param name="Callsign">The callsign of the radar track the dropdown was opened on.</param>
/// <param name="UserInput">The text the controller entered, or null when the component asks for none.</param>
/// <param name="Data">The values of the pressed component.</param>
public sealed record DropdownActionEvent(
    string ActionId,
    string ComponentId,
    string TagId,
    string Callsign,
    string? UserInput,
    IReadOnlyDictionary<string, string> Data)
{
    /// <summary>
    /// Gets the callsign of the flight plan the track stands for, which may be one correlated by hand.
    /// </summary>
    /// <remarks>Empty for a track whose own plan another track holds by hand.</remarks>
    public string PlanCallsign { get; init; } = "";
}
