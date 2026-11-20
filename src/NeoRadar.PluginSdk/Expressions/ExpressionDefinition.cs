using System;
using System.Globalization;

namespace NeoRadar.PluginSdk.Expressions;

/// <summary>
/// Represents a named, typed value a plugin exports for package conditions.
/// </summary>
/// <remarks>Create one with <see cref="Boolean"/>, <see cref="Number"/> or <see cref="Text"/> so the default matches the type.</remarks>
public sealed record ExpressionDefinition
{
    private ExpressionDefinition(string name, ExpressionValueType type, string defaultValue, string description)
    {
        Name = name ?? string.Empty;
        Type = type;
        DefaultValue = defaultValue;
        Description = description ?? string.Empty;
    }

    /// <summary>
    /// Gets the identifier within the plugin that forms the expression id.
    /// </summary>
    /// <remarks>Must be unique within the plugin, and renaming it breaks every condition that names it.</remarks>
    public string Name { get; }

    /// <summary>
    /// Gets the kind of value.
    /// </summary>
    /// <remarks>A push of another kind is refused.</remarks>
    public ExpressionValueType Type { get; }

    /// <summary>
    /// Gets the value a condition reads for an aircraft the plugin has pushed nothing for.
    /// </summary>
    /// <remarks>Held as a condition compares it, which is "1" or "0", an invariant number, or the text.</remarks>
    public string DefaultValue { get; }

    /// <summary>
    /// Gets the one-line description shown to a package author.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets what the value belongs to, which defaults to an aircraft.
    /// </summary>
    public ExpressionScope Scope { get; init; } = ExpressionScope.Aircraft;

    /// <summary>
    /// Creates a true or false expression.
    /// </summary>
    public static ExpressionDefinition Boolean(string name, bool defaultValue, string description) =>
        new(name, ExpressionValueType.Boolean, defaultValue ? "1" : "0", description);

    /// <summary>
    /// Creates a numeric expression.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The default is NaN or infinite.</exception>
    public static ExpressionDefinition Number(string name, double defaultValue, string description)
    {
        if (!double.IsFinite(defaultValue))
            throw new ArgumentOutOfRangeException(nameof(defaultValue), "An expression's default must be a finite number.");

        return new ExpressionDefinition(
            name, ExpressionValueType.Number, defaultValue.ToString("R", CultureInfo.InvariantCulture), description);
    }

    /// <summary>
    /// Creates a text expression.
    /// </summary>
    /// <param name="name">The identifier within the plugin that forms the expression id.</param>
    /// <param name="defaultValue">The default text, where null is read as an empty string.</param>
    /// <param name="description">The one-line description shown to a package author.</param>
    public static ExpressionDefinition Text(string name, string defaultValue, string description) =>
        new(name, ExpressionValueType.Text, defaultValue ?? string.Empty, description);
}
