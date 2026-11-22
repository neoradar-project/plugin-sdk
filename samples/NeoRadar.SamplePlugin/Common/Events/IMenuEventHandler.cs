using NeoRadar.PluginSdk.Menu;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Clicks on the plugin's menu entries, on client threads.</summary>
internal interface IMenuEventHandler
{
    void OnMenuItemClicked(MenuItemClickedEvent evt) { }
}
