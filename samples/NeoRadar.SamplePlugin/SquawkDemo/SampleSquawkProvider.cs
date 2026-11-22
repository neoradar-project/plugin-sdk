using System.Threading;
using System.Threading.Tasks;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Flightplan;
using NeoRadar.PluginSdk.Squawk;

namespace NeoRadar.SamplePlugin.SquawkDemo;

/// <summary>A demo provider: the next code in its range, whatever the aircraft.</summary>
internal sealed class SampleSquawkProvider : ISquawkProvider
{
    public const string Name = "Sample";

    private long _next = -1;

    public string ProviderName => Name;

    public int Priority => 1;

    // Off the UI and render threads, perhaps several at once.
    public Task<string?> GenerateSquawkAsync(string callsign, Aircraft aircraft, Flightplan? flightplan, CancellationToken cancellationToken)
        => Task.FromResult<string?>(DemoSquawkCodes.At(Interlocked.Increment(ref _next)));
}
