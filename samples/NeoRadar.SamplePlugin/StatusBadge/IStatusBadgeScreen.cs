namespace NeoRadar.SamplePlugin.StatusBadge;

/// <summary>The status badge, a main-radar screen another feature may turn off and on.</summary>
internal interface IStatusBadgeScreen
{
    /// <returns>Whether the screen is on afterwards.</returns>
    bool Toggle();
}
