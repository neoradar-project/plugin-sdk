using System;
using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using Ac = NeoRadar.PluginSdk.Aircraft.Aircraft;

namespace NeoRadar.SamplePlugin.MiniMap;

internal sealed class MiniMapRenderer : IPluginScreenRenderer
{
    private const double ArrivalLat = 51.4775;
    private const double ArrivalLon = -0.4614;
    private const float NmToMetres = 1852f;
    private const float LabelOffsetMetres = 12_000f;

    private const float BackgroundOpacity = 0.55f;
    private const float PanelOpacity = 0.94f;
    private static readonly RgbaColor LineDim = new(0.32f, 0.40f, 0.45f, 0.82f);
    private static readonly RgbaColor MarkerColor = new(1f, 0.85f, 0.20f, 1f);
    private static readonly RgbaColor SelectedMarkerColor = new(0.20f, 0.90f, 1f, 1f);
    private static readonly RgbaColor LabelColor = new(1f, 1f, 1f, 0.88f);
    private static readonly RgbaColor Green = new(0.25f, 0.82f, 0.44f, 1f);
    private static readonly RgbaColor Amber = new(1.00f, 0.70f, 0.22f, 1f);
    private static readonly RgbaColor Red = new(1.00f, 0.34f, 0.28f, 1f);
    private static readonly RgbaColor Cyan = new(0.18f, 0.78f, 0.88f, 1f);

    private sealed class AircraftItem
    {
        public required string Callsign { get; init; }
        public required Vector2 BaseWorld { get; init; }
        public required GraphicsObjectRef Marker { get; init; }
        public required GraphicsObjectRef Leader { get; init; }
        public required GraphicsObjectRef Trail { get; init; }
        public required GraphicsObjectRef Label { get; init; }
    }

    private readonly ICoreApi _core;
    private readonly ThemeColours _colours;
    private IPluginGraphics? _graphics;
    private readonly List<GraphicsObjectRef> _screenRefs = new();
    private readonly List<GraphicsObjectRef> _staticWorldRefs = new();
    private readonly List<GraphicsObjectRef> _buttonRefs = new();
    private readonly List<GraphicsObjectRef> _buttonStrokeRefs = new();
    private readonly Dictionary<GraphicsObjectRef, string> _markerOwners = new();
    private readonly Dictionary<string, AircraftItem> _aircraftItems = new(StringComparer.OrdinalIgnoreCase);
    private volatile bool _dirty = true;
    private bool _chromeDirty = true;
    private bool _staticWorldDirty = true;
    private Vector2 _lastSize;
    private int _lastTrackCount;
    private ScreenHandle _screenHandle;
    private bool _showLabels = true;
    private bool _showTrails = true;
    private string? _selectedCallsign;
    private int _themeVersion = -1;

    public MiniMapRenderer(ICoreApi core, ThemeColours colours)
    {
        _core = core;
        _colours = colours;
    }

    public void SetScreenHandle(ScreenHandle handle) => _screenHandle = handle;
    public void MarkDirty() => _dirty = true;

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
        if (_colours.Current.Version != _themeVersion) _chromeDirty = true;

