using NeoRadar.PluginSdk.Window;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Plugin window events, on the UI thread.</summary>
internal interface IWindowEventHandler
{
    void OnWindowEvent(WindowEvent evt) { }
}
