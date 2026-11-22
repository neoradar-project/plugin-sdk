namespace NeoRadar.PluginSdk.Tag;

/// <summary>
/// Provides registration of tag items, tag actions and their dropdowns.
/// </summary>
public interface ITagInterface
{
    /// <summary>
    /// Registers a tag item a package can place in a label.
    /// </summary>
    /// <returns>The tag id, <c>plugin:{manifest name}:TAG_{name}</c>, or an empty string when the name is empty or blank, already registered, or the id would be 128 characters or more.</returns>
    string RegisterTagItem(TagItemDefinition definition);

    /// <summary>
    /// Registers a tag action a package can bind to a click.
    /// </summary>
    /// <returns>The action id, <c>plugin:{manifest name}:ACTION_{name}</c>, or an empty string when the name is empty or blank, already registered, or the id would be 128 characters or more.</returns>
    string RegisterTagAction(TagActionDefinition definition);

    /// <summary>
    /// Sets the value one of this plugin's tag items shows for one aircraft.
    /// </summary>
    /// <remarks>An empty value shows the item's <see cref="TagItemDefinition.DefaultValue"/>, on radar labels, in lists and in conditions alike.</remarks>
    /// <returns>False if the id is not a tag item this plugin registered.</returns>
    bool UpdateTagValue(string tagId, string value, TagContext context);

    /// <summary>
    /// Attaches a dropdown to a tag action this plugin owns.
    /// </summary>
    /// <returns>False if the action is not this plugin's, requires input, or already has a dropdown.</returns>
    bool SetActionDropdown(string actionId, DropdownDefinition dropdown);

    /// <summary>
    /// Attaches or replaces the dropdown of a tag action this plugin owns.
    /// </summary>
    /// <returns>False if the action is not this plugin's or requires input.</returns>
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
