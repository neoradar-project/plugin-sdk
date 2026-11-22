using System.Numerics;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Tests.Fakes;
using NeoRadar.SamplePlugin.WarningZone;
using Xunit;

namespace NeoRadar.SamplePlugin.Tests.WarningZone;

public sealed class WarningCircleRendererTests
{
    [Fact]
    public void TurnedOffAndOn_KeepsItsObjects_AndDrawsNoSecondCopy()
    {
        WarningCircleRenderer renderer = new(new NeoRadar.SamplePlugin.Common.ThemeColours());

        (int before, int after) = ScreenReenable.Count(renderer, new RecordingGraphics());

        Assert.True(before > 0);
        Assert.Equal(before, after);
    }

    [Fact]
    public void TheZone_IsDecorative_SoAPressOnItReachesTheMapAndItsTools()
    {
        WarningCircleRenderer renderer = new(new NeoRadar.SamplePlugin.Common.ThemeColours());
        RecordingGraphics graphics = new();

        renderer.OnFrameUpdate(graphics, new FrameInfo(0.016, 1, null, new Vector2(800, 600)));

        GraphicsObjectRef ring = Assert.Single(graphics.Live.Keys);
        Assert.Contains(ring, graphics.NotHitTestable);
        Assert.False(((IPluginScreenRenderer)renderer).OnPointerPressed(new PointerEvent(Vector2.Zero, Vector2.Zero, PointerButton.Left, KeyModifiers.None, 1, ring)));
    }
}
