using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Logger;
using NeoRadar.PluginSdk.Menu;
using NeoRadar.PluginSdk.Window;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using G = NeoRadar.SamplePlugin.Showcase.ShowcaseGuide;
using L = NeoRadar.SamplePlugin.Showcase.ShowcaseWindowLayout;
using P = NeoRadar.SamplePlugin.Showcase.ShowcasePageLayout;

namespace NeoRadar.SamplePlugin.Showcase;

/// <summary>The plugin's front page: a window of guide pages, picked from a nav, whose buttons run each feature's live demo.</summary>
internal sealed class ShowcaseFeature : IFeature, IWindowEventHandler, IMenuEventHandler
{
    public const string MenuItemName = "Showcase";

    private readonly ShowcaseWindow _window;
    private readonly IMenuApi _menu;
    private readonly ILoggerApi _logger;
    private readonly DemoWritesGate _gate;
    private readonly ShowcaseDemoCatalogue _demos;
    private readonly List<IDisposable> _subscriptions = new();
    private string _menuItemId = string.Empty;
    private int _opens;

    public ShowcaseFeature(ShowcaseWindow window, IMenuApi menu, ILoggerApi logger, DemoWritesGate gate, ShowcaseDemoCatalogue demos)
    {
        _window = window;
        _menu = menu;
        _logger = logger;
        _gate = gate;
        _demos = demos;
    }

    // Registered first, so its subscriptions are in place before any other feature initialises and reports.
    public void Initialize()
    {
        Open();
        _menuItemId = _menu.RegisterMenuItem(new MenuItemDefinition(MenuItemName, "Reopen the Showcase"));
        if (_menuItemId.Length == 0) _logger.Warning("Showcase: the menu entry was refused");

        // Each report arrives on the thread of the feature that published it; window calls are safe from any thread.
        _subscriptions.AddRange(_demos.ShowReports());
    }

    public void Shutdown()
    {
        foreach (IDisposable subscription in _subscriptions) subscription.Dispose();
        _subscriptions.Clear();
        if (_menuItemId.Length > 0) _menu.RemoveMenuItem(_menuItemId);
        _window.Close();
    }

    // UI thread.
    public void OnWindowEvent(WindowEvent evt)
    {
        if (evt.Window != _window.Handle) return;

        if (evt.EventType == "change" && evt.ElementId == L.DemoWrites)
            _window.SetDemoWrites(evt.Value == "true");
        else if (evt.EventType == "click" && L.PageIdOf(evt.ElementId) is string pageId)
            ShowPage(pageId);
        else if (evt.EventType == "click" && P.DemoIdOf(evt.ElementId) is string demoId)
            RunUnlessGated(demoId);
        else if (evt.EventType == "submit")
            SubmitInput(evt.ElementId, evt.Value);
    }

    // A client thread; the host cannot show a hidden window again, so the Showcase is opened afresh.
    public void OnMenuItemClicked(MenuItemClickedEvent evt)
    {
        if (evt.ItemId != _menuItemId) return;
        _window.Close();
        Open();
        _menu.UpdateMenuItem(_menuItemId, $"Reopen the Showcase (reopened {++_opens} times)", null);
    }

    private void Open()
    {
        if (!_window.Open().IsValid) _logger.Warning("Showcase: the window was refused (no UI, or ten windows already open)");
    }

    private void ShowPage(string pageId)
    {
        if (G.PageOrNull(pageId) is ShowcasePage page) _window.ShowPage(page);
        else _logger.Warning($"Showcase: no page {pageId} for its nav item");
    }

    private void RunUnlessGated(string demoId)
    {
        if (IsNetworkDemo(demoId) && !_gate.IsOpen) return;
        Run(demoId, string.Empty);
    }

    private void SubmitInput(string elementId, string text)
    {
        foreach (ShowcasePage page in G.Pages)
            if (page.Input is PageInput input && elementId == P.InputId(page.Id)) Run(input.DemoId, text);
    }

    private void Run(string demoId, string text)
    {
        if (_demos.Find(demoId) is ShowcaseDemo demo) demo.Run(text);
        else _logger.Warning($"Showcase: no demo {demoId} is registered for its button");
    }

    private static bool IsNetworkDemo(string demoId)
    {
        foreach (ShowcasePage page in G.Pages)
            foreach (DemoButton button in page.Buttons)
                if (button.DemoId == demoId && button.SendsToNetwork) return true;
        return false;
    }
}
