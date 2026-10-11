using System;
using System.Globalization;

namespace NeoRadar.PluginSdk.Expressions;

/// <summary>
/// A named, typed value the plugin exports for package conditions. Build one with
/// <see cref="Boolean"/>, <see cref="Number"/> or <see cref="Text"/>, so the default is always a
/// value of the declared type.
/// </summary>
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
    /// Stable identifier within the plugin. It forms the expression id and must be unique across
    /// the plugin's expressions; renaming it breaks every package condition that names it.
    /// </summary>
    public string Name { get; }

    /// <summary>The kind of value. A push of another kind is refused.</summary>
    public ExpressionValueType Type { get; }

    /// <summary>
    /// What a package condition reads for an aircraft the plugin has pushed nothing for, in the
    /// form a condition compares: <c>"1"</c> or <c>"0"</c>, an invariant number, or the text.
    /// </summary>
    public string DefaultValue { get; }

    /// <summary>One line telling a package author what the value means.</summary>
    public string Description { get; }

    /// <summary>What the value belongs to. An aircraft unless set otherwise.</summary>
    public ExpressionScope Scope { get; init; } = ExpressionScope.Aircraft;

    /// <summary>A true or false expression.</summary>
    public static ExpressionDefinition Boolean(string name, bool defaultValue, string description) =>
        new(name, ExpressionValueType.Boolean, defaultValue ? "1" : "0", description);

    /// <summary>A numeric expression.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The default is NaN or infinite.</exception>
    public static ExpressionDefinition Number(string name, double defaultValue, string description)
    {
        if (!double.IsFinite(defaultValue))
            throw new ArgumentOutOfRangeException(nameof(defaultValue), "An expression's default must be a finite number.");

        return new ExpressionDefinition(
            name, ExpressionValueType.Number, defaultValue.ToString("R", CultureInfo.InvariantCulture), description);
    }

    /// <summary>A text expression. A null default is the empty string.</summary>
    public static ExpressionDefinition Text(string name, string defaultValue, string description) =>
        new(name, ExpressionValueType.Text, defaultValue ?? string.Empty, description);
}
