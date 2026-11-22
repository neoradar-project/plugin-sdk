using System;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using L = NeoRadar.SamplePlugin.DemoWindow.DemoWindowLayout;

namespace NeoRadar.SamplePlugin.DemoWindow;

/// <summary>A form window, opened on request, listed in the client's window menu, and changed while hidden.</summary>
internal sealed class DemoWindowFeature : IFeature, IWindowEventHandler, IDemoWindow
{
    private readonly IWindowInterface _windows;

    // Read and written on the UI thread only.
    private WindowHandle _window;

    public DemoWindowFeature(HostSurfaces surfaces) => _windows = surfaces.Window;

    public void Initialize()
    {
    }

    public void Shutdown()
    {
        if (_window.IsValid) _windows.CloseWindow(_window);
        _window = WindowHandle.Invalid;
    }

    // A ShowInList window the controller closed is only hidden, so the menu reopens it; this opens it the first time.
    public void Open()
    {
        if (_window.IsValid) return;
        _window = _windows.OpenWindow(L.Build());
    }

    // UI thread.
    public void OnWindowEvent(WindowEvent evt)
    {
        if (evt.Window != _window || !_window.IsValid) return;

        switch (evt.EventType)
        {
            case "close":
                _windows.UpdateElement(_window, L.HiddenNote, $"The plugin changed this line at {DateTimeOffset.UtcNow:HH:mm:ss} while the window was hidden.");
                break;
            case "click" when evt.ElementId == L.Save:
                _windows.UpdateElement(_window, L.Progress, "1");
                _windows.UpdateElement(_window, L.Status, "Saved");
                break;
            case "click" when evt.ElementId == L.Close:
                _windows.CloseWindow(_window);
                _window = WindowHandle.Invalid;
                break;
            case "click" when evt.ElementId == L.Hint:
                _windows.UpdateElement(_window, L.Status, $"Clicked with the {evt.Value} button");
                break;
            case "change" when evt.ElementId == L.Callsign:
                _windows.UpdateElement(_window, L.Status, $"Editing: {evt.Value}");
                break;
            case "change" when evt.ElementId == L.Urgent:
                _windows.UpdateElement(_window, L.Status, $"Urgent: {evt.Value}");
                break;
            case "select" when evt.ElementId == L.Traffic:
                _windows.UpdateElement(_window, L.Status, evt.Value.Length == 0 ? "Nothing selected" : $"Selected {evt.Value}");
                break;
            case "activate" when evt.ElementId == L.Traffic:
                _windows.UpdateElement(_window, L.Status, $"Opened {evt.Value}");
                break;
            case "sort" when evt.ElementId == L.Traffic:
                _windows.SetTableRows(_window, L.Traffic, L.Rows(byState: evt.Value == "state"), -1);
                break;
        }
    }
}
