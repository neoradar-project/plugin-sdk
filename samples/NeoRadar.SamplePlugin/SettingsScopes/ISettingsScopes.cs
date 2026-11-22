namespace NeoRadar.SamplePlugin.SettingsScopes;

/// <summary>Reads and writes the settings demo's keys in the plugin scope and the profile scope.</summary>
internal interface ISettingsScopes
{
    void WriteGlobal();

    void WriteForProfile();

    void ReadUnsetKey();

    void Show();
}
