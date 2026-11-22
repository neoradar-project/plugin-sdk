using System.Collections.Generic;
using NeoRadar.PluginSdk.Controller;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

internal sealed class FakeControllerApi : IControllerApi
{
    public List<Controller> All { get; } = new();

    public IReadOnlyList<Controller> GetAll() => All.ToArray();

    public Controller? GetByCallsign(string callsign) => All.Find(c => c.Callsign == callsign);
}
