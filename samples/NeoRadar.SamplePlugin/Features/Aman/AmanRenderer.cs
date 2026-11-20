using System;
using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;
using L = NeoRadar.SamplePlugin.Features.Aman.AmanLayout;

namespace NeoRadar.SamplePlugin.Features.Aman;

internal sealed partial class AmanRenderer : IPluginScreenRenderer
{
    private sealed class AircraftRefs
    {
        public GraphicsObjectRef Hit;
        public GraphicsObjectRef Selection;
        public GraphicsObjectRef Leader;
        public GraphicsObjectRef Dot;
        public GraphicsObjectRef[] Segments = Array.Empty<GraphicsObjectRef>();
        public string[] LastSegmentText = Array.Empty<string>();
        public RgbaColor[] LastSegmentColor = Array.Empty<RgbaColor>();
        public RgbaColor LastLineColor;
        public bool LastSelected;
        public bool Touched;
    }

    private readonly ICoreApi _core;
    private IPluginGraphics? _graphics;

    private readonly List<GraphicsObjectRef> _chromeRefs = new();
    private readonly List<GraphicsObjectRef> _dynamicRefs = new();
    private readonly Dictionary<GraphicsObjectRef, string> _aircraftHits = new();
    private readonly Dictionary<string, AircraftRefs> _aircraftPool = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<GraphicsObjectRef, L.ButtonId> _buttonHits = new();

    private GraphicsObjectRef _bgRef = GraphicsObjectRef.Invalid;
    private GraphicsObjectRef _nonSeqBgRef;
    private GraphicsObjectRef _nonSeqTextRef;
    private GraphicsObjectRef _clockRef;
    private string _lastClockText = string.Empty;
    private string _lastNonSeqText = string.Empty;
    private bool _lastNonSeqAlert;

    private (GraphicsObjectRef bg, GraphicsObjectRef stroke, GraphicsObjectRef text)[] _titleBadges
        = Array.Empty<(GraphicsObjectRef, GraphicsObjectRef, GraphicsObjectRef)>();
    private (GraphicsObjectRef dot, GraphicsObjectRef label)[] _statusItems
        = Array.Empty<(GraphicsObjectRef, GraphicsObjectRef)>();

    private L.TimelineColumn[] _columns = Array.Empty<L.TimelineColumn>();

    private GraphicsObjectRef _sbHandleFill;
    private GraphicsObjectRef _sbHandleStroke;
    private GraphicsObjectRef _sbGripTop;
    private GraphicsObjectRef _sbGripBottom;
    private float _sbTrackTop;
    private float _sbTrackBottom;
    private float _sbHandleHeight;
    private float _scrollOffsetMin;
    private bool _draggingScroll;
    private float _dragStartScreenY;
    private float _dragStartHandleTop;

    private volatile bool _dirty = true;
    private bool _chromeDirty = true;
    private Vector2 _lastSize;
    private string? _selectedCallsign;
    private string _lastSignature = string.Empty;
    private bool _showDepartures;
    private int _nonSeqCount;

    public AmanRenderer(ICoreApi core) { _core = core; }

    public void MarkDirty() => _dirty = true;

