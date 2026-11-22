namespace NeoRadar.SamplePlugin.RunwayBoard;

/// <summary>The runway board's demos; each acts on the first active airport, and its runway writes need Demo writes.</summary>
internal interface IRunwayBoard
{
    void Refresh();

    void ToggleCentrelines();

    void ActivateDepartureRunway();

    void DeactivateDepartureRunway();

    void SwapRunwaysInOneBatch();

    void ToggleObserved();

    void DeleteAirport();

    void LookUpNavaid(string identifier);
}
