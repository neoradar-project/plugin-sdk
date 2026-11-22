namespace NeoRadar.PluginSdk.Expressions;

/// <summary>
/// Specifies the kind of value an expression holds, fixed when it is declared.
/// </summary>
public enum ExpressionValueType
{
    /// <summary>
    /// A true or false value, read by a condition as <c>"1"</c> or <c>"0"</c>.
    /// </summary>
    Boolean,

    /// <summary>
    /// A finite number, read by a condition in invariant form such as <c>"3.5"</c>.
    /// </summary>
    Number,

    /// <summary>
    /// Text, read by a condition exactly as pushed.
    /// </summary>
    Text,
}
