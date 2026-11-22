using System;
using NeoRadar.PluginSdk.Squawk;
using NeoRadar.SamplePlugin.Common;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.SquawkDemo.Events;

namespace NeoRadar.SamplePlugin.SquawkDemo;

/// <summary>A squawk provider registered only when the controller asks, and the assignments it hears.</summary>
internal sealed class SquawkDemoFeature : IFeature, ISquawkDemo, ISquawkEventHandler
{
    private readonly ISquawkApi _squawk;
    private readonly DemoWritesGate _gate;
    private readonly IPluginEventBus _bus;
    private readonly RecentLines _activity = new(8);
    private readonly object _sync = new();
    private IDisposable? _registration;

    public SquawkDemoFeature(ISquawkApi squawk, DemoWritesGate gate, IPluginEventBus bus)
    {
        _squawk = squawk;
        _gate = gate;
        _bus = bus;
    }

    public void Initialize()
    {
    }

    // The host also drops the provider at unload; disposing the token is still the plugin's job.
    public void Shutdown() => Unregister();

    public void Register()
    {
        if (!_gate.IsOpen)
        {
            Record("Register: tick Demo writes first; nothing was registered");
            return;
        }

        string outcome;
        lock (_sync)
        {
            if (_registration is null)
            {
                _registration = _squawk.RegisterProviderWithToken(new SampleSquawkProvider());
                outcome = _registration is null ? "Registration refused (the name is taken)" : $"Registered '{SampleSquawkProvider.Name}'";
            }
            else
            {
                outcome = "Already registered";
            }
        }
        Record(outcome);
    }

    public void Unregister()
    {
        IDisposable? registration;
        lock (_sync)
        {
            registration = _registration;
            _registration = null;
        }
        if (registration is null) return;

        registration.Dispose();
        Record("Unregistered");
    }

    public void ListProviders() => Record($"Available providers: {string.Join(", ", _squawk.GetAvailableProviders())}");

    public void MakeActive()
    {
        if (!_gate.IsOpen)
        {
            Record("SetActiveProvider: tick Demo writes first; nothing was changed");
            return;
        }
        Record($"SetActiveProvider({SampleSquawkProvider.Name}) returned {_squawk.SetActiveProvider(SampleSquawkProvider.Name)}");
    }

    // Client thread.
    public void OnSquawkAssigned(SquawkAssignedEvent evt) => Record($"{evt.ProviderName} assigned {evt.Squawk} to {evt.Callsign}");

    private void Record(string line)
    {
        _activity.Add($"{DateTimeOffset.UtcNow:HH:mm:ss} {line}");
        _bus.Publish(new SquawkActivityRecorded(_activity.Snapshot()));
    }
}
