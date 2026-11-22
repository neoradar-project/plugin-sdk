using System;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;
using L = NeoRadar.SamplePlugin.Aman.AmanLayout;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;

namespace NeoRadar.SamplePlugin.Aman;

internal sealed partial class AmanRenderer
{
    private void DrawScrollbarTrack(float h)
    {
        if (_graphics is null) return;
        float top = L.TopBarHeight;
        float bottom = h - L.FooterHeight;
        float availH = bottom - top;

        GraphicsObjectRef track = _graphics.AddScreenRect(
            new Rect(0, top, L.ScrollbarWidth, availH), P.ScrollbarTrack);
        _graphics.SetHitTestVisible(track, false);
        _chromeRefs.Add(track);

        _sbTrackTop = top;
        _sbTrackBottom = top + availH;
        _sbHandleHeight = availH * 0.6f;
        float handleTop = ComputeHandleTopFromOffset();

        _sbHandleFill = _graphics.AddScreenRect(
            new Rect(2f, handleTop, L.ScrollbarWidth - 4f, _sbHandleHeight), P.ScrollbarHandle);
        _buttonHits[_sbHandleFill] = L.ButtonId.ScrollHandle;
        _chromeRefs.Add(_sbHandleFill);

        _sbHandleStroke = _graphics.AddScreenStrokeRect(
            new Rect(2f, handleTop, L.ScrollbarWidth - 4f, _sbHandleHeight), P.ScrollbarHandleBorder, width: 1f);
        _graphics.SetHitTestVisible(_sbHandleStroke, false);
        _chromeRefs.Add(_sbHandleStroke);

        _sbGripTop = _graphics.AddScreenRect(
            new Rect(2f, handleTop, L.ScrollbarWidth - 4f, 4f), P.ScrollbarHandleBorder);
        _graphics.SetHitTestVisible(_sbGripTop, false);
        _chromeRefs.Add(_sbGripTop);

        _sbGripBottom = _graphics.AddScreenRect(
            new Rect(2f, handleTop + _sbHandleHeight - 4f, L.ScrollbarWidth - 4f, 4f), P.ScrollbarHandleBorder);
        _graphics.SetHitTestVisible(_sbGripBottom, false);
        _chromeRefs.Add(_sbGripBottom);
    }

    private float ComputeHandleTopFromOffset()
    {
        float minTop = _sbTrackTop;
        float maxTop = _sbTrackBottom - _sbHandleHeight;
        float t = 1f - ((_scrollOffsetMin + L.ScrollOffsetRange) / (2f * L.ScrollOffsetRange));
        return minTop + (maxTop - minTop) * Math.Clamp(t, 0f, 1f);
    }

    private void MoveScrollHandleVisuals(float newTop)
    {
        if (_graphics is null) return;
        _graphics.SetScreenAnchor(_sbHandleFill, new Vector2(2f, newTop));
        _graphics.SetScreenAnchor(_sbHandleStroke, new Vector2(2f, newTop));
        _graphics.SetScreenAnchor(_sbGripTop, new Vector2(2f, newTop));
        _graphics.SetScreenAnchor(_sbGripBottom, new Vector2(2f, newTop + _sbHandleHeight - 4f));
    }
}
