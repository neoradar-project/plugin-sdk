using System;
using System.Collections.Generic;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Common.Events;

public sealed class PluginEventBusTests
{
    [Fact]
    public void AThrowingSubscriber_IsLogged_AndTheNextStillHearsTheEvent()
    {
        FakeLoggerApi logger = new();
        PluginEventBus bus = new(logger);
        List<int> heard = new();
        bus.Subscribe<Pinged>(_ => throw new InvalidOperationException("boom"));
        bus.Subscribe<Pinged>(evt => heard.Add(evt.Count));

        bus.Publish(new Pinged(3));

        Assert.Equal([3], heard);
        Assert.Single(logger.Lines);
    }

    [Fact]
    public void ADisposedSubscription_HearsNothingMore()
    {
        PluginEventBus bus = new(new FakeLoggerApi());
        List<int> heard = new();
        IDisposable subscription = bus.Subscribe<Pinged>(evt => heard.Add(evt.Count));

        bus.Publish(new Pinged(1));
        subscription.Dispose();
        bus.Publish(new Pinged(2));

        Assert.Equal([1], heard);
    }

    private sealed record Pinged(int Count);
}
