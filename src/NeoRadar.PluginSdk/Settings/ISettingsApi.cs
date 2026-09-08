using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Settings;

/// <summary>
/// Key/value storage the host persists for a plugin. Values are strings only, so a plugin does its
/// own typing and its own parsing. Two scopes: plugin scope is keyed by the manifest name and is
/// the place for a setting that follows the controller everywhere; profile scope is additionally
/// keyed by the active profile and is the place for per-display state such as a panel's position,
/// the analogue of a EuroScope ASR value.
/// Keys are case-sensitive and at most 128 characters. The host may reject a value over 64 KB and
/// returns false when it does.
/// </summary>
public interface ISettingsApi
{
    /// <summary>
    /// Read a plugin-scoped value, or null when the key was never set. Plugin-scoped values are
    /// keyed by the manifest name and survive a client restart.
    /// </summary>
    string? Get(string key);

    /// <summary>
    /// Write a plugin-scoped value. Returns false when the key is empty or over 128 characters, or
    /// when the host rejected the value for being over 64 KB.
    /// </summary>
    bool Set(string key, string value);

    /// <summary>
    /// Delete a plugin-scoped value. Returns false when the key was not set.
    /// </summary>
    bool Remove(string key);

    /// <summary>
    /// Every plugin-scoped key currently set, in no defined order. Profile-scoped keys are not
    /// listed here.
    /// </summary>
    IReadOnlyList<string> Keys();

    /// <summary>
    /// Name of the profile the client currently has loaded, or null when none is loaded. A profile
    /// is the saved radar display setup, the analogue of a EuroScope ASR.
    /// </summary>
    string? ActiveProfile { get; }

    /// <summary>
    /// Read a value scoped to both the plugin and the active profile. Returns null when the key was
    /// never set for that profile, and null when no profile is active.
    /// </summary>
    string? GetForProfile(string key);

    /// <summary>
    /// Write a value scoped to both the plugin and the active profile. Returns false when no
    /// profile is active, when the key is empty or over 128 characters, or when the host rejected
    /// the value for being over 64 KB.
    /// </summary>
    bool SetForProfile(string key, string value);

    /// <summary>
    /// Delete a value scoped to both the plugin and the active profile. Returns false when no
    /// profile is active or the key was not set for that profile.
    /// </summary>
    bool RemoveForProfile(string key);
}