        if (_chromeDirty) RebuildScreenChrome();
        if (!_dirty) return;
        _dirty = false;
        RebuildWorldScene();
    }

    // Takes the wheel only with Control held, so a plain scroll still zooms the host map; the log line shows each step.
    public bool OnPointerWheel(PointerWheelEvent e)
    {
        _core.Logger.Info($"Mini-map wheel dy={e.Delta.Y} modifiers={e.Modifiers}");
        return e.Modifiers.HasFlag(KeyModifiers.Control);
    }

    public bool OnPointerPressed(PointerEvent e)
    {
        if (e.Hit is null) return false;
        for (int i = 0; i < _buttonRefs.Count; i++)
        {
            if (!e.Hit.Value.Equals(_buttonRefs[i])) continue;
            if (i == 0) _showLabels = !_showLabels;
            else if (i == 1) _showTrails = !_showTrails;
            else _core.Rendering.GetInterface().CenterOnLatLon(_screenHandle, ArrivalLat, ArrivalLon, zoom: 8.2);
            UpdateButtonColors();
            ApplyVisibilityToggles();
            _dirty = true;
            return true;
        }
        if (_markerOwners.TryGetValue(e.Hit.Value, out string? callsign))
        {
            _selectedCallsign = callsign;
            _core.Logger.Info($"Mini-map selected {callsign}");
            _chromeDirty = true;
            _dirty = true;
            return true;
        }
        return false;
    }

    private void RebuildScreenChrome()
    {
        if (_graphics is null) return;
        foreach (GraphicsObjectRef r in _screenRefs) _graphics.Remove(r);
        _screenRefs.Clear();
        _buttonRefs.Clear();
        _buttonStrokeRefs.Clear();

        float w = MathF.Round(_lastSize.X);
        float h = MathF.Round(_lastSize.Y);
        ThemeSnapshot theme = _colours.Current;
        _themeVersion = theme.Version;
        GraphicsObjectRef background = _graphics.AddScreenRect(new Rect(0, 0, w, h), theme.Surface with { A = BackgroundOpacity });
        _graphics.SetHitTestVisible(background, false);
        _screenRefs.Add(background);
        DrawScreenChrome(w, h, _lastTrackCount);
        _chromeDirty = false;
    }

    private void RebuildWorldScene()
    {
        if (_graphics is null) return;
        Ac[] all;
        try { all = GetAircraftSnapshot(); }
        catch { return; }

        if (_staticWorldDirty) RebuildStaticWorld();
        if (_lastTrackCount != all.Length) _chromeDirty = true;
        _lastTrackCount = all.Length;

        HashSet<string> live = new(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < all.Length; i++)
        {
            Ac ac = all[i];
            live.Add(ac.Callsign);
            Vector2 worldPos = _graphics.LatLonToWorld(ac.Position.Latitude, ac.Position.Longitude);
            if (!_aircraftItems.TryGetValue(ac.Callsign, out AircraftItem? item))
                item = CreateAircraftItem(ac, worldPos);
            UpdateAircraftItem(item, ac, worldPos);
        }

        List<string> removed = [];
        foreach (string callsign in _aircraftItems.Keys)
            if (!live.Contains(callsign)) removed.Add(callsign);
        for (int i = 0; i < removed.Count; i++)
            if (_aircraftItems.Remove(removed[i], out AircraftItem? item))
                RemoveAircraftItem(item);
    }

    private void DrawScreenChrome(float w, float h, int count)
    {
        if (_graphics is null) return;
        _screenRefs.Add(_graphics.AddScreenRect(new Rect(0, 0, w, 34f), _colours.Current.SurfaceRaised with { A = PanelOpacity }));
        _screenRefs.Add(_graphics.AddScreenText(new Vector2(12f, 22f), "Arrival Mini Radar", LabelColor, fontSize: 13f));
        _screenRefs.Add(_graphics.AddScreenText(new Vector2(w - 84f, 22f), $"{count} tracks", Cyan, fontSize: 10f));

        AddButton(0, "LABELS", 12f, h - 32f, 66f, _showLabels);
        AddButton(1, "TRAILS", 84f, h - 32f, 66f, _showTrails);
        AddButton(2, "EGLL", w - 68f, h - 32f, 56f, true);

        float cx = w - 42f;
        float cy = 56f;
        _screenRefs.Add(_graphics.AddScreenLine([new Vector2(cx, cy - 18f), new Vector2(cx, cy + 18f)], Cyan, width: 1.4f));
        _screenRefs.Add(_graphics.AddScreenLine([new Vector2(cx - 18f, cy), new Vector2(cx + 18f, cy)], LineDim, width: 1.0f));
        _screenRefs.Add(_graphics.AddScreenText(new Vector2(cx - 4f, cy - 22f), "N", Cyan, fontSize: 10f));

        if (!string.IsNullOrEmpty(_selectedCallsign))
        {
            _screenRefs.Add(_graphics.AddScreenRect(new Rect(12f, 42f, 150f, 22f), new RgbaColor(0.06f, 0.15f, 0.18f, 0.92f)));
            _screenRefs.Add(_graphics.AddScreenText(new Vector2(20f, 57f), $"SELECTED {_selectedCallsign}", Cyan, fontSize: 10f));
        }
    }

    private void AddButton(int index, string label, float x, float y, float width, bool active)
    {
        if (_graphics is null) return;
        GraphicsObjectRef bg = _graphics.AddScreenRect(
            new Rect(x, y, width, 22f),
            active ? new RgbaColor(0.08f, 0.21f, 0.24f, 0.95f) : new RgbaColor(0.10f, 0.11f, 0.12f, 0.95f));
        _buttonRefs.Add(bg);
        _screenRefs.Add(bg);
        GraphicsObjectRef stroke = _graphics.AddScreenStrokeRect(new Rect(x, y, width, 22f), active ? Cyan : LineDim, width: 1f);
        _graphics.SetHitTestVisible(stroke, false);
        _buttonStrokeRefs.Add(stroke);
        _screenRefs.Add(stroke);
        _screenRefs.Add(_graphics.AddScreenText(new Vector2(x + 8f, y + 15f), label,
            active ? LabelColor : new RgbaColor(0.60f, 0.62f, 0.64f, 1f), fontSize: 9f));
        _ = index;
    }

    private void UpdateButtonColors()
    {
        if (_graphics is null) return;
        for (int i = 0; i < _buttonRefs.Count; i++)
        {
            bool active = i switch { 0 => _showLabels, 1 => _showTrails, _ => true };
            RgbaColor bg = active
                ? new RgbaColor(0.08f, 0.21f, 0.24f, 0.95f)
                : new RgbaColor(0.10f, 0.11f, 0.12f, 0.95f);
            _graphics.SetColor(_buttonRefs[i], bg);
            if (i < _buttonStrokeRefs.Count)
                _graphics.SetColor(_buttonStrokeRefs[i], active ? Cyan : LineDim);
        }
    }

    private void DrawArrivalGeometry()
    {
        if (_graphics is null) return;
        Vector2 arrival = _graphics.LatLonToWorld(ArrivalLat, ArrivalLon);
        _staticWorldRefs.Add(_graphics.AddWorldPoint(arrival, PointSymbol.Diamond, Green));
        _staticWorldRefs.Add(_graphics.AddWorldText(arrival + new Vector2(8_000f, 8_000f), "EGLL 27L", Green, fontSize: 10f));
        for (int i = 1; i <= 3; i++)
        {
            float radius = i * 20f * NmToMetres;
            _staticWorldRefs.Add(_graphics.AddWorldLine(BuildCircle(arrival, radius), LineDim, width: 1.0f));
        }
        Vector2 finalStart = arrival + new Vector2(-95f * NmToMetres, 0f);
        _staticWorldRefs.Add(_graphics.AddWorldLine([finalStart, arrival], Cyan, width: 1.5f));
        _staticWorldRefs.Add(_graphics.AddWorldLine(
            [arrival + new Vector2(-70f * NmToMetres, -24f * NmToMetres), arrival, arrival + new Vector2(-70f * NmToMetres, 24f * NmToMetres)],
            new RgbaColor(0.18f, 0.78f, 0.88f, 0.55f), width: 1.0f));
    }

    private void RebuildStaticWorld()
    {
        if (_graphics is null) return;
        foreach (GraphicsObjectRef r in _staticWorldRefs) _graphics.Remove(r);
        _staticWorldRefs.Clear();
        DrawArrivalGeometry();
        _staticWorldDirty = false;
    }

    private AircraftItem CreateAircraftItem(Ac ac, Vector2 worldPos)
    {
        if (_graphics is null) throw new InvalidOperationException("Mini-map graphics surface is not available.");
        Vector2 labelPos = worldPos + new Vector2(LabelOffsetMetres, LabelOffsetMetres);
        Vector2 trailStart = worldPos - TrailVector(ac);
        RgbaColor marker = MarkerColorFor(ac);
        AircraftItem item = new()
        {
            Callsign = ac.Callsign,
            BaseWorld = worldPos,
            Marker = _graphics.AddWorldPoint(worldPos, PointSymbol.Circle, marker),
            Leader = _graphics.AddWorldLine([worldPos, labelPos], new RgbaColor(marker.R, marker.G, marker.B, 0.40f), width: 0.8f),
            Trail = _graphics.AddWorldLine([trailStart, worldPos], new RgbaColor(0.42f, 0.62f, 0.68f, 0.58f), width: 1.0f),
            Label = _graphics.AddWorldText(labelPos, ac.Callsign, LabelColor, fontSize: 9f),
        };
        _graphics.SetHitTestVisible(item.Leader, false);
        _graphics.SetHitTestVisible(item.Trail, false);
        _graphics.SetHitTestVisible(item.Label, false);
        _aircraftItems[item.Callsign] = item;
        _markerOwners[item.Marker] = item.Callsign;
        return item;
    }

    private void UpdateAircraftItem(AircraftItem item, Ac ac, Vector2 worldPos)
    {
        if (_graphics is null) return;
        Transform2D move = Transform2D.Translate(worldPos - item.BaseWorld);
        _graphics.SetTransform(item.Marker, move);
        _graphics.SetTransform(item.Leader, move);
        _graphics.SetTransform(item.Trail, move);
        _graphics.SetTransform(item.Label, move);
        bool selected = string.Equals(_selectedCallsign, ac.Callsign, StringComparison.OrdinalIgnoreCase);
        RgbaColor marker = selected ? SelectedMarkerColor : MarkerColorFor(ac);
        _graphics.SetColor(item.Marker, marker);
        _graphics.SetColor(item.Leader, new RgbaColor(marker.R, marker.G, marker.B, 0.40f));
        _graphics.SetColor(item.Label, selected ? SelectedMarkerColor : LabelColor);
        _graphics.SetVisible(item.Label, _showLabels || selected);
        _graphics.SetVisible(item.Trail, _showTrails);
    }

    private void RemoveAircraftItem(AircraftItem item)
    {
        if (_graphics is null) return;
        _graphics.Remove(item.Marker);
        _graphics.Remove(item.Leader);
        _graphics.Remove(item.Trail);
        _graphics.Remove(item.Label);
        _markerOwners.Remove(item.Marker);
    }

    private void ApplyVisibilityToggles()
    {
        if (_graphics is null) return;
        foreach (AircraftItem item in _aircraftItems.Values)
        {
            bool selected = string.Equals(_selectedCallsign, item.Callsign, StringComparison.OrdinalIgnoreCase);
            _graphics.SetVisible(item.Label, _showLabels || selected);
            _graphics.SetVisible(item.Trail, _showTrails);
        }
    }

    private static RgbaColor MarkerColorFor(Ac ac)
        => ac.Position.Altitude < 6000 ? Amber
            : ac.Position.GroundSpeed < 180 ? Red
            : MarkerColor;

    private static Vector2 TrailVector(Ac ac)
    {
        float heading = ac.Position.TrackHeading != 0 ? ac.Position.TrackHeading : ac.Position.ReportedHeading;
        float radians = (heading - 90f) * MathF.PI / 180f;
        float length = MathF.Max(10_000f, MathF.Min(42_000f, ac.Position.GroundSpeed * 80f));
        return new Vector2(MathF.Cos(radians) * length, MathF.Sin(radians) * length);
    }

    private Ac[] GetAircraftSnapshot()
    {
        IReadOnlyList<Ac> all = _core.Aircraft.GetAll();
        Ac[] snapshot = new Ac[all.Count];
        for (int i = 0; i < all.Count; i++) snapshot[i] = all[i];
        Array.Sort(snapshot, static (a, b) => string.Compare(a.Callsign, b.Callsign, StringComparison.OrdinalIgnoreCase));
        return snapshot;
    }

    private static Vector2[] BuildCircle(Vector2 center, float radius)
    {
        const int Segments = 72;
        Vector2[] points = new Vector2[Segments + 1];
        for (int i = 0; i < Segments; i++)
        {
            float angle = MathF.PI * 2f * i / Segments;
            points[i] = new Vector2(
                center.X + MathF.Cos(angle) * radius,
                center.Y + MathF.Sin(angle) * radius);
        }
        points[Segments] = points[0];
        return points;
    }
}
