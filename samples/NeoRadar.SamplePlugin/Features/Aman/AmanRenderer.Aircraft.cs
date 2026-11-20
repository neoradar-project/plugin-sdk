using System;
using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using Ac = NeoRadar.PluginSdk.Aircraft.Aircraft;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;
using L = NeoRadar.SamplePlugin.Features.Aman.AmanLayout;

namespace NeoRadar.SamplePlugin.Features.Aman;

internal sealed partial class AmanRenderer
{
    private void DrawArrivalChain(Ac[] list, L.TimelineColumn col, DateTimeOffset rangeStart, DateTimeOffset rangeEnd,
        float top, float height, DateTimeOffset now)
    {
        if (_graphics is null) return;

        float scaleRight = col.ScaleRight;
        float labelLeft = col.LabelLeft;
        float maxLabelChars = MathF.Floor(col.LabelMaxWidth / P.MonoCharWidth);
        float labelWidth = MathF.Min(L.TotalLabelChars * P.MonoCharWidth, maxLabelChars * P.MonoCharWidth);
        float labelMidYOffset = L.TagLineHeight * 0.5f;
        float previousLabelTop = -1f;
        Ac? prev = null;
        DateTimeOffset prevEta = now;

        for (int i = 0; i < list.Length; i++)
        {
            Ac ac = list[i];
            int etaMin = DemoFakery.EtaMinutesTo(ac);
            DateTimeOffset eta = now.AddMinutes(etaMin);
            if (eta < rangeStart) continue;
            if (eta > rangeEnd) break;

            float dotY = YForInstant(eta, rangeStart, rangeEnd, top, height);
            if (dotY < top - 2f) break;

            TimeSpan timeBehind = prev is null ? TimeSpan.Zero : eta - prevEta;
            int ttg = ComputeTtgMinutes(etaMin);
            char wake = DemoFakery.GuessWake(ac.Callsign);

            float labelTopY = MathF.Round(dotY - L.TagLineHeight * 0.5f);
            float labelBottomY = labelTopY + L.TagLineHeight;
            if (previousLabelTop >= 0f && labelBottomY > previousLabelTop)
            {
                float diff = labelBottomY - previousLabelTop;
                labelTopY -= diff;
                labelBottomY -= diff;
            }
            if (labelBottomY < top) break;

            bool selected = string.Equals(_selectedCallsign, ac.Callsign, StringComparison.OrdinalIgnoreCase);
            RgbaColor lineCol = selected ? P.TextWhite : P.LabelLineOk;

            AircraftRefs refs = GetOrCreateAircraftRefs(ac, labelLeft, labelTopY, labelWidth, scaleRight, dotY, lineCol);
            refs.Touched = true;

            _graphics.SetScreenAnchor(refs.Hit, new Vector2(labelLeft - 2f, labelTopY - 1f));
            _graphics.SetScreenSize(refs.Hit, labelWidth + 4f, L.TagLineHeight + 2f);

            if (selected != refs.LastSelected)
            {
                _graphics.SetVisible(refs.Selection, selected);
                refs.LastSelected = selected;
            }
            if (selected)
            {
                _graphics.SetScreenAnchor(refs.Selection, new Vector2(labelLeft - 2f, labelTopY - 2f));
                _graphics.SetScreenSize(refs.Selection, labelWidth + 4f, L.TagLineHeight + 4f);
            }

            _graphics.SetScreenPoints(refs.Leader,
                new[] { new Vector2(scaleRight, dotY), new Vector2(labelLeft, labelTopY + labelMidYOffset) });
            _graphics.SetScreenAnchor(refs.Dot, new Vector2(scaleRight - L.DotRadius, dotY - L.DotRadius));

            if (!L.ColorEqual(refs.LastLineColor, lineCol))
            {
                _graphics.SetColor(refs.Leader, lineCol);
                _graphics.SetColor(refs.Dot, lineCol);
                refs.LastLineColor = lineCol;
            }

            UpdateLabelSegments(refs, ac, etaMin, timeBehind, ttg, wake, labelLeft, labelTopY);

            previousLabelTop = labelTopY - L.TagSeparation;
            prev = ac;
            prevEta = eta;
        }
    }

