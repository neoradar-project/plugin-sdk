namespace NeoRadar.SamplePlugin.Aman;

/// <summary>The AMAN inset, which another feature may turn off and on.</summary>
internal interface IAmanInset
{
    /// <returns>Whether the inset is on afterwards.</returns>
    bool Toggle();
}
