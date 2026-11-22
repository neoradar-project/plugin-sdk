using System;
using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk;
using NeoRadar.PluginSdk.Aircraft;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using Ac = NeoRadar.PluginSdk.Aircraft.Aircraft;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;

namespace NeoRadar.SamplePlugin.NonSeq;

internal sealed class NonSeqRenderer : IPluginScreenRenderer
{
    private const float TitleBorderTop = 8f;
    private const float TitleBorderHeight = 14f;
    private const float HeaderRowHeight = 22f;
    private const float DataRowHeight = 20f;
    private const float Padding = 10f;
    private const float HeaderFontSize = 10f;
    private const float CellFontSize = 10f;
    private const float TitleFontSize = 11f;

    private static readonly (string Name, float Width)[] Columns =
    {
        ("Call Sign", 80f),
        ("A/C", 70f),
        ("WVC", 60f),
        ("Reason", 0f),
    };

    private static readonly string[] Reasons =
    {
        "No ETA",
        "Outside window",
        "Holding",
        "Diverted",
        "Off-route",
    };

    private sealed class RowRefs
    {
        public GraphicsObjectRef Tint;
        public GraphicsObjectRef Cell0, Cell1, Cell2, Cell3;
        public GraphicsObjectRef Divider;
        public string LastCs = "", LastType = "", LastWvc = "", LastReason = "";
        public RgbaColor LastWvcColor;
        public bool LastTintVisible;
        public bool Touched;
    }

    private readonly ICoreApi _core;
    private IPluginGraphics? _graphics;
    private readonly List<GraphicsObjectRef> _chromeRefs = new();
    private readonly Dictionary<string, RowRefs> _rowPool = new(StringComparer.OrdinalIgnoreCase);
    private GraphicsObjectRef _emptyTextRef = GraphicsObjectRef.Invalid;
    private GraphicsObjectRef _bgRef = GraphicsObjectRef.Invalid;
    private bool _chromeDirty = true;
    private bool _dirty = true;
    private Vector2 _lastSize;
    private string _lastSig = string.Empty;

    public NonSeqRenderer(ICoreApi core) { _core = core; }

    public void MarkDirty() => _dirty = true;

