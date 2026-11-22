using NeoRadar.SamplePlugin.EventLog;
using NeoRadar.SamplePlugin.EventLog.Events;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.EventLog;

public sealed class EventCountStoreTests
{
    [Fact]
    public void Counts_AreKeptPerEvent_InNameOrder_AndTheNewestLineComesFirst()
    {
        EventCountStore store = new();
        long before = store.Version;

        store.Count("OnTimer");
        store.Count("OnAircraftConnected");
        store.Count("OnTimer");
        store.Note("first");
        store.Note("second");

        EventsCounted counted = store.Snapshot();
        Assert.Equal([new EventCount("OnAircraftConnected", 1), new EventCount("OnTimer", 2)], counted.Counts);
        Assert.Equal(["second", "first"], counted.Recent);
        Assert.True(store.Version > before);
    }
}
