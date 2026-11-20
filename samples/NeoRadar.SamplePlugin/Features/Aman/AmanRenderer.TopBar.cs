using System;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;
using L = NeoRadar.SamplePlugin.Features.Aman.AmanLayout;

namespace NeoRadar.SamplePlugin.Features.Aman;

internal sealed partial class AmanRenderer
{
    private void DrawTopBar(float w)
    {
        if (_graphics is null) return;

        _chromeRefs.Add(_graphics.AddScreenRect(new Rect(0, 0, w, L.TopBarHeight), P.PanelBg));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(0, L.TopBarSeparatorY), new Vector2(w, L.TopBarSeparatorY)],
            P.SeparatorLine, width: 1f));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(0, L.TopBarHeight), new Vector2(w, L.TopBarHeight)],
            P.SeparatorLine, width: 1f));

        float y = 3f;
        float x = 5f;
        (GraphicsObjectRef bg, GraphicsObjectRef text, float endX) ns =
            AddRowButton("NonSeq", x, y, _nonSeqCount > 0, L.ButtonId.NonSeq, fixedWidth: 92f);
        _nonSeqBgRef = ns.bg;
        _nonSeqTextRef = ns.text;
        _lastNonSeqText = "NonSeq";
        _lastNonSeqAlert = _nonSeqCount > 0;
        x = ns.endX + 5f;
        x = AddRowButton("TLM", x, y, false, L.ButtonId.Tlm).endX + 5f;
        AddCheckbox("Departures", x, y, _showDepartures, L.ButtonId.Departures);

        float modeY = L.TopBarRow1Height + 6f;
        float modeX = 6f;
        for (int i = 0; i < L.RunwayModes.Length; i++)
        {
            string m = L.RunwayModes[i];
            RgbaColor col = L.RunwayActive[i] ? P.TextWhite : P.TextGrey;
            _chromeRefs.Add(_graphics.AddScreenText(
                new Vector2(MathF.Round(modeX), MathF.Round(modeY + L.TopBarFontSize * 0.85f)),
                m, col, fontSize: L.TopBarFontSize));
            modeX += m.Length * P.MonoCharWidth + 12f;
        }
    }

    private (GraphicsObjectRef bg, GraphicsObjectRef text, float endX) AddRowButton(
        string label, float x, float y, bool alert, L.ButtonId id, float fixedWidth = -1f)
    {
        if (_graphics is null) return (default, default, x);
        float textW = label.Length * P.MonoCharWidth;
        float bw = fixedWidth > 0f ? fixedWidth : MathF.Max(56f, textW + 18f);
        float bh = 18f;
        RgbaColor bg = alert ? P.NonSeqAlertBg : P.WindowBg;
        RgbaColor fg = alert ? P.NonSeqAlertFg : P.TextWhite;

        GraphicsObjectRef bgRef = _graphics.AddScreenRect(new Rect(x, y, bw, bh), bg);
        _chromeRefs.Add(bgRef);
        _buttonHits[bgRef] = id;
        GraphicsObjectRef stroke = _graphics.AddScreenStrokeRect(new Rect(x, y, bw, bh), P.SeparatorLine, width: 1f);
        _graphics.SetHitTestVisible(stroke, false);
        _chromeRefs.Add(stroke);
        GraphicsObjectRef textRef = _graphics.AddScreenText(
            new Vector2(MathF.Round(x + (bw - textW) * 0.5f),
                        MathF.Round(y + bh * 0.5f + L.TopBarFontSize * 0.35f)),
            label, fg, fontSize: L.TopBarFontSize);
        _graphics.SetHitTestVisible(textRef, false);
        _chromeRefs.Add(textRef);
        return (bgRef, textRef, x + bw);
    }

    private void UpdateNonSeqButton()
    {
        if (_graphics is null || !_nonSeqBgRef.IsValid) return;
        bool alert = _nonSeqCount > 0;
        string text = _nonSeqCount > 0 ? $"NonSeq ({_nonSeqCount})" : "NonSeq";
        _graphics.SetText(_nonSeqTextRef, text);
        _graphics.SetColor(_nonSeqBgRef, alert ? P.NonSeqAlertBg : P.PanelBg);
        _graphics.SetColor(_nonSeqTextRef, alert ? P.NonSeqAlertFg : P.TextWhite);
    }

    private void AddCheckbox(string label, float x, float y, bool @checked, L.ButtonId id)
    {
        if (_graphics is null) return;
        float boxSize = 14f;
        float labelW = label.Length * P.MonoCharWidth;
        float hitWidth = boxSize + 6f + labelW + 4f;
        float hitHeight = 18f;

        GraphicsObjectRef hit = _graphics.AddScreenRect(
            new Rect(x, y, hitWidth, hitHeight), new RgbaColor(0f, 0f, 0f, 0f));
        _chromeRefs.Add(hit);
        _buttonHits[hit] = id;

        float boxY = y + 2f;
        GraphicsObjectRef stroke = _graphics.AddScreenStrokeRect(new Rect(x, boxY, boxSize, boxSize), P.TextDim, width: 1f);
        _graphics.SetHitTestVisible(stroke, false);
        _chromeRefs.Add(stroke);
        if (@checked)
        {
            GraphicsObjectRef tick = _graphics.AddScreenRect(new Rect(x + 3f, boxY + 3f, boxSize - 6f, boxSize - 6f), P.TextWhite);
            _graphics.SetHitTestVisible(tick, false);
            _chromeRefs.Add(tick);
        }
        GraphicsObjectRef labelRef = _graphics.AddScreenText(
            new Vector2(MathF.Round(x + boxSize + 6f), MathF.Round(y + 9f + L.TopBarFontSize * 0.35f)),
            label, P.TextWhite, fontSize: L.TopBarFontSize);
        _graphics.SetHitTestVisible(labelRef, false);
        _chromeRefs.Add(labelRef);
    }
}
