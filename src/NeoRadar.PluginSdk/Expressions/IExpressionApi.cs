namespace NeoRadar.PluginSdk.Expressions;

/// <summary>
/// Provides typed values a plugin exports for package conditions such as tag colours, variants and list filters.
/// </summary>
/// <remarks>The client keeps the last value pushed and never calls the plugin to evaluate a condition.</remarks>
public interface IExpressionApi
{
    /// <summary>
    /// Declares an expression, normally once in <c>Initialize</c>.
    /// </summary>
    /// <returns>The id <c>plugin:{Manifest.Name}:EXP_{Name}</c>, or an empty string if the name is empty or already declared.</returns>
    string Declare(ExpressionDefinition definition);

    /// <summary>
    /// Sets a boolean expression's value for one aircraft.
    /// </summary>
    /// <remarks>Push on change rather than per frame. Pushing the value already held does nothing.</remarks>
    /// <returns>False for an undeclared id, an expression that is not an aircraft boolean, or an empty callsign.</returns>
    bool Push(string expressionId, string callsign, bool value);

    /// <summary>
    /// Sets a numeric expression's value for one aircraft.
    /// </summary>
    /// <returns>False for an undeclared id, a non-aircraft or non-numeric expression, an empty callsign, or NaN or infinity.</returns>
    bool Push(string expressionId, string callsign, double value);

    /// <summary>
    /// Sets a text expression's value for one aircraft.
    /// </summary>
    /// <returns>False for an undeclared id, an expression that is not aircraft text, an empty callsign, or a null value.</returns>
    bool Push(string expressionId, string callsign, string value);

    /// <summary>
    /// Sets the value of a session-scoped boolean expression.
    /// </summary>
    /// <returns>False for an undeclared id or an expression that is not a session boolean.</returns>
    bool PushSession(string expressionId, bool value);

    /// <summary>
    /// Sets the value of a session-scoped numeric expression.
    /// </summary>
    /// <returns>False for an undeclared id, an expression that is not a session number, or NaN or infinity.</returns>
    bool PushSession(string expressionId, double value);

    /// <summary>
    /// Sets the value of a session-scoped text expression.
    /// </summary>
    /// <returns>False for an undeclared id, an expression that is not session text, or a null value.</returns>
    bool PushSession(string expressionId, string value);
}
