using System.Collections.Generic;
using System.Linq;
using NeoRadar.PluginSdk.Tag;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.TagDemo;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.TagDemo;

public sealed class TagDemoFeatureTests
{
    private readonly FakeCoreApi _core = new();
    private readonly TagDemoFeature _feature;

    public TagDemoFeatureTests()
    {
        _core.FocusFake.Selected = "BAW1";
        _feature = new TagDemoFeature(HostSurfaces.FromHost(_core), _core.FocusFake, _core.LoggerFake, new PluginEventBus(_core.LoggerFake));
        _feature.Initialize();
    }

    private FakeTagInterface Tags => _core.TagFake.Interface;

    [Fact]
    public void ItRegisters_TwoItems_AnInputAction_AndADropdownAction()
    {
        Assert.Equal([TagDemoFeature.NoteItemName, TagDemoFeature.StatusItemName], Tags.Items.Select(i => i.Name));
        Assert.Equal([true, false], Tags.Actions.Select(a => a.RequiresInput));
    }

    [Fact]
    public void TypedInput_BecomesTheNote_ForThatAircraft()
    {
        _feature.OnTagAction(new TagActionEvent("plugin:Sample:ACTION_Note", "plugin:Sample:TAG_Note", "EZY2", 0, "LATE"));

        (string tagId, string value, TagContext context) = Tags.Pushes.Single();
        Assert.Equal(("plugin:Sample:TAG_Note", "LATE", "EZY2"), (tagId, value, context.Callsign));
    }

    [Fact]
    public void AnEmptyPush_IsSentEmpty_SoTheHostShowsTheDefault()
    {
        _feature.PushEmptyStatus();

        Assert.Equal(string.Empty, Tags.Pushes.Single().Value);
    }

    [Fact]
    public void OpeningTheDropdownTwice_UpdatesItTheSecondTime_WithTheNewChoice()
    {
        TagShowDropdownEvent show = new() { ActionId = "plugin:Sample:ACTION_Menu", Callsign = "BAW1", PlanCallsign = "BAW1" };

        Assert.True(_feature.OnTagShowDropdown(show));
        _feature.OnTagDropdownAction(new DropdownActionEvent("plugin:Sample:ACTION_Menu", TagDropdownBuilder.Watch, "", "BAW1", null, new Dictionary<string, string>()));
        _feature.OnTagShowDropdown(show);

        Assert.True(Tags.Dropdowns["plugin:Sample:ACTION_Menu"].Components.Single(c => c.Id == TagDropdownBuilder.Watch).Checked);
    }
}
