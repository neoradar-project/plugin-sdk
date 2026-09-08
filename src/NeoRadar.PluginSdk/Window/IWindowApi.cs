using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Window;

/// <summary>
/// Builds and mutates a plugin window. The host remembers a window's position and size per plugin
/// and title, so a window the controller moved reopens where they left it, and it honours
/// <see cref="WindowDefinition.Height"/> at creation.
/// </summary>
public interface IWindowInterface
{
    WindowHandle OpenWindow(WindowDefinition definition);

    bool UpdateElement(WindowHandle window, string elementId, string text);

    /// <summary>
    /// Show or hide one element and its subtree. Returns false for an unknown window or element.
    /// </summary>
    bool SetElementVisible(WindowHandle window, string elementId, bool visible);

    /// <summary>
    /// Enable or disable one element, greying it out and refusing input while disabled. Returns
    /// false for an unknown window or element.
    /// </summary>
    bool SetElementEnabled(WindowHandle window, string elementId, bool enabled);

    /// <summary>
    /// Replace one element's style wholesale; every field of <paramref name="style"/> applies, so
    /// an omitted field falls back to the theme rather than keeping its previous value. Returns
    /// false for an unknown window or element.
    /// </summary>
    bool SetElementStyle(WindowHandle window, string elementId, ElementStyle style);

    /// <summary>
    /// Replace an element's children with <paramref name="children"/>. The subtree is rebuilt, so
    /// the old children are gone and event handlers fire against the new ids. Returns false for an
    /// unknown window or element.
    /// </summary>
    bool ReplaceChildren(WindowHandle window, string elementId, IReadOnlyList<WindowElement> children);

    /// <summary>
    /// Append one child to the end of a container's children. Returns false for an unknown window
    /// or parent element.
    /// </summary>
    bool AppendChild(WindowHandle window, string parentId, WindowElement child);

    /// <summary>
    /// Remove one element and its subtree from its parent. Returns false for an unknown window or
    /// element.
    /// </summary>
    bool RemoveChild(WindowHandle window, string elementId);

    /// <summary>
    /// Move a Select or Radio element's selection. An index outside the element's options clears
    /// the selection. Returns false for an unknown window or element.
    /// </summary>
    bool SetSelectedIndex(WindowHandle window, string elementId, int index);

    /// <summary>
    /// Set a Checkbox, Toggle or Radio element's check state without waiting for a click. Returns
    /// false for an unknown window or element.
    /// </summary>
    bool SetChecked(WindowHandle window, string elementId, bool @checked);

    bool CloseWindow(WindowHandle window);
}

public interface IWindowApi
{
    IWindowInterface? GetInterface();
}
