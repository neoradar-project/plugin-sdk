using NeoRadar.PluginSdk.Focus;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Selected-aircraft events, on the UI thread.</summary>
internal interface IFocusEventHandler
{
    void OnSelectedAircraftChanged(SelectedAircraftChangedEvent evt) { }
}
