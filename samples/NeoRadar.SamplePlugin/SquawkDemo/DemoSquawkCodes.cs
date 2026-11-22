namespace NeoRadar.SamplePlugin.SquawkDemo;

/// <summary>Hands out 4601 to 4677 in turn: four octal digits, as a provider must.</summary>
internal static class DemoSquawkCodes
{
    public const int Count = 63;

    /// <param name="index">Any counter value; it wraps around the 63 codes.</param>
    public static string At(long index)
    {
        int n = (int)(((index % Count) + Count) % Count) + 1;
        return $"46{n / 8}{n % 8}";
    }
}
