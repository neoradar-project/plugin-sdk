using System.Collections.Generic;

namespace NeoRadar.PluginSdk.Settings;

/// <summary>
/// Provides string key and value storage the host persists for a plugin, scoped to the plugin or the active profile.
/// </summary>
/// <remarks>Keys are case-sensitive and at most 128 characters, and the host may reject a value over 64 KB.</remarks>
public interface ISettingsApi
{
    /// <summary>
    /// Gets a plugin-scoped value, which follows the controller across profiles and restarts.
    /// </summary>
    /// <returns>The value, or null when the key was never set.</returns>
    string? Get(string key);

    /// <summary>
    /// Sets a plugin-scoped value.
    /// </summary>
    /// <returns>False if the key is empty or over 128 characters, or the value was rejected for being over 64 KB.</returns>
    bool Set(string key, string value);

    /// <summary>
    /// Removes a plugin-scoped value.
    /// </summary>
    /// <returns>False if the key was not set.</returns>
    bool Remove(string key);

    /// <summary>
    /// Gets every plugin-scoped key currently set, in no defined order.
    /// </summary>
    IReadOnlyList<string> Keys();

    /// <summary>
    /// Gets the name of the loaded profile, the saved radar display setup, or null when none is loaded.
    /// </summary>
    string? ActiveProfile { get; }

    /// <summary>
    /// Gets a value scoped to the plugin and the active profile, such as a panel's position.
    /// </summary>
    /// <returns>The value, or null when the key was never set for that profile or no profile is active.</returns>
    string? GetForProfile(string key);

    /// <summary>
    /// Sets a value scoped to the plugin and the active profile.
    /// </summary>
    /// <returns>False if no profile is active, the key is empty or over 128 characters, or the value is over 64 KB.</returns>
    bool SetForProfile(string key, string value);

    /// <summary>
    /// Removes a value scoped to the plugin and the active profile.
    /// </summary>
    /// <returns>False if no profile is active or the key was not set for that profile.</returns>
    bool RemoveForProfile(string key);
}
