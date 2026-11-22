using System.Collections.Generic;
using NeoRadar.PluginSdk.Window;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeWindowInterface : IWindowInterface
{
    private ulong _next;

    public Dictionary<WindowHandle, WindowDefinition> Open { get; } = new();

    public List<string> Calls { get; } = new();

    public Dictionary<(WindowHandle, string), string> Texts { get; } = new();

    public Dictionary<(WindowHandle, string), bool> EnabledStates { get; } = new();

    public Dictionary<(WindowHandle, string), IReadOnlyList<WindowElement>> Children { get; } = new();

    public WindowHandle OpenWindow(WindowDefinition definition)
    {
        WindowHandle handle = new(++_next);
        Open[handle] = definition;
        return handle;
    }

    public WindowHandle Titled(string title)
    {
        foreach (KeyValuePair<WindowHandle, WindowDefinition> entry in Open)
            if (entry.Value.Title == title) return entry.Key;
        return WindowHandle.Invalid;
    }

    public bool UpdateElement(WindowHandle window, string elementId, string text)
    {
        Calls.Add($"update {elementId} {text}");
        Texts[(window, elementId)] = text;
        return Open.ContainsKey(window);
    }

    public bool SetElementVisible(WindowHandle window, string elementId, bool visible) => Record(window, $"visible {elementId} {visible}");

    public bool SetElementEnabled(WindowHandle window, string elementId, bool enabled)
    {
        EnabledStates[(window, elementId)] = enabled;
        return Record(window, $"enabled {elementId} {enabled}");
    }

    public Dictionary<(WindowHandle, string), ElementStyle> Styles { get; } = new();

    public bool SetElementStyle(WindowHandle window, string elementId, ElementStyle style)
    {
        Styles[(window, elementId)] = style;
        return Record(window, $"style {elementId}");
    }

    public bool ReplaceChildren(WindowHandle window, string elementId, IReadOnlyList<WindowElement> children)
    {
        Children[(window, elementId)] = children;
        return Record(window, $"children {elementId} {children.Count}");
    }

    public bool AppendChild(WindowHandle window, string parentId, WindowElement child) => Record(window, $"append {parentId}");

    public bool RemoveChild(WindowHandle window, string elementId) => Record(window, $"remove {elementId}");

    public bool SetSelectedIndex(WindowHandle window, string elementId, int index) => Record(window, $"selected {elementId} {index}");

    public bool SetChecked(WindowHandle window, string elementId, bool @checked) => Record(window, $"checked {elementId} {@checked}");

    public bool SetOptions(WindowHandle window, string elementId, IReadOnlyList<string> options, int selectedIndex) => Record(window, $"options {elementId}");

    public bool SetTableRows(WindowHandle window, string elementId, IReadOnlyList<TableRow> rows, int selectedIndex) => Record(window, $"rows {elementId} {rows.Count}");

    public bool SetTableSelection(WindowHandle window, string elementId, int index) => Record(window, $"table selection {elementId} {index}");

    public bool CloseWindow(WindowHandle window) => Open.Remove(window);

    private bool Record(WindowHandle window, string call)
    {
        Calls.Add(call);
        return Open.ContainsKey(window);
    }
}
