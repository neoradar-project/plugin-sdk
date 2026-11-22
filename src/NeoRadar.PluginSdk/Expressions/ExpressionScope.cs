namespace NeoRadar.PluginSdk.Expressions;

/// <summary>
/// Specifies what an expression's value belongs to.
/// </summary>
public enum ExpressionScope
{
    /// <summary>
    /// One value per aircraft, pushed with <c>IExpressionApi.Push</c>.
    /// </summary>
    Aircraft,

    /// <summary>
    /// One value for the whole client, pushed with <c>IExpressionApi.PushSession</c>.
    /// </summary>
    /// <remarks>A map item's showWhen reads it, and a condition evaluated for an aircraft reads it as empty.</remarks>
    Session,
}
