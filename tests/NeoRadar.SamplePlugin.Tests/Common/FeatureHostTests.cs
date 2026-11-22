using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Tag;
using NeoRadar.PluginSdk.Timer;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Common.Scheduling;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Common;

public sealed class FeatureHostTests
{
    private readonly FakeLoggerApi _logger = new();
    private readonly List<string> _calls = new();

    [Fact]
    public void AThrowingFeature_DoesNotStopTheOthers_FromInitialising_OrHearingAnEvent()
    {
        ProbeFeature first = new("first", _calls, throws: true);
        ProbeFeature second = new("second", _calls, throws: false);
        FeatureHost host = HostOf(first, second);

        host.Initialize();
        host.OnPositionUpdate(new PositionUpdateEvent([]));

        Assert.Equal(["first Initialize", "second Initialize", "first OnPositionUpdate", "second OnPositionUpdate"], _calls);
        Assert.Equal(2, _logger.Lines.Count);
        Assert.Contains("ProbeFeature.Initialize", _logger.Lines[0].Message);
    }

    [Fact]
    public void Shutdown_RunsInReverseOrder_AndOnlyOnce()
    {
        FeatureHost host = HostOf(new ProbeFeature("first", _calls, false), new ProbeFeature("second", _calls, false));
        host.Initialize();
        _calls.Clear();

        host.Shutdown();
        host.Shutdown();

        Assert.Equal(["second Shutdown", "first Shutdown"], _calls);
    }

    [Fact]
    public void OnTagShowDropdown_IsRefused_WhenAnyFeatureRefuses_AndAThrowDoesNotRefuse()
    {
        FeatureHost host = HostOf(new ProbeFeature("a", _calls, throws: true), new ProbeFeature("b", _calls, false, allowDropdown: false));
        TagShowDropdownEvent evt = new() { ActionId = "x", Callsign = "BAW1", PlanCallsign = "BAW1" };

        Assert.False(host.OnTagShowDropdown(evt));
        Assert.True(HostOf(new ProbeFeature("c", _calls, throws: true)).OnTagShowDropdown(evt));
    }

    [Fact]
    public void Shutdown_StopsTheScheduler_SoNoTimedJobRunsAfterIt()
    {
        TimedTaskScheduler scheduler = new(_logger, new FakeTimeProvider(), new InlineTaskScheduler());
        int runs = 0;
        scheduler.Every("probe", TimeSpan.FromSeconds(1), () => runs++);
        FeatureHost host = HostOf(scheduler, new ProbeFeature("first", _calls, false));
        host.Initialize();

        host.Shutdown();
        host.OnTimer(new TimerEvent(1));

        Assert.Equal(0, runs);
    }

    private FeatureHost HostOf(params ProbeFeature[] features)
        => HostOf(new TimedTaskScheduler(_logger, new FakeTimeProvider(), TaskScheduler.Default), features);

    private FeatureHost HostOf(TimedTaskScheduler scheduler, params ProbeFeature[] features)
        => new(
            _logger,
            scheduler,
            features,
            features,
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            features,
            [],
            [],
            [],
            [],
            []);

    private sealed class ProbeFeature(string name, List<string> calls, bool throws, bool allowDropdown = true)
        : IFeature, IAircraftEventHandler, ITagEventHandler
    {
        public void Initialize() => Record(nameof(Initialize));

        public void Shutdown() => calls.Add($"{name} {nameof(Shutdown)}");

        public void OnPositionUpdate(PositionUpdateEvent evt) => Record(nameof(OnPositionUpdate));

        public bool OnTagShowDropdown(TagShowDropdownEvent evt)
        {
            if (throws) throw new InvalidOperationException("probe");
            return allowDropdown;
        }

        private void Record(string call)
        {
            calls.Add($"{name} {call}");
            if (throws) throw new InvalidOperationException("probe");
        }
    }
}
