using System;
using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using Ac = NeoRadar.PluginSdk.Aircraft.Aircraft;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;

namespace NeoRadar.SamplePlugin.Tlm;

internal sealed class TlmRenderer : IPluginScreenRenderer
{
    private const float TopControlsHeight = 28f;
    private const float BottomScrollbarHeight = 18f;
    private const float ChartMarginLeft = 36f;
    private const float ChartMarginRight = 12f;
    private const float ChartMarginTop = 12f;
    private const float ChartMarginBottom = 24f;
    private const float ChartFontSize = 10f;
    private const float BucketBtnHeight = 18f;
    private const float ScrollOffsetRangeMin = 60f;
    private const int ScrollHandleHitId = -1;

    private static readonly int[] BucketChoicesMin = { 10, 30, 60 };

    private readonly ICoreApi _core;
    private IPluginGraphics? _graphics;
    private readonly List<GraphicsObjectRef> _chromeRefs = new();
    private readonly List<GraphicsObjectRef> _dynamicRefs = new();
    private readonly Dictionary<GraphicsObjectRef, int> _bucketBtnHits = new();
    private GraphicsObjectRef _bgRef = GraphicsObjectRef.Invalid;
    private GraphicsObjectRef _sbHandleFill;
    private GraphicsObjectRef _sbHandleStroke;
    private float _sbTrackLeft;
    private float _sbTrackRight;
    private float _sbHandleWidth;
    private float _sbHandleY;
    private float _scrollOffsetMin;
    private bool _draggingScroll;
    private float _dragStartScreenX;
    private float _dragStartHandleLeft;
    private int _bucketMin = 10;
    private volatile bool _dirty = true;
    private bool _chromeDirty = true;
    private Vector2 _lastSize;
    private string _lastSig = string.Empty;

    public TlmRenderer(ICoreApi core) { _core = core; }

    public void MarkDirty() => _dirty = true;

    public bool OnPointerPressed(PointerEvent e)
    {
        if (e.Hit is null) return false;
        if (!_bucketBtnHits.TryGetValue(e.Hit.Value, out int min)) return false;

        if (min == ScrollHandleHitId)
        {
            _draggingScroll = true;
            _dragStartScreenX = e.Screen.X;
            _dragStartHandleLeft = ComputeHandleLeftFromOffset();
            _graphics?.CapturePointer();
            return true;
        }

        if (_bucketMin != min)
        {
            _bucketMin = min;
            _chromeDirty = true;
            _dirty = true;
            _lastSig = string.Empty;
        }
        return true;
    }

    public void OnPointerMoved(PointerEvent e)
    {
        if (!_draggingScroll || _graphics is null) return;

        float minLeft = _sbTrackLeft;
        float maxLeft = _sbTrackRight - _sbHandleWidth;
        float newLeft = Math.Clamp(_dragStartHandleLeft + (e.Screen.X - _dragStartScreenX), minLeft, maxLeft);

        float t = (newLeft - minLeft) / Math.Max(1f, maxLeft - minLeft);
        _scrollOffsetMin = (2f * t - 1f) * ScrollOffsetRangeMin;

        MoveScrollHandleVisuals(newLeft);
        _dirty = true;
        _lastSig = string.Empty;
    }

