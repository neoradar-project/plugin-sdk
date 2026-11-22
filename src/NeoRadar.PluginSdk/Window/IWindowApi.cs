using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Window;

/// <summary>
/// Provides methods to open and update plugin windows.
/// </summary>
/// <remarks>
/// Each method returns <c>false</c> for an unknown or closed window. An element call is applied asynchronously, and an
/// unknown element or one of the wrong type is ignored. A call never raises a <see cref="WindowEvent"/> back to the plugin,
/// and the host remembers placement per plugin and title.
/// </remarks>
public interface IWindowInterface
{
    /// <summary>
    /// Opens a window built from a definition.
    /// </summary>
    /// <remarks>A window the controller closed frees its place, but a hidden <c>ShowInList</c> window does not.</remarks>
    /// <returns>The window's handle, or <see cref="WindowHandle.Invalid"/> when the plugin already holds 10 windows.</returns>
    WindowHandle OpenWindow(WindowDefinition definition);

    /// <summary>
    /// Replaces the text of an element.
    /// </summary>
    /// <remarks>
    /// A Label, Heading, Paragraph, Button, Badge or TextInput takes its text, a Group its caption and a TabPage its header.
    /// A Checkbox, Toggle or single Radio takes <c>true</c> or <c>false</c>, and a Select an option's text. A Slider,
    /// NumberInput or ProgressBar takes a number in the invariant culture, where a negative ProgressBar value is
    /// indeterminate. A Tabs, Image, Table or Radio group takes no text, and a captioned Select, Slider or NumberInput keeps
    /// its caption.
    /// </remarks>
    /// <param name="text">The new text or value, read as the element's type reads it.</param>
    bool UpdateElement(WindowHandle window, string elementId, string text);

    /// <summary>
    /// Shows or hides an element and its subtree.
    /// </summary>
    bool SetElementVisible(WindowHandle window, string elementId, bool visible);

    /// <summary>
    /// Enables or disables an element, which greys it out and blocks input.
    /// </summary>
    bool SetElementEnabled(WindowHandle window, string elementId, bool enabled);

    /// <summary>
    /// Replaces the whole style of an element.
    /// </summary>
    /// <param name="style">The new style, where a field left unset falls back to the theme rather than the previous value.</param>
    bool SetElementStyle(WindowHandle window, string elementId, ElementStyle style);

    /// <summary>
    /// Replaces the children of an element and rebuilds its subtree.
    /// </summary>
    /// <param name="children">The new children, whose ids events then report.</param>
    bool ReplaceChildren(WindowHandle window, string elementId, IReadOnlyList<WindowElement> children);

    /// <summary>
    /// Appends a child, which may be a TabPage, to the end of a container.
    /// </summary>
    /// <remarks>A TabPage appended to a Tabs becomes a new tab.</remarks>
    bool AppendChild(WindowHandle window, string parentId, WindowElement child);

    /// <summary>
    /// Removes an element and its subtree from its parent.
    /// </summary>
    bool RemoveChild(WindowHandle window, string elementId);

    /// <summary>
    /// Sets the selection of a Select or Radio element.
    /// </summary>
    /// <param name="index">The option index, where an index outside the options clears the selection.</param>
    bool SetSelectedIndex(WindowHandle window, string elementId, int index);

    /// <summary>
    /// Sets the check state of a Checkbox, Toggle or Radio element.
    /// </summary>
    bool SetChecked(WindowHandle window, string elementId, bool @checked);

    /// <summary>
    /// Replaces the options of a Select or Radio element in place without raising <c>change</c>.
    /// </summary>
    bool SetOptions(WindowHandle window, string elementId, IReadOnlyList<string> options, int selectedIndex);

    /// <summary>
    /// Replaces the rows of a Table in place, keeping the scroll position, without raising <c>select</c>.
    /// </summary>
    bool SetTableRows(WindowHandle window, string elementId, IReadOnlyList<TableRow> rows, int selectedIndex);

    /// <summary>
    /// Sets the selected row of a Table without raising <c>select</c>.
    /// </summary>
    /// <param name="index">The row index, or -1 to clear the selection.</param>
    bool SetTableSelection(WindowHandle window, string elementId, int index);

    /// <summary>
    /// Closes a window.
    /// </summary>
    bool CloseWindow(WindowHandle window);
}

/// <summary>
/// Provides access to the host's window interface.
/// </summary>
public interface IWindowApi
{
    /// <summary>
    /// Gets the host's window interface, which is never null and the same instance on every call.
    /// </summary>
    IWindowInterface GetInterface();
}
