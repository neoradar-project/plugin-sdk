namespace NeoRadar.PluginSdk.Menu;

/// <summary>
/// A controller clicked one of the plugin's menu entries. <c>ItemId</c> is the id
/// <see cref="IMenuApi.RegisterMenuItem"/> returned. For a checkable entry the host has already
/// toggled the check and <c>Checked</c> carries the new state; for a plain entry it is always false.
/// </summary>
public sealed record MenuItemClickedEvent(string ItemId, bool Checked);
