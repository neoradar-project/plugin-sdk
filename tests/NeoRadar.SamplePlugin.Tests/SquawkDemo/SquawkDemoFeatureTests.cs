using System;
using System.Collections.Generic;
using System.Threading;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.SquawkDemo;
using NeoRadar.SamplePlugin.SquawkDemo.Events;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.SquawkDemo;

public sealed class SquawkDemoFeatureTests
{
    private readonly FakeSquawkApi _squawk = new();
    private readonly DemoWritesGate _gate = new();
    private readonly PluginEventBus _bus = new(new FakeLoggerApi());
    private readonly SquawkDemoFeature _feature;

    public SquawkDemoFeatureTests() => _feature = new SquawkDemoFeature(_squawk, _gate, _bus);

    [Fact]
    public void Register_BeforeDemoWrites_AddsNoProviderThatCouldOutrankTheClientsOwn()
    {
        _feature.Register();

        Assert.Empty(_squawk.Providers);
    }

    [Fact]
    public void Register_WithDemoWrites_AddsTheProvider_AboveTheClientsOwnAtPriorityZero()
    {
        _gate.Set(true);

        _feature.Register();

        Assert.True(Assert.Single(_squawk.Providers).Priority > 0);
    }

    [Fact]
    public void Register_PublishesAfterLeavingItsLock_SoASubscriberOnAnotherThreadCanCallBackIn()
    {
        _gate.Set(true);
        List<bool> calledBack = new();
        int heard = 0;
        _bus.Subscribe<SquawkActivityRecorded>(_ =>
        {
            if (Interlocked.Increment(ref heard) > 1) return;
            Thread other = new(_feature.Unregister);
            other.Start();
            calledBack.Add(other.Join(TimeSpan.FromSeconds(2)));
        });

        _feature.Register();

        Assert.Equal([true], calledBack);
    }

    [Fact]
    public void MakeActive_BeforeDemoWrites_LeavesTheActiveProviderAlone()
    {
        _gate.Set(true);
        _feature.Register();
        _gate.Set(false);

        _feature.MakeActive();

        Assert.Null(_squawk.Active);
    }

    [Fact]
    public void Shutdown_DisposesTheProviderToken()
    {
        _gate.Set(true);
        _feature.Register();

        _feature.Shutdown();

        Assert.Empty(_squawk.Providers);
    }
}
