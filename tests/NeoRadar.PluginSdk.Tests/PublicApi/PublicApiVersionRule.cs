using System;

namespace NeoRadar.PluginSdk.Tests.PublicApi;

// Checks the version step fits the surface change: a break needs a minor, an addition a patch.
internal static class PublicApiVersionRule
{
    // Returns null when the version step fits the change, otherwise the reason it does not.
    internal static string? Refusal(Version frozen, Version current, int added, int removed)
    {
        Version nextPatch = new(frozen.Major, frozen.Minor, frozen.Build + 1);
        Version nextMinor = new(frozen.Major, frozen.Minor + 1, 0);
        Version nextMajor = new(frozen.Major + 1, 0, 0);
        bool isNextMinorOrMajor = current == nextMinor || current == nextMajor;

        if (removed > 0)
        {
            return isNextMinorOrMajor
                ? null
                : $"{removed} public member(s) were removed or changed, which breaks a plugin: the version must move " +
                  $"from {frozen} to {nextMinor}, not {current}.";
        }

        if (added > 0)
        {
            return isNextMinorOrMajor || current == nextPatch
                ? null
                : $"{added} public member(s) were added: the version must move from {frozen} to {nextPatch}, not {current}.";
        }

        return current == frozen
            ? null
            : $"No public member changed, so the version must stay {frozen}, not {current}.";
    }
}
