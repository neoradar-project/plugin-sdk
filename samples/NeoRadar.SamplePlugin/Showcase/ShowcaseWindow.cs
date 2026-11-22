using System.Collections.Generic;
using System.Globalization;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.EventLog.Events;
using L = NeoRadar.SamplePlugin.Showcase.ShowcaseWindowLayout;
using P = NeoRadar.SamplePlugin.Showcase.ShowcasePageLayout;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>The open Showcase window: the page its detail pane shows, and each page's last readout; callable from any thread.</summary>
internal sealed class ShowcaseWindow
{
    private readonly IWindowInterface _windows;
    private readonly DemoWritesGate _gate;

    // Guards every field below, so a page swap and a readout write are never interleaved.
    private readonly object _lock = new();
    private readonly Dictionary<string, IReadOnlyList<string>> _readouts = new();
    private IReadOnlyList<TableRow> _counts = [];
    private WindowHandle _handle = WindowHandle.Invalid;
    private ShowcasePage _page = ShowcaseGuide.Pages[0];

    public ShowcaseWindow(IWindowInterface windows, DemoWritesGate gate)
    {
        _windows = windows;
        _gate = gate;
    }

    public WindowHandle Handle
    {
        get { lock (_lock) return _handle; }
    }

    /// <summary>Opens the window on the first page, with Demo writes closed.</summary>
    public WindowHandle Open()
    {
        lock (_lock)
        {
            _gate.Set(false);
            _page = ShowcaseGuide.Pages[0];
            _handle = _windows.OpenWindow(L.Build(ShowcaseGuide.Groups, _page, PageContent(_page)));
            return _handle;
        }
    }

    public void Close()
    {
        lock (_lock)
        {
            if (_handle.IsValid) _windows.CloseWindow(_handle);
            _handle = WindowHandle.Invalid;
        }
    }

    // UI thread, from a nav click.
    public void ShowPage(ShowcasePage page)
    {
        lock (_lock)
        {
            if (!_handle.IsValid || page.Id == _page.Id) return;

            _windows.SetElementStyle(_handle, L.NavId(_page.Id), L.NavItemStyle(active: false));
            _windows.SetElementStyle(_handle, L.NavId(page.Id), L.NavItemStyle(active: true));
            _page = page;
            _windows.ReplaceChildren(_handle, L.DetailPane, PageContent(page));
        }
    }

    // UI thread, from the Demo writes tick; only the shown page's buttons exist to enable.
    public void SetDemoWrites(bool open)
    {
        lock (_lock)
        {
            _gate.Set(open);
            if (!_handle.IsValid) return;
            foreach (DemoButton button in _page.Buttons)
                if (button.SendsToNetwork) _windows.SetElementEnabled(_handle, P.ButtonId(_page, button.DemoId), open);
        }
    }

    public void ShowReadout(string pageId, IReadOnlyList<string> lines)
    {
        lock (_lock)
        {
            _readouts[pageId] = lines;
            if (_handle.IsValid && pageId == _page.Id) _windows.ReplaceChildren(_handle, P.ReadoutId(pageId), P.ReadoutLines(lines));
        }
    }

    public void ShowCounts(EventsCounted counted)
    {
        List<TableRow> rows = new(counted.Counts.Count);
        foreach (EventCount count in counted.Counts)
            rows.Add(new TableRow(count.EventName, [count.EventName, count.Count.ToString(CultureInfo.InvariantCulture)]));

        lock (_lock)
        {
            _counts = rows;
            if (_handle.IsValid && _page.Id == ShowcaseGuide.Events) _windows.SetTableRows(_handle, P.CountsTable, rows, -1);
        }
        ShowReadout(ShowcaseGuide.Events, counted.Recent);
    }

    // Under the lock.
    private IReadOnlyList<WindowElement> PageContent(ShowcasePage page)
    {
        IReadOnlyList<string> readout = _readouts.TryGetValue(page.Id, out IReadOnlyList<string>? lines) ? lines : [];
        return P.Build(page, _gate.IsOpen, readout, _counts);
    }
}
