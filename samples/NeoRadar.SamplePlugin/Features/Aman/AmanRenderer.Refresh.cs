using System;
using System.Collections.Generic;
using NeoRadar.PluginSdk.Rendering;
using NeoRadar.SamplePlugin.Common;
using Ac = NeoRadar.PluginSdk.Aircraft.Aircraft;
using P = NeoRadar.SamplePlugin.Common.AmanPalette;
using L = NeoRadar.SamplePlugin.Features.Aman.AmanLayout;

namespace NeoRadar.SamplePlugin.Features.Aman;

internal sealed partial class AmanRenderer
{
    private void RefreshDynamic()
    {
        if (_graphics is null) return;
        Ac[] aircraft;
        try { aircraft = GetSortedByEta(); }
        catch { return; }

        _nonSeqCount = ComputeFakeNonSeqCount(aircraft);
        UpdateNonSeqButton();

        string sig = BuildSignature(aircraft);
        if (sig == _lastSignature) return;
        _lastSignature = sig;

        foreach (GraphicsObjectRef r in _dynamicRefs) _graphics.Remove(r);
        _dynamicRefs.Clear();

        foreach (AircraftRefs refs in _aircraftPool.Values) refs.Touched = false;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        DateTimeOffset rangeStart = now.AddMinutes(-L.HistoryMinutes + _scrollOffsetMin);
        DateTimeOffset rangeEnd = now.AddMinutes(L.FutureMinutes + _scrollOffsetMin);
        float top = TimelineTop;
        float height = TimelineHeight;
        float nowY = YForInstant(now, rangeStart, rangeEnd, top, height);

        for (int i = 0; i < _columns.Length; i++)
        {
            L.TimelineColumn col = _columns[i];

            if (nowY < TimelineBottom)
            {
                _dynamicRefs.Add(_graphics.AddScreenRect(
                    new Rect(col.ScaleX, nowY, L.ScaleWidth, TimelineBottom - nowY), P.ScalePastBg));
            }

            DrawScaleTicks(col, rangeStart, rangeEnd, top, height);
            DrawHourglass(col.ScaleRight, nowY);

            Ac[] forColumn = FilterForColumn(aircraft, i);
            DrawArrivalChain(forColumn, col, rangeStart, rangeEnd, top, height, now);
        }

        UpdateClock(now);
        RemoveStaleAircraft();
    }

    private Ac[] FilterForColumn(Ac[] all, int columnIndex)
    {
        if (_columns.Length == 1) return all;
        List<Ac> picked = new(all.Length / _columns.Length + 1);
        for (int i = 0; i < all.Length; i++)
        {
            int hash = (int)((uint)(all[i].Callsign?.GetHashCode() ?? 0) % (uint)_columns.Length);
            if (hash == columnIndex) picked.Add(all[i]);
        }
        return picked.ToArray();
    }

    private Ac[] GetSortedByEta()
    {
        IReadOnlyList<Ac> all = _core.Aircraft.GetAll();
        List<Ac> filtered = new(all.Count);
        for (int i = 0; i < all.Count; i++)
        {
            int eta = DemoFakery.EtaMinutesTo(all[i]);
            double dist = DemoFakery.DistanceNmTo(all[i]);
            if (eta < 1) continue;
            if (dist < 2.0) continue;
            if (eta > L.FutureMinutes + 5) continue;
            filtered.Add(all[i]);
        }
        Ac[] arr = filtered.ToArray();
        Array.Sort(arr, static (a, b) => DemoFakery.EtaMinutesTo(a).CompareTo(DemoFakery.EtaMinutesTo(b)));
        return arr;
    }

    private string BuildSignature(Ac[] aircraft)
    {
        System.Text.StringBuilder sb = new();
        sb.Append(aircraft.Length);
        sb.Append('|');
        sb.Append(_selectedCallsign);
        sb.Append('|');
        sb.Append(DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60);
        sb.Append('|');
        sb.Append(MathF.Round(_scrollOffsetMin));
        for (int i = 0; i < aircraft.Length; i++)
        {
            sb.Append('|');
            sb.Append(aircraft[i].Callsign);
            sb.Append(':');
            sb.Append(DemoFakery.EtaMinutesTo(aircraft[i]));
        }
        return sb.ToString();
    }

    private static int ComputeFakeNonSeqCount(Ac[] aircraft)
    {
        int count = 0;
        for (int i = 1; i < aircraft.Length; i++)
        {
            int a = DemoFakery.EtaMinutesTo(aircraft[i - 1]);
            int b = DemoFakery.EtaMinutesTo(aircraft[i]);
            if (b - a < 1) count++;
        }
        return count;
    }
}
