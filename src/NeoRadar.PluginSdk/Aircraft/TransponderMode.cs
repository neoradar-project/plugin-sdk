namespace NeoRadar.PluginSdk.Aircraft;

/// <summary>
/// Specifies the mode an aircraft's transponder is in.
/// </summary>
public enum TransponderMode
{
    /// <summary>
    /// The transponder is on standby and not replying.
    /// </summary>
    Standby = 0,

    /// <summary>
    /// The transponder is on and replying with altitude (mode C).
    /// </summary>
    ModeC = 1,

    /// <summary>
    /// The transponder is squawking ident.
    /// </summary>
    Ident = 2,
}
