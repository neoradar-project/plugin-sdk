namespace NeoRadar.PluginSdk.Rendering;

public interface IRenderingInterface
{
    ObjectHandle DrawLine(DrawLineParams parameters);

    ObjectHandle DrawFill(DrawFillParams parameters);

    ObjectHandle DrawText(DrawTextParams parameters);

    ObjectHandle DrawPoints(DrawPointParams parameters);

    bool SetVisible(ObjectHandle handle, bool visible);

    bool SetColor(ObjectHandle handle, Color color);

    bool Remove(ObjectHandle handle);

    void RemoveAll();

    ViewportInfo GetViewport();

    Point2D WorldToScreen(double worldX, double worldY);

    Point2D ScreenToWorld(double screenX, double screenY);
}

public interface IRenderingApi
{
    IRenderingInterface? GetInterface();
}
