using System;
using System.Numerics;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;

namespace NeoRadar.SamplePlugin.StatusBadge;

internal sealed class StatusBadgeRenderer : IPluginScreenRenderer
{
    private const float BadgeWidth = 180f;
    private const float BadgeHeight = 26f;
    private const float MarginRight = 12f;
    private const float MarginTop = 12f;

    private const float BadgeOpacity = 0.85f;

    private readonly ICoreApi _core;
    private readonly ThemeColours _colours;
    private GraphicsObjectRef _bg;
    private GraphicsObjectRef _text;
    private int _shownCount = -1;
    private float _lastWidth = -1f;
    private int _themeVersion = -1;

    // Written on client threads, read on the render thread.
    private volatile int _aircraftCount;

    public StatusBadgeRenderer(ICoreApi core, ThemeColours colours)
    {
        _core = core;
        _colours = colours;
    }

    /// <summary>Reads the count on the caller's thread; the next frame shows it.</summary>
    public void Refresh() => _aircraftCount = _core.Aircraft.GetAll().Count;

    public void OnFrameUpdate(IPluginGraphics g, FrameInfo info)
    {
        float width = MathF.Round(info.ScreenSize.X);
        if (MathF.Abs(width - _lastWidth) > 0.5f)
        {
            _lastWidth = width;
            RebuildBadge(g, width);
        }

        ThemeSnapshot theme = _colours.Current;
        if (theme.Version != _themeVersion)
        {
            _themeVersion = theme.Version;
            g.SetColor(_bg, theme.Surface with { A = BadgeOpacity });
            g.SetColor(_text, theme.Text);
        }

        int count = _aircraftCount;
        if (count == _shownCount) return;
        _shownCount = count;
        g.SetText(_text, $"Aircraft: {count}");
    }

    private void RebuildBadge(IPluginGraphics g, float width)
    {
        if (_bg.IsValid) g.Remove(_bg);
        if (_text.IsValid) g.Remove(_text);

        float x = MathF.Round(width - BadgeWidth - MarginRight);
        ThemeSnapshot theme = _colours.Current;
        _bg = g.AddScreenRect(new Rect(x, MarginTop, BadgeWidth, BadgeHeight), theme.Surface with { A = BadgeOpacity });
        float anchorX = MathF.Round(x + 10f);
        float anchorY = MathF.Round(MarginTop + BadgeHeight * 0.5f + 4f);
        _text = g.AddScreenText(new Vector2(anchorX, anchorY), " ", theme.Text, fontSize: 11f);
        _shownCount = -1;
        _themeVersion = theme.Version;
    }
}
