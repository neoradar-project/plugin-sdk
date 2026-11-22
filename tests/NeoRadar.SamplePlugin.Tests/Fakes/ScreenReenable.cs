using System.Numerics;
using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Tests.Fakes;

// Draws a frame, turns the screen off and on as the host does, draws again, and returns the live object counts.
internal static class ScreenReenable
{
    public static (int Before, int After) Count(IPluginScreenRenderer renderer, RecordingGraphics graphics)
    {
        FrameInfo frame = new(0.016, 1, null, new Vector2(800, 600));
        renderer.OnFrameUpdate(graphics, frame);
        renderer.OnFrameUpdate(graphics, frame with { FrameNumber = 2 });
        int before = graphics.Live.Count;

        renderer.OnDisabled();
        renderer.OnEnabled();
        renderer.OnFrameUpdate(graphics, frame with { FrameNumber = 3 });
        renderer.OnFrameUpdate(graphics, frame with { FrameNumber = 4 });

        return (before, graphics.Live.Count);
    }
}
