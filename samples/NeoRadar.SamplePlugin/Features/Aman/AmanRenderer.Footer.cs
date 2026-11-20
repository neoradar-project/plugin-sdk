using System;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;
using L = NeoRadar.SamplePlugin.Features.Aman.AmanLayout;

namespace NeoRadar.SamplePlugin.Features.Aman;

internal sealed partial class AmanRenderer
{
    private void DrawFooterFrame(float w, float h)
    {
        if (_graphics is null) return;
        float top = h - L.FooterHeight;
        _chromeRefs.Add(_graphics.AddScreenRect(new Rect(0, top, w, L.FooterHeight), P.PanelBg));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(0, top), new Vector2(w, top)], P.SeparatorLine, width: 1f));

        float baseline = MathF.Round(top + L.FooterHeight * 0.5f + L.FooterFontSize * 0.35f);
        _clockRef = _graphics.AddScreenText(
            new Vector2(6f, baseline), "--:--:--", P.TextWhite, fontSize: L.FooterFontSize);
        _graphics.SetHitTestVisible(_clockRef, false);
        _chromeRefs.Add(_clockRef);
        _lastClockText = "--:--:--";

        _statusItems = new (GraphicsObjectRef, GraphicsObjectRef)[L.StatusKinds.Length];
        float x = 70f;
        for (int i = 0; i < L.StatusKinds.Length; i++)
        {
            RgbaColor col = i switch
            {
                0 => P.StatusOk,
                1 => P.StatusOk,
                2 => P.StatusLoading,
                3 => P.StatusError,
                _ => P.StatusInactive,
            };
            GraphicsObjectRef dot = _graphics.AddScreenRect(
                new Rect(x, top + (L.FooterHeight - 8f) * 0.5f, 8f, 8f), col);
            _graphics.SetHitTestVisible(dot, false);
            _chromeRefs.Add(dot);
            GraphicsObjectRef label = _graphics.AddScreenText(
                new Vector2(x + 12f, baseline), L.StatusKinds[i], P.TextWhite, fontSize: L.FooterFontSize);
            _graphics.SetHitTestVisible(label, false);
            _chromeRefs.Add(label);
            _statusItems[i] = (dot, label);
            x += 12f + L.StatusKinds[i].Length * P.MonoCharWidth + 12f;
        }
    }

    private void UpdateClock(DateTimeOffset now)
    {
        if (_graphics is null || !_clockRef.IsValid) return;
        string s = P.FormatHhMmSs(now);
        if (s == _lastClockText) return;
        _graphics.SetText(_clockRef, s);
        _lastClockText = s;
    }
}
