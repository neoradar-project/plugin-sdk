namespace NeoRadar.PluginSdk.Tag;

/// <summary>
/// Provides registration of tag items, tag actions and their dropdowns.
/// </summary>
public interface ITagInterface
{
    /// <summary>
    /// Registers a tag item a package can place in a label.
    /// </summary>
    /// <returns>The tag id, or an empty string when the name is empty or already registered.</returns>
    string RegisterTagItem(TagItemDefinition definition);

    /// <summary>
    /// Registers a tag action a package can bind to a click.
    /// </summary>
    /// <returns>The action id, or an empty string when the name is empty or already registered.</returns>
    string RegisterTagAction(TagActionDefinition definition);

    /// <summary>
    /// Sets the value a tag item shows for one aircraft.
    /// </summary>
    /// <returns>False if the tag id was not registered.</returns>
    bool UpdateTagValue(string tagId, string value, TagContext context);

    /// <summary>
    /// Attaches a dropdown to a tag action this plugin owns.
    /// </summary>
    /// <returns>False if the action is not this plugin's or already has a dropdown.</returns>
    bool SetActionDropdown(string actionId, DropdownDefinition dropdown);

    /// <summary>
    /// Attaches or replaces the dropdown of a tag action this plugin owns.
    /// </summary>
    /// <returns>False if the action is not this plugin's.</returns>
    bool UpdateActionDropdown(string actionId, DropdownDefinition dropdown);

    /// <summary>
    /// Removes the dropdown of a tag action this plugin owns.
    /// </summary>
    /// <returns>False if the action has no dropdown or the dropdown is not this plugin's.</returns>
    bool RemoveActionDropdown(string actionId);

    /// <summary>
    /// Gets the dropdown attached to a tag action.
    /// </summary>
    /// <returns>The dropdown, or null when the action has none.</returns>
    DropdownDefinition? GetDropdownForAction(string actionId);
}

/// <summary>
/// Provides access to the host's tag interface.
/// </summary>
public interface ITagApi
{
    /// <summary>
    /// Gets the host's tag interface, which is never null and the same instance on every call.
    /// </summary>
    ITagInterface GetInterface();
}