    private AircraftRefs GetOrCreateAircraftRefs(Ac ac, float labelLeft, float labelTopY, float labelWidth,
        float scaleRight, float dotY, RgbaColor lineCol)
    {
        if (_aircraftPool.TryGetValue(ac.Callsign ?? "", out AircraftRefs? existing))
            return existing;

        AircraftRefs refs = new();
        refs.Hit = _graphics!.AddScreenRect(
            new Rect(labelLeft - 2f, labelTopY - 1f, labelWidth + 4f, L.TagLineHeight + 2f),
            new RgbaColor(0f, 0f, 0f, 0f));
        _aircraftHits[refs.Hit] = ac.Callsign ?? "";

        refs.Selection = _graphics.AddScreenStrokeRect(
            new Rect(labelLeft - 2f, labelTopY - 2f, labelWidth + 4f, L.TagLineHeight + 4f),
            P.TextWhite, width: 1f);
        _graphics.SetHitTestVisible(refs.Selection, false);
        _graphics.SetVisible(refs.Selection, false);

        refs.Leader = _graphics.AddScreenLine(
            new[] { new Vector2(scaleRight, dotY), new Vector2(labelLeft, labelTopY + L.TagLineHeight * 0.5f) },
            lineCol, width: 1f);
        _graphics.SetHitTestVisible(refs.Leader, false);
        refs.Dot = _graphics.AddScreenRect(
            new Rect(scaleRight - L.DotRadius, dotY - L.DotRadius, L.DotRadius * 2f, L.DotRadius * 2f), lineCol);
        _graphics.SetHitTestVisible(refs.Dot, false);
        refs.LastLineColor = lineCol;

        refs.Segments = new GraphicsObjectRef[L.TagLayout.Length];
        refs.LastSegmentText = new string[L.TagLayout.Length];
        refs.LastSegmentColor = new RgbaColor[L.TagLayout.Length];
        for (int i = 0; i < L.TagLayout.Length; i++)
        {
            refs.Segments[i] = _graphics.AddScreenText(Vector2.Zero, " ", P.LabelArrivalFg, fontSize: L.LabelFontSize);
            _graphics.SetHitTestVisible(refs.Segments[i], false);
            refs.LastSegmentText[i] = " ";
            refs.LastSegmentColor[i] = P.LabelArrivalFg;
        }

        _aircraftPool[ac.Callsign ?? ""] = refs;
        return refs;
    }

    private void UpdateLabelSegments(AircraftRefs refs, Ac ac, int etaMin, TimeSpan timeBehind,
        int ttg, char wake, float x0, float y)
    {
        if (_graphics is null) return;
        float baseline = MathF.Round(y + L.TagLineHeight * 0.5f + L.LabelFontSize * 0.35f);
        float x = x0;
        double distNm = DemoFakery.DistanceNmTo(ac);
        for (int i = 0; i < L.TagLayout.Length; i++)
        {
            L.TagSpec spec = L.TagLayout[i];
            string raw = spec.Source switch
            {
                "callsign" => ac.Callsign ?? "",
                "runway" => "27L",
                "type" => DemoFakery.GuessType(ac.Callsign),
                "wake" => wake.ToString(),
                "ttg" => ttg > 0 ? $"+{ttg}" : ttg < 0 ? ttg.ToString() : "",
                "tbehind" => timeBehind > TimeSpan.Zero ? $"{(int)timeBehind.TotalMinutes:00}:{timeBehind.Seconds:00}" : "",
                "dist" => ((int)Math.Round(distNm)).ToString(),
                _ => "",
            };
            RgbaColor col = spec.Source switch
            {
                "wake" => P.WakeColor(wake),
                "ttg" when ttg > 0 => P.WakeHeavy,
                "ttg" when ttg < 0 => new RgbaColor(0.4f, 1f, 0.4f, 1f),
                _ => P.LabelArrivalFg,
            };
            string display = raw.Length > spec.Width ? raw.Substring(0, spec.Width) : raw;
            if (display.Length == 0) display = " ";

            if (refs.LastSegmentText[i] != display)
            {
                _graphics.SetText(refs.Segments[i], display);
                refs.LastSegmentText[i] = display;
            }
            if (!L.ColorEqual(refs.LastSegmentColor[i], col))
            {
                _graphics.SetColor(refs.Segments[i], col);
                refs.LastSegmentColor[i] = col;
            }
            _graphics.SetScreenAnchor(refs.Segments[i], new Vector2(MathF.Round(x), baseline));

            x += (spec.Width + 1) * P.MonoCharWidth;
        }
        _ = etaMin;
    }

    private void RemoveStaleAircraft()
    {
        if (_graphics is null) return;
        List<string>? toRemove = null;
        foreach ((string callsign, AircraftRefs refs) in _aircraftPool)
        {
            if (refs.Touched) continue;
            (toRemove ??= new List<string>()).Add(callsign);
        }
        if (toRemove is null) return;
        for (int i = 0; i < toRemove.Count; i++)
        {
            string callsign = toRemove[i];
            if (!_aircraftPool.Remove(callsign, out AircraftRefs? refs)) continue;
            _aircraftHits.Remove(refs.Hit);
            _graphics.Remove(refs.Hit);
            _graphics.Remove(refs.Selection);
            _graphics.Remove(refs.Leader);
            _graphics.Remove(refs.Dot);
            for (int s = 0; s < refs.Segments.Length; s++)
                _graphics.Remove(refs.Segments[s]);
        }
    }

    private static int ComputeTtgMinutes(int etaMinutes)
    {
        int slot = (etaMinutes / 4) * 4;
        return etaMinutes - slot - 2;
    }
}
