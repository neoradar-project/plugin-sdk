using NeoRadar.PluginSdk.Tag;

namespace NeoRadar.SamplePlugin.Common.Events;

/// <summary>Tag click and dropdown events, on client threads.</summary>
internal interface ITagEventHandler
{
    void OnTagAction(TagActionEvent evt) { }

    void OnTagDropdownAction(DropdownActionEvent evt) { }

    /// <summary>Return false to stop the dropdown opening; FeatureHost ANDs every handler's answer.</summary>
    bool OnTagShowDropdown(TagShowDropdownEvent evt) => true;
}
