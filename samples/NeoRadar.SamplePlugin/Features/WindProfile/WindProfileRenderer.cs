using System;
using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Rendering;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;

namespace NeoRadar.SamplePlugin.Features.WindProfile;

internal sealed class WindProfileRenderer : IPluginScreenRenderer
{
    private const float DiagramMargin = 30f;
    private const float ReloadBarHeight = 30f;
    private const float TextFontSize = 10f;
    private const float ReloadFontSize = 10f;

    private readonly record struct Layer(int FlightLevelFt, int DirDeg, int SpeedKts, int TempC);
    private static readonly Layer[] Profile =
    {
        new(34000, 270, 95, -52),
        new(30000, 280, 80, -42),
        new(24000, 290, 65, -28),
        new(18000, 290, 45, -12),
        new(12000, 280, 30, -1),
        new(8000, 270, 22, 5),
        new(5000, 260, 18, 9),
        new(3000, 250, 14, 12),
        new(1500, 240, 10, 14),
        new(0, 230, 6, 16),
    };

    private readonly ICoreApi _core;
    private IPluginGraphics? _graphics;
    private readonly List<GraphicsObjectRef> _chromeRefs = new();
    private readonly List<GraphicsObjectRef> _dynamicRefs = new();
    private GraphicsObjectRef _bgRef = GraphicsObjectRef.Invalid;
    private GraphicsObjectRef _reloadHit = GraphicsObjectRef.Invalid;
    private bool _chromeDirty = true;
    private bool _dirty = true;
    private Vector2 _lastSize;
    private DateTimeOffset _validAt = DateTimeOffset.UtcNow;
    private string _lastSig = string.Empty;

    public WindProfileRenderer(ICoreApi core) { _core = core; }

    public void MarkDirty() => _dirty = true;

    /// <summary>
    /// Host destroys the GraphicsContainer + adapter on disable; drop stale
    /// refs so re-enable rebuilds chrome from scratch.
    /// </summary>
    public void OnDisabled()
    {
        _chromeRefs.Clear();
        _dynamicRefs.Clear();
        _bgRef = GraphicsObjectRef.Invalid;
        _reloadHit = GraphicsObjectRef.Invalid;
        _lastSize = Vector2.Zero;
        _lastSig = string.Empty;
        _chromeDirty = true;
        _dirty = true;
    }

    public bool OnPointerPressed(PointerEvent e)
    {
        if (e.Hit is null) return false;
        if (_reloadHit.IsValid && e.Hit.Value.Equals(_reloadHit))
        {
            _validAt = DateTimeOffset.UtcNow;
            _dirty = true;
            _lastSig = string.Empty;
            return true;
        }
        return false;
    }

    public void OnFrameUpdate(IPluginGraphics g, FrameInfo info)
    {
        _graphics = g;
        if (info.ScreenSize.X < 100f || info.ScreenSize.Y < 100f) return;
        if (info.ScreenSize != _lastSize)
        {
            _lastSize = info.ScreenSize;
            _chromeDirty = true;
            _dirty = true;
        }
        try
        {
            if (_chromeDirty) RebuildChrome();
            if (!_dirty) return;
            _dirty = false;
            RefreshDynamic();
        }
        catch (Exception ex)
        {
            _core.Logger.Error($"Wind render error: {ex}");
            _dirty = true;
            _lastSig = string.Empty;
        }
    }

    private void RebuildChrome()
    {
        if (_graphics is null) return;
        foreach (GraphicsObjectRef r in _chromeRefs) _graphics.Remove(r);
        _chromeRefs.Clear();

        float w = MathF.Round(_lastSize.X);
        float h = MathF.Round(_lastSize.Y);
        EnsureBackground(w, h);

        float reloadBarTop = h - ReloadBarHeight;
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(0, reloadBarTop), new Vector2(w, reloadBarTop)], P.SeparatorLine, width: 1f));
        const string reloadLabel = "↻ Reload winds";
        float btnW = 130f;
        float btnH = 20f;
        float btnX = 5f;
        float btnY = reloadBarTop + (ReloadBarHeight - btnH) * 0.5f;
        _reloadHit = _graphics.AddScreenRect(new Rect(btnX, btnY, btnW, btnH), P.PanelBg);
        _chromeRefs.Add(_reloadHit);
        _chromeRefs.Add(_graphics.AddScreenStrokeRect(new Rect(btnX, btnY, btnW, btnH), P.SeparatorLine, width: 1f));
        float lblW = reloadLabel.Length * P.MonoCharWidth;
        _chromeRefs.Add(_graphics.AddScreenText(
            new Vector2(MathF.Round(btnX + (btnW - lblW) * 0.5f),
                        MathF.Round(btnY + btnH * 0.5f + ReloadFontSize * 0.35f)),
            reloadLabel, P.TextWhite, fontSize: ReloadFontSize));

        _chromeDirty = false;
    }

    private void EnsureBackground(float w, float h)
    {
        if (_graphics is null) return;
        if (_bgRef.IsValid) _graphics.Remove(_bgRef);
        _bgRef = _graphics.AddScreenRect(new Rect(0, 0, w, h), P.WindowBg);
        _graphics.SetHitTestVisible(_bgRef, false);
    }

    private void RefreshDynamic()
    {
        if (_graphics is null) return;

        string sig = $"{_validAt.ToUnixTimeSeconds()}|{_lastSize.X}|{_lastSize.Y}";
        if (sig == _lastSig) return;
        _lastSig = sig;

        foreach (GraphicsObjectRef r in _dynamicRefs) _graphics.Remove(r);
        _dynamicRefs.Clear();

        float w = MathF.Round(_lastSize.X);
        float h = MathF.Round(_lastSize.Y);
        float reloadBarTop = h - ReloadBarHeight;
        float plotTop = DiagramMargin;
        float plotBottom = reloadBarTop - DiagramMargin;
        float plotHeight = plotBottom - plotTop;
        if (plotHeight <= 20f) return;

        int maxFl = 0;
        for (int i = 0; i < Profile.Length; i++) maxFl = Math.Max(maxFl, Profile[i].FlightLevelFt);
        if (maxFl <= 0) return;
        float pxPerFt = plotHeight / maxFl;

        for (int i = 0; i < Profile.Length; i++)
        {
            Layer layer = Profile[i];
            float yPos = MathF.Round(plotBottom - pxPerFt * layer.FlightLevelFt);

            _dynamicRefs.Add(_graphics.AddScreenLine(
                [new Vector2(0, yPos), new Vector2(10f, yPos)], P.TextWhite, width: 1f));

            float barbX = w - DiagramMargin - 20f;
            WindBarbs.Draw(_graphics, _dynamicRefs, barbX, yPos,
                layer.DirDeg, layer.SpeedKts,
                length: 40f, barbMaxLength: 12f, color: P.TextWhite);

            string flStr = (layer.FlightLevelFt / 100).ToString().PadLeft(3, '0');
            string dirStr = layer.DirDeg.ToString().PadLeft(3, '0');
            string text = $"FL{flStr}: {dirStr} / {layer.SpeedKts} kt / {layer.TempC} C";
            _dynamicRefs.Add(_graphics.AddScreenText(
                new Vector2(30f, MathF.Round(yPos + TextFontSize * 0.35f)),
                text, P.TextWhite, fontSize: TextFontSize));
        }

        string valid = $"Valid {_validAt.UtcDateTime:yyyy/MM/dd HH:mm}Z";
        _dynamicRefs.Add(_graphics.AddScreenText(
            new Vector2(5f, MathF.Round(reloadBarTop - 5f)), valid, P.TextWhite, fontSize: TextFontSize));
    }
}
