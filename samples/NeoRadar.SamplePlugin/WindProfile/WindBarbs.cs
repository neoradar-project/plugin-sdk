using System;
using System.Collections.Generic;
using System.Numerics;
using NeoRadar.PluginSdk.Rendering;

namespace NeoRadar.SamplePlugin.WindProfile;

internal static class WindBarbs
{
    public static void Draw(
        IPluginGraphics g,
        List<GraphicsObjectRef> sink,
        float xPos, float yPos,
        int windDirDeg,
        int windSpeedKt,
        float length,
        float barbMaxLength,
        RgbaColor color)
    {
        double shaftRad = (windDirDeg + 180 + 90) * Math.PI / 180.0;
        float sx = xPos + (float)(length * Math.Cos(shaftRad));
        float sy = yPos + (float)(length * Math.Sin(shaftRad));

        sink.Add(g.AddScreenLine([new Vector2(xPos, yPos), new Vector2(sx, sy)], color, width: 1f));
        sink.Add(g.AddScreenStrokeRect(new Rect(xPos - 2f, yPos - 2f, 4f, 4f), color, width: 1f));

        int numFifties = windSpeedKt / 50;
        int numTens = (windSpeedKt % 50) / 10;
        int numFives = (windSpeedKt % 10) / 5;
        float spacing = barbMaxLength / 2f;
        float pos = 0f;

        for (int i = 0; i < numFifties; i++)
        {
            DrawTriangle(g, sink, sx, sy, shaftRad, pos, barbMaxLength, color);
            pos += spacing;
        }
        for (int i = 0; i < numTens; i++)
        {
            DrawBarb(g, sink, sx, sy, shaftRad, pos, barbMaxLength, color);
            pos += spacing;
        }
        for (int i = 0; i < numFives; i++)
        {
            DrawBarb(g, sink, sx, sy, shaftRad, pos, barbMaxLength / 2f, color);
            pos += spacing;
        }
    }

    private static void DrawBarb(IPluginGraphics g, List<GraphicsObjectRef> sink,
        float baseX, float baseY, double angle, float offset, float barbLength, RgbaColor color)
    {
        float px = baseX - (float)(offset * Math.Cos(angle));
        float py = baseY - (float)(offset * Math.Sin(angle));
        double barbAngle = angle - Math.PI / 3;
        float bx = px + (float)(barbLength * Math.Cos(barbAngle));
        float by = py + (float)(barbLength * Math.Sin(barbAngle));
        sink.Add(g.AddScreenLine([new Vector2(px, py), new Vector2(bx, by)], color, width: 1f));
    }

    private static void DrawTriangle(IPluginGraphics g, List<GraphicsObjectRef> sink,
        float baseX, float baseY, double angle, float offset, float length, RgbaColor color)
    {
        float p1x = baseX - (float)(offset * Math.Cos(angle));
        float p1y = baseY - (float)(offset * Math.Sin(angle));
        double sideAngle = angle - Math.PI / 3;
        float p2x = p1x + (float)(length * Math.Cos(sideAngle));
        float p2y = p1y + (float)(length * Math.Sin(sideAngle));
        float p3x = p1x + (float)(length * Math.Cos(angle));
        float p3y = p1y + (float)(length * Math.Sin(angle));
        sink.Add(g.AddScreenLine(
            [new Vector2(p1x, p1y), new Vector2(p2x, p2y), new Vector2(p3x, p3y), new Vector2(p1x, p1y)],
            color, width: 1f));
    }
}
