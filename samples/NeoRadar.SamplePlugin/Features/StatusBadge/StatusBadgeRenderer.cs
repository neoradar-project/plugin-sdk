using System;
using System.Numerics;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.Features.StatusBadge;

internal sealed class StatusBadgeRenderer : IPluginScreenRenderer
{
    private const float BadgeWidth = 180f;
    private const float BadgeHeight = 26f;
    private const float MarginRight = 12f;
    private const float MarginTop = 12f;

    private static readonly RgbaColor BadgeBg = new(0.05f, 0.05f, 0.05f, 0.85f);
    private static readonly RgbaColor BadgeFg = new(0.95f, 0.95f, 0.95f, 1f);

    private readonly ICoreApi _core;
    private IPluginGraphics? _graphics;
    private GraphicsObjectRef _bg;
    private GraphicsObjectRef _text;
    private string _lastText = string.Empty;
    private int _lastCount = -1;
    private float _lastWidth = -1f;

    public StatusBadgeRenderer(ICoreApi core) { _core = core; }

    /// <summary>
    /// Host destroys the GraphicsContainer + adapter on disable; drop stale
    /// refs so RebuildBackground fires on the next OnFrameUpdate after re-enable.
    /// </summary>
    public void OnDisabled()
    {
        _bg = GraphicsObjectRef.Invalid;
        _text = GraphicsObjectRef.Invalid;
        _lastText = string.Empty;
        _lastCount = -1;
        _lastWidth = -1f;
    }

    public void OnFrameUpdate(IPluginGraphics g, FrameInfo info)
    {
        _graphics = g;
        float width = MathF.Round(info.ScreenSize.X);
        if (MathF.Abs(width - _lastWidth) > 0.5f)
        {
            _lastWidth = width;
            RebuildBackground(width);
        }
        RefreshText();
    }

    public void Refresh() => RefreshText();

    private void RebuildBackground(float width)
    {
        if (_graphics is null) return;
        if (_bg.IsValid) _graphics.Remove(_bg);
        float x = MathF.Round(width - BadgeWidth - MarginRight);
        _bg = _graphics.AddScreenRect(new Rect(x, MarginTop, BadgeWidth, BadgeHeight), BadgeBg);
        if (_text.IsValid)
        {
            _graphics.Remove(_text);
            _text = GraphicsObjectRef.Invalid;
        }
        _lastText = string.Empty;
    }

    private void RefreshText()
    {
        if (_graphics is null || _lastWidth < 0f) return;
        int count;
        try { count = _core.Aircraft.GetAll().Count; }
        catch { count = SamplePlugin.AircraftConnectedCount; }
        if (count == _lastCount && _text.IsValid) return;
        _lastCount = count;

        string desired = $"Aircraft: {count}";
        if (desired == _lastText && _text.IsValid) return;

        if (_text.IsValid) _graphics.Remove(_text);
        float anchorX = MathF.Round(_lastWidth - BadgeWidth - MarginRight + 10f);
        float anchorY = MathF.Round(MarginTop + BadgeHeight * 0.5f + 4f);
        _text = _graphics.AddScreenText(new Vector2(anchorX, anchorY), desired, BadgeFg, fontSize: 11f);
        _lastText = desired;
    }
}
