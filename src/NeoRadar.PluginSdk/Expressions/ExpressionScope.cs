namespace NeoRadar.PluginSdk.Expressions;

/// <summary>What an expression's value belongs to.</summary>
public enum ExpressionScope
{
    /// <summary>One value per aircraft, pushed with <c>IExpressionApi.Push</c>.</summary>
    Aircraft,

    /// <summary>
    /// One value for the whole client, pushed with <c>IExpressionApi.PushSession</c>. The client
    /// stores and lists it, but a package condition evaluated for an aircraft reads it as empty.
    /// </summary>
    Session,
}