    public bool OnPointerPressed(PointerEvent e) => false;

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
            _core.Logger.Error($"NonSeq render error: {ex}");
            _dirty = true;
            _lastSig = string.Empty;
        }
    }

    private void RebuildChrome()
    {
        if (_graphics is null) return;
        foreach (GraphicsObjectRef r in _chromeRefs) _graphics.Remove(r);
        _chromeRefs.Clear();
        WipeRowPool();

        float w = MathF.Round(_lastSize.X);
        float h = MathF.Round(_lastSize.Y);
        EnsureBackground(w, h);

        float frameLeft = Padding * 0.5f;
        float frameTop = TitleBorderTop + TitleBorderHeight * 0.5f;
        float frameRight = w - Padding * 0.5f;
        float frameBottom = h - Padding * 0.5f;

        const string title = "Arrivals";
        float titleW = title.Length * P.MonoCharWidth;
        float titleX = frameLeft + 14f;

        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(frameLeft, frameTop), new Vector2(titleX - 4f, frameTop)],
            P.SeparatorLine, width: 1f));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(titleX + titleW + 4f, frameTop), new Vector2(frameRight, frameTop)],
            P.SeparatorLine, width: 1f));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(frameLeft, frameBottom), new Vector2(frameRight, frameBottom)],
            P.SeparatorLine, width: 1f));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(frameLeft, frameTop), new Vector2(frameLeft, frameBottom)],
            P.SeparatorLine, width: 1f));
        _chromeRefs.Add(_graphics.AddScreenLine(
            [new Vector2(frameRight, frameTop), new Vector2(frameRight, frameBottom)],
            P.SeparatorLine, width: 1f));
        _chromeRefs.Add(_graphics.AddScreenText(
            new Vector2(MathF.Round(titleX), MathF.Round(frameTop + TitleFontSize * 0.35f)),
            title, P.TextWhite, fontSize: TitleFontSize));

        float tableLeft = frameLeft + 6f;
        float tableTop = frameTop + 12f;
        float tableRight = frameRight - 6f;
        float tableWidth = tableRight - tableLeft;

        _chromeRefs.Add(_graphics.AddScreenRect(
            new Rect(tableLeft, tableTop, tableWidth, HeaderRowHeight), P.PanelBg));
        _chromeRefs.Add(_graphics.AddScreenStrokeRect(
            new Rect(tableLeft, tableTop, tableWidth, HeaderRowHeight), P.SeparatorLine, width: 1f));

        float[] colXs = ComputeColumnXs(tableLeft, tableWidth);
        for (int i = 0; i < Columns.Length; i++)
        {
            _chromeRefs.Add(_graphics.AddScreenText(
                new Vector2(MathF.Round(colXs[i] + 6f),
                            MathF.Round(tableTop + HeaderRowHeight * 0.5f + HeaderFontSize * 0.35f)),
                Columns[i].Name, P.TextWhite, fontSize: HeaderFontSize));
            if (i > 0)
            {
                _chromeRefs.Add(_graphics.AddScreenLine(
                    [new Vector2(colXs[i], tableTop),
                     new Vector2(colXs[i], tableTop + HeaderRowHeight)],
                    P.SeparatorLine, width: 1f));
            }
        }

        _chromeDirty = false;
    }

    private void EnsureBackground(float w, float h)
    {
        if (_graphics is null) return;
        if (_bgRef.IsValid) _graphics.Remove(_bgRef);
        _bgRef = _graphics.AddScreenRect(new Rect(0, 0, w, h), P.WindowBg);
        _graphics.SetHitTestVisible(_bgRef, false);
    }

    private void WipeRowPool()
    {
        if (_graphics is null) return;
        foreach (RowRefs r in _rowPool.Values)
        {
            _graphics.Remove(r.Tint);
            _graphics.Remove(r.Cell0);
            _graphics.Remove(r.Cell1);
            _graphics.Remove(r.Cell2);
            _graphics.Remove(r.Cell3);
            _graphics.Remove(r.Divider);
        }
        _rowPool.Clear();
        if (_emptyTextRef.IsValid)
        {
            _graphics.Remove(_emptyTextRef);
            _emptyTextRef = GraphicsObjectRef.Invalid;
        }
    }

    private float[] ComputeColumnXs(float tableLeft, float tableWidth)
    {
        float[] xs = new float[Columns.Length];
        float fixedSum = 0f;
        for (int i = 0; i < Columns.Length; i++)
            if (Columns[i].Width > 0) fixedSum += Columns[i].Width;
        float lastCol = MathF.Max(80f, tableWidth - fixedSum);

        float x = tableLeft;
        for (int i = 0; i < Columns.Length; i++)
        {
            xs[i] = x;
            x += Columns[i].Width > 0 ? Columns[i].Width : lastCol;
        }
        return xs;
    }

    private void RefreshDynamic()
    {
        if (_graphics is null) return;

        float w = MathF.Round(_lastSize.X);
        float h = MathF.Round(_lastSize.Y);
        float frameLeft = Padding * 0.5f;
        float frameTop = TitleBorderTop + TitleBorderHeight * 0.5f;
        float frameBottom = h - Padding * 0.5f;
        float tableLeft = frameLeft + 6f;
        float tableTop = frameTop + 12f;
        float tableRight = w - Padding * 0.5f - 6f;
        float tableWidth = tableRight - tableLeft;
        float dataTop = tableTop + HeaderRowHeight;
        float dataBottom = frameBottom - 6f;
        if (dataBottom - dataTop < DataRowHeight) return;
        int maxRows = (int)((dataBottom - dataTop) / DataRowHeight);

        float[] colXs = ComputeColumnXs(tableLeft, tableWidth);
        Ac[] rows = PickNonSequenced(maxRows);

        foreach (RowRefs r in _rowPool.Values) r.Touched = false;
        if (rows.Length > 0 && _emptyTextRef.IsValid)
            _graphics.SetVisible(_emptyTextRef, false);

        for (int i = 0; i < rows.Length && i < maxRows; i++)
        {
            Ac ac = rows[i];
            string callsign = ac.Callsign ?? "";
            string type = DemoFakery.GuessType(callsign);
            char wvc = DemoFakery.GuessWake(callsign);
            string reason = Reasons[Math.Abs(callsign.GetHashCode()) % Reasons.Length];
            RgbaColor wvcColor = P.WakeColor(wvc);
            float rowTop = dataTop + i * DataRowHeight;
            float baseline = MathF.Round(rowTop + DataRowHeight * 0.5f + CellFontSize * 0.35f);

            RowRefs row = GetOrCreateRow(callsign, tableLeft, tableWidth, rowTop, colXs, baseline);
            row.Touched = true;

            bool tintShown = (i % 2 == 1);
            if (tintShown != row.LastTintVisible)
            {
                _graphics.SetVisible(row.Tint, tintShown);
                row.LastTintVisible = tintShown;
            }
            _graphics.SetScreenAnchor(row.Tint, new Vector2(tableLeft, rowTop));
            _graphics.SetScreenSize(row.Tint, tableWidth, DataRowHeight);

            if (row.LastCs != callsign) { _graphics.SetText(row.Cell0, callsign); row.LastCs = callsign; }
            if (row.LastType != type) { _graphics.SetText(row.Cell1, type); row.LastType = type; }
            string wvcStr = wvc.ToString();
            if (row.LastWvc != wvcStr) { _graphics.SetText(row.Cell2, wvcStr); row.LastWvc = wvcStr; }
            if (row.LastReason != reason) { _graphics.SetText(row.Cell3, reason); row.LastReason = reason; }
            if (!ColorEqual(row.LastWvcColor, wvcColor))
            {
                _graphics.SetColor(row.Cell2, wvcColor);
                row.LastWvcColor = wvcColor;
            }

            _graphics.SetScreenAnchor(row.Cell0, new Vector2(MathF.Round(colXs[0] + 6f), baseline));
            _graphics.SetScreenAnchor(row.Cell1, new Vector2(MathF.Round(colXs[1] + 6f), baseline));
            _graphics.SetScreenAnchor(row.Cell2, new Vector2(MathF.Round(colXs[2] + 6f), baseline));
            _graphics.SetScreenAnchor(row.Cell3, new Vector2(MathF.Round(colXs[3] + 6f), baseline));

            _graphics.SetScreenPoints(row.Divider, new[]
            {
                new Vector2(tableLeft, rowTop + DataRowHeight),
                new Vector2(tableLeft + tableWidth, rowTop + DataRowHeight),
            });
        }

        RemoveStaleRows();

        if (rows.Length == 0)
        {
            if (!_emptyTextRef.IsValid)
            {
                _emptyTextRef = _graphics.AddScreenText(
                    new Vector2(MathF.Round(tableLeft + 6f),
                                MathF.Round(dataTop + DataRowHeight * 0.5f + CellFontSize * 0.35f)),
                    "(none)", P.TextDim, fontSize: CellFontSize);
                _graphics.SetHitTestVisible(_emptyTextRef, false);
            }
            else
            {
                _graphics.SetVisible(_emptyTextRef, true);
                _graphics.SetScreenAnchor(_emptyTextRef,
                    new Vector2(MathF.Round(tableLeft + 6f),
                                MathF.Round(dataTop + DataRowHeight * 0.5f + CellFontSize * 0.35f)));
            }
        }

        _lastSig = $"{rows.Length}|{w}|{h}";
    }

    private RowRefs GetOrCreateRow(string callsign, float tableLeft, float tableWidth, float rowTop, float[] colXs, float baseline)
    {
        if (_rowPool.TryGetValue(callsign, out RowRefs? existing)) return existing;
        RowRefs row = new();
        row.Tint = _graphics!.AddScreenRect(
            new Rect(tableLeft, rowTop, tableWidth, DataRowHeight), new RgbaColor(1f, 1f, 1f, 0.03f));
        _graphics.SetHitTestVisible(row.Tint, false);
        _graphics.SetVisible(row.Tint, false);
        row.Cell0 = NewCell(" ", colXs[0] + 6f, baseline, P.TextWhite);
        row.Cell1 = NewCell(" ", colXs[1] + 6f, baseline, P.TextWhite);
        row.Cell2 = NewCell(" ", colXs[2] + 6f, baseline, P.TextWhite);
        row.Cell3 = NewCell(" ", colXs[3] + 6f, baseline, P.TextDim);
        row.LastWvcColor = P.TextWhite;
        row.Divider = _graphics.AddScreenLine(
            new[] { new Vector2(tableLeft, rowTop + DataRowHeight),
                    new Vector2(tableLeft + tableWidth, rowTop + DataRowHeight) },
            P.SeparatorLine, width: 1f);
        _graphics.SetHitTestVisible(row.Divider, false);
        _rowPool[callsign] = row;
        return row;
    }

    private GraphicsObjectRef NewCell(string text, float x, float baseline, RgbaColor color)
    {
        GraphicsObjectRef r = _graphics!.AddScreenText(
            new Vector2(MathF.Round(x), baseline), text, color, fontSize: CellFontSize);
        _graphics.SetHitTestVisible(r, false);
        return r;
    }

    private void RemoveStaleRows()
    {
        if (_graphics is null) return;
        List<string>? gone = null;
        foreach ((string cs, RowRefs row) in _rowPool)
        {
            if (row.Touched) continue;
            (gone ??= new List<string>()).Add(cs);
        }
        if (gone is null) return;
        for (int i = 0; i < gone.Count; i++)
        {
            if (!_rowPool.Remove(gone[i], out RowRefs? r)) continue;
            _graphics.Remove(r.Tint);
            _graphics.Remove(r.Cell0);
            _graphics.Remove(r.Cell1);
            _graphics.Remove(r.Cell2);
            _graphics.Remove(r.Cell3);
            _graphics.Remove(r.Divider);
        }
    }

    private static bool ColorEqual(RgbaColor a, RgbaColor b)
        => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;

    private Ac[] PickNonSequenced(int max)
    {
        IReadOnlyList<Ac> all = _core.Aircraft.GetAll();
        List<Ac> picked = new();
        for (int i = 0; i < all.Count && picked.Count < max; i++)
        {
            int eta = DemoFakery.EtaMinutesTo(all[i]);
            if (eta == 0 || eta > 60) picked.Add(all[i]);
        }
        return picked.ToArray();
    }
}
