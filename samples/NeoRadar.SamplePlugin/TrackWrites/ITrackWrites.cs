namespace NeoRadar.SamplePlugin.TrackWrites;

/// <summary>Controller-data writes on the selected aircraft; each is refused while "Demo writes" is not ticked.</summary>
internal interface ITrackWrites
{
    void SetScratchpad();

    void SetClearedLevel();

    void BurstScratchpad();

    void Assume();

    void Release();

    void AcceptHandoffWithNoOffer();

    void RejectHandoffWithNoOffer();

    void CancelHandoff();

    void SetNextController();
}