    public void OnPointerReleased(PointerEvent e)
    {
        if (!_draggingScroll) return;
        _draggingScroll = false;
        _graphics?.ReleasePointer();
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
            _core.Logger.Error($"TLM render error: {ex}");
            _dirty = true;
            _lastSig = string.Empty;
        }
    }

    private void RebuildChrome()
    {
        if (_graphics is null) return;
        foreach (GraphicsObjectRef r in _chromeRefs) _graphics.Remove(r);
        _chromeRefs.Clear();
        _bucketBtnHits.Clear();

        float w = MathF.Round(_lastSize.X);
        float h = MathF.Round(_lastSize.Y);
        EnsureBackground(w, h);

        _chromeRefs.Add(_graphics.AddScreenRect(new Rect(0, 0, w, TopControlsHeight), P.PanelBg));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(0, TopControlsHeight), new Vector2(w, TopControlsHeight)], P.SeparatorLine, width: 1f));

        float btnX = 8f;
        float btnY = (TopControlsHeight - BucketBtnHeight) * 0.5f;
        for (int i = 0; i < BucketChoicesMin.Length; i++)
        {
            int min = BucketChoicesMin[i];
            string label = $"{min} min";
            float bw = 56f;
            bool active = min == _bucketMin;
            RgbaColor bg = active ? P.TitleBadgeBg : P.WindowBg;
            RgbaColor border = active ? P.TextWhite : P.SeparatorLine;
            GraphicsObjectRef hit = _graphics.AddScreenRect(new Rect(btnX, btnY, bw, BucketBtnHeight), bg);
            _chromeRefs.Add(hit);
            _bucketBtnHits[hit] = min;
            _chromeRefs.Add(_graphics.AddScreenStrokeRect(new Rect(btnX, btnY, bw, BucketBtnHeight), border, width: 1f));
            float textW = label.Length * P.MonoCharWidth;
            _chromeRefs.Add(_graphics.AddScreenText(
                new Vector2(MathF.Round(btnX + (bw - textW) * 0.5f),
                            MathF.Round(btnY + BucketBtnHeight * 0.5f + ChartFontSize * 0.35f)),
                label, P.TextWhite, fontSize: ChartFontSize));
            btnX += bw + 6f;
        }

        BuildScrollbar(w, h);
        _chromeDirty = false;
    }

    private void BuildScrollbar(float w, float h)
    {
        if (_graphics is null) return;
        float sbTop = h - BottomScrollbarHeight;
        _chromeRefs.Add(_graphics.AddScreenRect(new Rect(0, sbTop, w, BottomScrollbarHeight), P.PanelBg));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(0, sbTop), new Vector2(w, sbTop)], P.SeparatorLine, width: 1f));

        const float TrackInset = 6f;
        const float TrackHeight = 10f;
        float trackY = sbTop + 4f;
        float trackLeft = TrackInset;
        float trackRight = w - TrackInset;
        _sbTrackLeft = trackLeft;
        _sbTrackRight = trackRight;
        _sbHandleY = trackY;
        _sbHandleWidth = (trackRight - trackLeft) * 0.5f;

        GraphicsObjectRef track = _graphics.AddScreenRect(
            new Rect(trackLeft, trackY, trackRight - trackLeft, TrackHeight), P.ScrollbarTrack);
        _graphics.SetHitTestVisible(track, false);
        _chromeRefs.Add(track);

        float handleLeft = ComputeHandleLeftFromOffset();
        _sbHandleFill = _graphics.AddScreenRect(
            new Rect(handleLeft, trackY, _sbHandleWidth, TrackHeight), P.ScrollbarHandle);
        _bucketBtnHits[_sbHandleFill] = ScrollHandleHitId;
        _chromeRefs.Add(_sbHandleFill);

        _sbHandleStroke = _graphics.AddScreenStrokeRect(
            new Rect(handleLeft, trackY, _sbHandleWidth, TrackHeight), P.ScrollbarHandleBorder, width: 1f);
        _graphics.SetHitTestVisible(_sbHandleStroke, false);
        _chromeRefs.Add(_sbHandleStroke);
    }

    private float ComputeHandleLeftFromOffset()
    {
        float minLeft = _sbTrackLeft;
        float maxLeft = _sbTrackRight - _sbHandleWidth;
        float t = (_scrollOffsetMin + ScrollOffsetRangeMin) / (2f * ScrollOffsetRangeMin);
        return minLeft + (maxLeft - minLeft) * Math.Clamp(t, 0f, 1f);
    }

    private void MoveScrollHandleVisuals(float newLeft)
    {
        if (_graphics is null) return;
        _graphics.SetScreenAnchor(_sbHandleFill, new Vector2(newLeft, _sbHandleY));
        _graphics.SetScreenAnchor(_sbHandleStroke, new Vector2(newLeft, _sbHandleY));
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

        DateTimeOffset now = DateTimeOffset.UtcNow;
        int bucketSec = _bucketMin * 60;
        IReadOnlyList<Ac> all = _core.Aircraft.GetAll();
        string sig = $"{_bucketMin}|{now.ToUnixTimeSeconds() / Math.Max(60, bucketSec / 4)}|{all.Count}|{(int)MathF.Round(_scrollOffsetMin)}";
        if (sig == _lastSig) return;
        _lastSig = sig;

        foreach (GraphicsObjectRef r in _dynamicRefs) _graphics.Remove(r);
        _dynamicRefs.Clear();

        float w = MathF.Round(_lastSize.X);
        float h = MathF.Round(_lastSize.Y);
        float plotLeft = ChartMarginLeft;
        float plotRight = w - ChartMarginRight;
        float plotTop = TopControlsHeight + ChartMarginTop;
        float plotBottom = h - BottomScrollbarHeight - ChartMarginBottom;
        float plotWidth = plotRight - plotLeft;
        float plotHeight = plotBottom - plotTop;
        if (plotWidth <= 10f || plotHeight <= 10f) return;

        DateTimeOffset windowStart = AlignDown(now.AddMinutes(-_bucketMin + _scrollOffsetMin), bucketSec);
        int totalBuckets = Math.Max(4, (int)Math.Ceiling(120.0 / _bucketMin));
        DateTimeOffset windowEnd = windowStart.AddSeconds(bucketSec * (long)totalBuckets);
        int[] counts = new int[totalBuckets];
        for (int i = 0; i < all.Count; i++)
        {
            int eta = DemoFakery.EtaMinutesTo(all[i]);
            if (eta < 1) continue;
            DateTimeOffset arr = now.AddMinutes(eta);
            if (arr < windowStart || arr >= windowEnd) continue;
            int b = (int)((arr - windowStart).TotalSeconds / bucketSec);
            if (b >= 0 && b < counts.Length) counts[b]++;
        }

        int peak = 1;
        for (int i = 0; i < counts.Length; i++) peak = Math.Max(peak, counts[i]);
        peak = Math.Max(peak, 4);

        for (int v = 0; v <= peak; v++)
        {
            float y = plotBottom - plotHeight * v / (float)peak;
            _dynamicRefs.Add(_graphics.AddScreenLine(
                [new Vector2(plotLeft - 2f, y), new Vector2(plotLeft, y)], P.TextWhite, width: 1f));
            string lbl = v.ToString();
            float lblW = lbl.Length * P.MonoCharWidth;
            _dynamicRefs.Add(_graphics.AddScreenText(
                new Vector2(MathF.Round(plotLeft - lblW - 4f), MathF.Round(y + ChartFontSize * 0.35f)),
                lbl, P.TextWhite, fontSize: ChartFontSize));
        }

        _dynamicRefs.Add(_graphics.AddScreenLine(
            [new Vector2(plotLeft, plotBottom), new Vector2(plotRight, plotBottom)], P.TextWhite, width: 1f));
        _dynamicRefs.Add(_graphics.AddScreenLine(
            [new Vector2(plotLeft, plotTop), new Vector2(plotLeft, plotBottom)], P.TextWhite, width: 1f));

        float bucketPx = plotWidth / counts.Length;
        for (int i = 0; i < counts.Length; i++)
        {
            int c = counts[i];
            DateTimeOffset bucketTime = windowStart.AddSeconds(bucketSec * (long)i);
            float xLeft = plotLeft + bucketPx * i + 1f;
            float xRight = plotLeft + bucketPx * (i + 1) - 1f;
            float bw = xRight - xLeft;

            if (bucketTime.UtcDateTime.Minute % 15 == 0)
            {
                string xl = P.FormatHhMm(bucketTime);
                float xlW = xl.Length * P.MonoCharWidth;
                _dynamicRefs.Add(_graphics.AddScreenText(
                    new Vector2(MathF.Round(xLeft - xlW * 0.5f + bw * 0.5f),
                                MathF.Round(plotBottom + ChartFontSize + 4f)),
                    xl, P.TextWhite, fontSize: ChartFontSize));
                _dynamicRefs.Add(_graphics.AddScreenLine(
                    [new Vector2(xLeft + bw * 0.5f, plotBottom),
                     new Vector2(xLeft + bw * 0.5f, plotBottom + 3f)], P.TextWhite, width: 1f));
            }

            if (c <= 0) continue;
            float barH = plotHeight * c / (float)peak;
            float yTop = plotBottom - barH;
            DrawGradientBar(xLeft, yTop, bw, barH);

            string lbl = c.ToString();
            float lblW = lbl.Length * P.MonoCharWidth;
            _dynamicRefs.Add(_graphics.AddScreenText(
                new Vector2(MathF.Round(xLeft + (bw - lblW) * 0.5f),
                            MathF.Round(yTop + ChartFontSize + 2f)),
                lbl, P.TextWhite, fontSize: ChartFontSize));
        }
    }

    private void DrawGradientBar(float x, float y, float w, float h)
    {
        if (_graphics is null) return;
        const int Slices = 10;
        for (int i = 0; i < Slices; i++)
        {
            float t = i / (float)(Slices - 1);
            RgbaColor c = new(
                Lerp(P.TlmBarTop.R, P.TlmBarBottom.R, t),
                Lerp(P.TlmBarTop.G, P.TlmBarBottom.G, t),
                Lerp(P.TlmBarTop.B, P.TlmBarBottom.B, t),
                1f);
            float sliceY = y + h * i / Slices;
            float sliceH = h / Slices + 0.5f;
            GraphicsObjectRef rect = _graphics.AddScreenRect(new Rect(x, sliceY, w, sliceH), c);
            _graphics.SetHitTestVisible(rect, false);
            _dynamicRefs.Add(rect);
        }
        GraphicsObjectRef outline = _graphics.AddScreenStrokeRect(new Rect(x, y, w, h), P.WindowBg, width: 1f);
        _graphics.SetHitTestVisible(outline, false);
        _dynamicRefs.Add(outline);
    }

    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    private static DateTimeOffset AlignDown(DateTimeOffset t, int bucketSec)
    {
        long s = t.ToUnixTimeSeconds();
        long aligned = s / bucketSec * bucketSec;
        return DateTimeOffset.FromUnixTimeSeconds(aligned);
    }
}
