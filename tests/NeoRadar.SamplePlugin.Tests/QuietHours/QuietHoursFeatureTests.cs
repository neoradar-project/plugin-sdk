using System.Linq;
using NeoRadar.PluginSdk.Expressions;
using NeoRadar.SamplePlugin.Common.Events;
using NeoRadar.SamplePlugin.QuietHours;
using NeoRadar.SamplePlugin.Tests.Fakes;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.QuietHours;

public sealed class QuietHoursFeatureTests
{
    private readonly FakeCoreApi _core = new();
    private readonly QuietHoursFeature _feature;

    public QuietHoursFeatureTests()
    {
        _feature = new QuietHoursFeature(_core.ExpressionsFake, _core.FocusFake, new PluginEventBus(_core.LoggerFake));
        _feature.Initialize();
    }

    [Fact]
    public void QuietHours_IsASessionBoolean_AndFlagged_AnAircraftBoolean()
    {
        ExpressionDefinition quiet = _core.ExpressionsFake.Declared.Single(d => d.Name == QuietHoursFeature.QuietHoursName);
        ExpressionDefinition flagged = _core.ExpressionsFake.Declared.Single(d => d.Name == QuietHoursFeature.FlaggedName);

        Assert.Equal((ExpressionScope.Session, ExpressionValueType.Boolean), (quiet.Scope, quiet.Type));
        Assert.Equal((ExpressionScope.Aircraft, ExpressionValueType.Boolean), (flagged.Scope, flagged.Type));
    }

    [Fact]
    public void FlaggingTwice_PushesTrueThenFalse_ForTheSelectedAircraft()
    {
        _core.FocusFake.Selected = "BAW1";

        _feature.ToggleFlagOnSelected();
        _feature.ToggleFlagOnSelected();

        Assert.Equal(["plugin:Sample:EXP_Flagged BAW1 True", "plugin:Sample:EXP_Flagged BAW1 False"], _core.ExpressionsFake.Pushes);
    }
}
