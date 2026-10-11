namespace NeoRadar.PluginSdk.Expressions;

/// <summary>What kind of value an expression holds, fixed when it is declared.</summary>
public enum ExpressionValueType
{
    /// <summary>True or false. A package condition reads it as <c>"1"</c> or <c>"0"</c>.</summary>
    Boolean,

    /// <summary>A finite number. A package condition reads it in invariant form, such as <c>"3.5"</c>.</summary>
    Number,

    /// <summary>Text, read by a package condition exactly as pushed.</summary>
    Text,
}
