using System;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeTimeProvider : TimeProvider
{
    public DateTimeOffset Now { get; set; } = DateTimeOffset.UnixEpoch;

    public override DateTimeOffset GetUtcNow() => Now;
}
