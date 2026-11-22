namespace NeoRadar.SamplePlugin.QuietHours;

/// <summary>Pushes the plugin's expressions that a package binds in its conditions.</summary>
internal interface IQuietHours
{
    void SetQuiet(bool quiet);

    void ToggleFlagOnSelected();
}