    /// <summary>
    /// Host destroys the GraphicsContainer + adapter when the screen is
    /// disabled; on re-enable the next OnFrameUpdate sees a fresh adapter and
    /// every tracked ref we stored is stale. Drop them and force a full
    /// rebuild so chrome reappears.
    /// </summary>
    public void OnDisabled()
    {
        _chromeRefs.Clear();
        _dynamicRefs.Clear();
        _aircraftPool.Clear();
        _aircraftHits.Clear();
        _buttonHits.Clear();
        _bgRef = GraphicsObjectRef.Invalid;
        _nonSeqBgRef = default;
        _nonSeqTextRef = default;
        _clockRef = default;
        _titleBadges = Array.Empty<(GraphicsObjectRef, GraphicsObjectRef, GraphicsObjectRef)>();
        _statusItems = Array.Empty<(GraphicsObjectRef, GraphicsObjectRef)>();
        _sbHandleFill = default;
        _sbHandleStroke = default;
        _sbGripTop = default;
        _sbGripBottom = default;
        _lastClockText = string.Empty;
        _lastNonSeqText = string.Empty;
        _lastSignature = string.Empty;
        _lastSize = Vector2.Zero;
        _chromeDirty = true;
        _dirty = true;
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
            _core.Logger.Error($"AMAN render error: {ex}");
            _dirty = true;
            _lastSignature = string.Empty;
        }
    }

    public bool OnPointerPressed(PointerEvent e)
    {
        if (e.Hit is null) return false;

        if (_buttonHits.TryGetValue(e.Hit.Value, out L.ButtonId btn))
        {
            if (btn == L.ButtonId.ScrollHandle)
            {
                _draggingScroll = true;
                _dragStartScreenY = e.Screen.Y;
                _dragStartHandleTop = ComputeHandleTopFromOffset();
                _graphics?.CapturePointer();
                return true;
            }
            HandleButton(btn);
            return true;
        }

        if (_aircraftHits.TryGetValue(e.Hit.Value, out string? cs))
        {
            _selectedCallsign = string.Equals(_selectedCallsign, cs, StringComparison.OrdinalIgnoreCase) ? null : cs;
            _dirty = true;
            _lastSignature = string.Empty;
            return true;
        }

        return false;
    }

    public void OnPointerMoved(PointerEvent e)
    {
        if (!_draggingScroll || _graphics is null) return;

        float minTop = _sbTrackTop;
        float maxTop = _sbTrackBottom - _sbHandleHeight;
        float newTop = Math.Clamp(_dragStartHandleTop + (e.Screen.Y - _dragStartScreenY), minTop, maxTop);

        float t = (newTop - minTop) / Math.Max(1f, maxTop - minTop);
        _scrollOffsetMin = (1f - 2f * t) * L.ScrollOffsetRange;

        MoveScrollHandleVisuals(newTop);
        _dirty = true;
        _lastSignature = string.Empty;
    }

    public void OnPointerReleased(PointerEvent e)
    {
        if (!_draggingScroll) return;
        _draggingScroll = false;
        _graphics?.ReleasePointer();
    }

    private void HandleButton(L.ButtonId btn)
    {
        switch (btn)
        {
            case L.ButtonId.Departures:
                _showDepartures = !_showDepartures;
                _core.Logger.Info($"AMAN: showDepartures={_showDepartures}");
                _chromeDirty = true;
                break;
            case L.ButtonId.NonSeq:
                _core.Logger.Info("AMAN: NonSeq button - enable the Non-Sequenced screen via plugin settings");
                break;
            case L.ButtonId.Tlm:
                _core.Logger.Info("AMAN: TLM button - enable the Traffic Load Monitor screen via plugin settings");
                break;
        }
        _dirty = true;
        _lastSignature = string.Empty;
    }

    private void RebuildChrome()
    {
        if (_graphics is null) return;
        foreach (GraphicsObjectRef r in _chromeRefs) _graphics.Remove(r);
        _chromeRefs.Clear();
        _buttonHits.Clear();

        foreach (AircraftRefs refs in _aircraftPool.Values)
        {
            _graphics.Remove(refs.Hit);
            _graphics.Remove(refs.Selection);
            _graphics.Remove(refs.Leader);
            _graphics.Remove(refs.Dot);
            for (int s = 0; s < refs.Segments.Length; s++)
                _graphics.Remove(refs.Segments[s]);
        }
        _aircraftPool.Clear();
        _aircraftHits.Clear();
        _lastSignature = string.Empty;

        float w = MathF.Round(_lastSize.X);
        float h = MathF.Round(_lastSize.Y);
        if (w <= 0f || h <= 0f) return;

        EnsureBackground(w, h);
        DrawTopBar(w);
        DrawScrollbarTrack(h);
        DrawScale(w, h);
        DrawFooterFrame(w, h);

        _chromeDirty = false;
    }

    private void EnsureBackground(float w, float h)
    {
        if (_graphics is null) return;
        if (_bgRef.IsValid) _graphics.Remove(_bgRef);
        _bgRef = _graphics.AddScreenRect(new Rect(0, 0, w, h), P.WindowBg);
        _graphics.SetHitTestVisible(_bgRef, false);
    }
}
