namespace NeoRadar.PluginSdk.Expressions;

/// <summary>
/// Typed values the plugin exports for package conditions. A package reads one by its id wherever
/// it reads a key: to colour a tag, pick a variant, filter a list or exclude an aircraft from the
/// safety nets. The client keeps its own copy of the last value pushed and never calls the plugin
/// to evaluate a condition.
/// </summary>
public interface IExpressionApi
{
    /// <summary>
    /// Declare an expression, once, normally in <c>Initialize</c>. Returns the expression id, which
    /// is <c>plugin:{Manifest.Name}:EXP_{Name}</c> and is stable across loads, or an empty string
    /// when the name is empty or already declared by this plugin.
    /// </summary>
    string Declare(ExpressionDefinition definition);

    /// <summary>
    /// Set a boolean expression's value for one aircraft. Push on the change, not per frame: a push
    /// of the value already held does nothing. Returns false for an id this plugin did not declare,
    /// an expression that is not an aircraft-scoped boolean, or an empty callsign.
    /// </summary>
    bool Push(string expressionId, string callsign, bool value);

    /// <summary>
    /// Set a numeric expression's value for one aircraft. Returns false for an id this plugin did
    /// not declare, an expression that is not an aircraft-scoped number, an empty callsign, or a
    /// value that is NaN or infinite.
    /// </summary>
    bool Push(string expressionId, string callsign, double value);

    /// <summary>
    /// Set a text expression's value for one aircraft. Returns false for an id this plugin did not
    /// declare, an expression that is not aircraft-scoped text, an empty callsign, or a null value.
    /// </summary>
    bool Push(string expressionId, string callsign, string value);

    /// <summary>
    /// Set a session-scoped boolean expression's one value. Returns false for an id this plugin did
    /// not declare or an expression that is not a session-scoped boolean.
    /// </summary>
    bool PushSession(string expressionId, bool value);

    /// <summary>
    /// Set a session-scoped numeric expression's one value. Returns false for an id this plugin did
    /// not declare, an expression that is not a session-scoped number, or a value that is NaN or
    /// infinite.
    /// </summary>
    bool PushSession(string expressionId, double value);

    /// <summary>
    /// Set a session-scoped text expression's one value. Returns false for an id this plugin did
    /// not declare, an expression that is not session-scoped text, or a null value.
    /// </summary>
    bool PushSession(string expressionId, string value);
}
