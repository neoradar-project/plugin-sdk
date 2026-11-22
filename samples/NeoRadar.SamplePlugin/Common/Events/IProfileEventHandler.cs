using NeoRadar.PluginSdk.Settings;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>The client loaded a different profile, so profile-scoped settings must be read again.</summary>
internal interface IProfileEventHandler
{
    void OnActiveProfileChanged(ActiveProfileChangedEvent evt) { }
}
