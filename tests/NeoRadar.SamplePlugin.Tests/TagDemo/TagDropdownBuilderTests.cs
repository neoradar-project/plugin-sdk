using System.Linq;
using NeoRadar.PluginSdk.Tag;
using NeoRadar.SamplePlugin.TagDemo;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.TagDemo;

public sealed class TagDropdownBuilderTests
{
    [Fact]
    public void TheToggleAndRadio_AreTickedFromThePluginsState_AndOneToggleAsksForInput()
    {
        DropdownDefinition dropdown = TagDropdownBuilder.Build("BAW1", new TagChoice(Watched: true, Priority: TagDropdownBuilder.High));

        Assert.True(Component(dropdown, TagDropdownBuilder.Watch).Checked);
        Assert.True(Component(dropdown, TagDropdownBuilder.High).Checked);
        Assert.False(Component(dropdown, TagDropdownBuilder.Low).Checked);
        Assert.True(Component(dropdown, TagDropdownBuilder.WatchWithNote).RequiresInput);
        Assert.Equal(DropdownComponentType.Radio, Component(dropdown, TagDropdownBuilder.Low).Type);
    }

    private static DropdownComponent Component(DropdownDefinition dropdown, string id) => dropdown.Components.Single(c => c.Id == id);
}
