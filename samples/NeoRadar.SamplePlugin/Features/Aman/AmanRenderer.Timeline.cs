using System;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;
using L = NeoRadar.SamplePlugin.Features.Aman.AmanLayout;

namespace NeoRadar.SamplePlugin.Features.Aman;

internal sealed partial class AmanRenderer
{
    private float TimelineTop => L.TopBarHeight;
    private float TimelineBottom => MathF.Round(_lastSize.Y) - L.FooterHeight;
    private float TimelineHeight => TimelineBottom - TimelineTop;

    private void ComputeColumns(float w)
    {
        int n = L.TimelineIds.Length;
        float available = MathF.Max(60f, w - L.ScrollbarWidth);
        float perCol = available / n;
        _columns = new L.TimelineColumn[n];
        for (int i = 0; i < n; i++)
            _columns[i] = new L.TimelineColumn(i, L.ScrollbarWidth + i * perCol, perCol, L.TimelineIds[i]);
    }

    private void DrawScale(float w, float h)
    {
        if (_graphics is null) return;
        ComputeColumns(w);
        float top = TimelineTop;
        float bottom = TimelineBottom;

        _titleBadges = new (GraphicsObjectRef, GraphicsObjectRef, GraphicsObjectRef)[_columns.Length];

        for (int i = 0; i < _columns.Length; i++)
        {
            L.TimelineColumn col = _columns[i];
            _chromeRefs.Add(_graphics.AddScreenRect(
                new Rect(col.ScaleX, top, L.ScaleWidth, bottom - top), P.ScaleBg));
            _chromeRefs.Add(_graphics.AddScreenLine(
                [new Vector2(col.ScaleX, top), new Vector2(col.ScaleX, bottom)], P.ScaleBorder, width: 1f));
            _chromeRefs.Add(_graphics.AddScreenLine(
                [new Vector2(col.ScaleRight, top), new Vector2(col.ScaleRight, bottom)], P.ScaleBorder, width: 1f));
            if (i + 1 < _columns.Length)
            {
                _chromeRefs.Add(_graphics.AddScreenLine(
                    [new Vector2(col.Left + col.Width, top), new Vector2(col.Left + col.Width, bottom)],
                    P.SeparatorLine, width: 1f));
            }
            BuildTitleBadge(i, col);
        }
        _ = h;
    }

    private void BuildTitleBadge(int columnIndex, L.TimelineColumn col)
    {
        if (_graphics is null) return;
        float textW = col.Id.Length * P.MonoCharWidth;
        float w = MathF.Max(L.TitleBadgeWidth, textW + 24f);
        float left = MathF.Round((col.ScaleX + col.ScaleRight - w) * 0.5f);
        float top = MathF.Round(TimelineBottom - L.TitleBadgeHeight - 4f);
        GraphicsObjectRef bg = _graphics.AddScreenRect(new Rect(left, top, w, L.TitleBadgeHeight), P.TitleBadgeBg);
        _graphics.SetHitTestVisible(bg, false);
        GraphicsObjectRef stroke = _graphics.AddScreenStrokeRect(new Rect(left, top, w, L.TitleBadgeHeight), P.TitleBadgeFg, width: 1f);
        _graphics.SetHitTestVisible(stroke, false);
        GraphicsObjectRef text = _graphics.AddScreenText(
            new Vector2(MathF.Round(left + (w - textW) * 0.5f),
                        MathF.Round(top + L.TitleBadgeHeight * 0.5f + L.TitleFontSize * 0.35f)),
            col.Id, P.TitleBadgeFg, fontSize: L.TitleFontSize);
        _graphics.SetHitTestVisible(text, false);
        _chromeRefs.Add(bg);
        _chromeRefs.Add(stroke);
        _chromeRefs.Add(text);
        _titleBadges[columnIndex] = (bg, stroke, text);
    }

    private void DrawScaleTicks(L.TimelineColumn col, DateTimeOffset rangeStart, DateTimeOffset rangeEnd, float top, float height)
    {
        if (_graphics is null) return;
        float scaleX = col.ScaleX;
        float scaleRight = col.ScaleRight;

        DateTime aligned = rangeStart.UtcDateTime.AddSeconds(60 - rangeStart.UtcDateTime.Second);
        DateTimeOffset firstMinute = new(aligned.Year, aligned.Month, aligned.Day, aligned.Hour, aligned.Minute, 0, TimeSpan.Zero);

        for (DateTimeOffset t = firstMinute; t <= rangeEnd; t = t.AddMinutes(1))
        {
            float y = YForInstant(t, rangeStart, rangeEnd, top, height);
            int mins = t.UtcDateTime.Minute;
            bool is5 = mins % 5 == 0;
            bool is10 = mins % 10 == 0;
            float tickLen = is5 ? L.Tick5Min : L.Tick1Min;
            _dynamicRefs.Add(_graphics.AddScreenLine(
                [new Vector2(scaleX, y), new Vector2(scaleX + tickLen, y)], P.ScaleBorder, width: 1f));
            _dynamicRefs.Add(_graphics.AddScreenLine(
                [new Vector2(scaleRight, y), new Vector2(scaleRight - tickLen, y)], P.ScaleBorder, width: 1f));
            if (is5)
            {
                string label = is10 ? P.FormatHhMm(t) : P.FormatMm(t);
                float labelW = label.Length * P.MonoCharWidth;
                _dynamicRefs.Add(_graphics.AddScreenText(
                    new Vector2(MathF.Round(scaleX + (L.ScaleWidth - labelW) * 0.5f),
                                MathF.Round(y + L.TickFontSize * 0.35f)),
                    label, P.ScaleBorder, fontSize: L.TickFontSize));
            }
        }
    }

    private void DrawHourglass(float x, float y)
    {
        if (_graphics is null) return;
        float r = L.HourglassRadius;
        Vector2 p1 = new(x - r, y - r);
        Vector2 p2 = new(x + r, y + r);
        Vector2 p3 = new(x + r, y - r);
        Vector2 p4 = new(x - r, y + r);
        _dynamicRefs.Add(_graphics.AddScreenLine([p1, p2, p3, p4, p1], P.TextWhite, width: 1f));
    }

    private static float YForInstant(DateTimeOffset t, DateTimeOffset start, DateTimeOffset end, float top, float height)
    {
        double total = (end - start).TotalSeconds;
        double offset = (t - start).TotalSeconds;
        float pxPerSec = height / (float)total;
        return MathF.Round(top + height - pxPerSec * (float)offset);
    }
}
