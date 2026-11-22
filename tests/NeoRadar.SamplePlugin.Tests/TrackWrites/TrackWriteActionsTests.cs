using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.TrackWrites;
using NeoRadar.SamplePlugin.TrackWrites.Events;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.TrackWrites;

public sealed class TrackWriteActionsTests
{
    private readonly FakeControllerDataApi _data = new();
    private readonly FakeFocusApi _focus = new() { Selected = "BAW1" };
    private readonly DemoWritesGate _gate = new();
    private readonly PluginEventBus _bus = new(new FakeLoggerApi());
    private readonly List<IReadOnlyList<string>> _tallies = new();

    public TrackWriteActionsTests() => _bus.Subscribe<TrackWritesTallied>(t => _tallies.Add(t.Lines));

    [Fact]
    public void WithDemoWritesUnticked_NothingIsWritten_AndThePanelSaysWhy()
    {
        TrackWriteActions actions = new(_data, _focus, _gate, _bus);

        actions.SetScratchpad();

        Assert.Empty(_data.Writes);
        Assert.Contains("tick Demo writes first", _tallies[^1][1]);
    }

    [Fact]
    public void ABurst_PastTheHostLimit_TalliesWhatWasSentAndRefused()
    {
        _gate.Set(true);
        _data.AllowedWrites = 1;
        TrackWriteActions actions = new(_data, _focus, _gate, _bus);

        actions.BurstScratchpad();

        Assert.Equal(TrackWriteActions.BurstSize, _data.Writes.Count);
        Assert.Equal("Sent 1, refused 6", _tallies[^1][0]);
    }

    [Fact]
    public void AcceptingAHandoff_NamesTheControllerWhoMadeNoOffer()
    {
        _gate.Set(true);
        TrackWriteActions actions = new(_data, _focus, _gate, _bus);

        actions.AcceptHandoffWithNoOffer();

        Assert.Equal(["accept BAW1 SAMPLE_CTR"], _data.Writes);
    }

    [Fact]
    public void WithNothingSelected_NothingIsWritten()
    {
        _gate.Set(true);
        _focus.Selected = null;
        TrackWriteActions actions = new(_data, _focus, _gate, _bus);

        actions.Assume();

        Assert.Empty(_data.Writes);
    }
}
