using System.Linq;
using NeoRadar.PluginSdk.Chat;
using NeoRadar.SamplePlugin.Commands;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.Commands;

public sealed class CommandsFeatureTests
{
    private readonly FakeCoreApi _core = new();

    public CommandsFeatureTests()
        => new CommandsFeature(_core.ChatFake, _core.AircraftFake, _core.LoggerFake, new PluginEventBus(_core.LoggerFake)).Initialize();

    [Fact]
    public void TheSwitch_DeclaresOnAndOff_AndAnswersWithTheValueTheHostPassed()
    {
        CommandParameter state = _core.ChatFake.Commands.Values.Single(c => c.Name == "sampleSwitch").Definition.Parameters.Single();

        Assert.Equal(["on", "off"], state.AllowedValues!);
        Assert.Equal("Sample switch is on (received 'on')", _core.ChatFake.Run("sampleSwitch", "on").Message);
    }

    [Fact]
    public void TheCallsignCommand_DeclaresAPattern()
    {
        CommandParameter callsign = _core.ChatFake.Commands.Values.Single(c => c.Name == "sampleCallsign").Definition.Parameters.Single();

        Assert.Equal(CallsignPatternCommand.CallsignPattern, callsign.Pattern);
    }
}
