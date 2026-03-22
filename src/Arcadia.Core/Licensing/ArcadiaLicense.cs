using System;
using System.Text.RegularExpressions;

namespace Arcadia.Core.Licensing;

/// <summary>
/// Represents the available license tiers for Arcadia Controls.
/// </summary>
public enum LicenseTier
{
    /// <summary>
    /// Free community tier with limited features.
    /// </summary>
    Community,

    /// <summary>
    /// Professional tier with full component access.
    /// </summary>
    Pro,

    /// <summary>
    /// Enterprise tier with premium support and extended features.
    /// </summary>
    Enterprise
}

/// <summary>
/// Provides license key validation and tier detection for Arcadia Controls.
/// Developers call <see cref="SetKey"/> in their Program.cs to activate a license.
/// </summary>
/// <remarks>
/// <para>License key format: <c>ARC-XXXX-XXXX-XXXX</c> where each group is 4 alphanumeric characters.</para>
/// <para>Keys starting with <c>ARC-P</c> activate the <see cref="LicenseTier.Pro"/> tier.</para>
/// <para>Keys starting with <c>ARC-E</c> activate the <see cref="LicenseTier.Enterprise"/> tier.</para>
/// <para>No key or an invalid key defaults to <see cref="LicenseTier.Community"/>.</para>
/// </remarks>
public static class ArcadiaLicense
{
    private static readonly object SyncLock = new();
    private static string? _currentKey;
    private static LicenseTier _currentTier = LicenseTier.Community;
    private static bool _isValid;

    // ARC-XXXX-XXXX-XXXX where X is alphanumeric
    private static readonly Regex KeyFormatRegex = new(
        @"^ARC-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Gets a value indicating whether a valid Pro or Enterprise license key has been set.
    /// </summary>
    public static bool IsProLicensed
    {
        get
        {
            lock (SyncLock)
            {
                return _isValid && _currentTier is LicenseTier.Pro or LicenseTier.Enterprise;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the library is running in Community mode.
    /// Returns <c>true</c> when no key has been set or the key is invalid.
    /// </summary>
    public static bool IsCommunity
    {
        get
        {
            lock (SyncLock)
            {
                return !_isValid || _currentTier == LicenseTier.Community;
            }
        }
    }

    /// <summary>
    /// Sets and validates a license key for the Arcadia Controls library.
    /// Call this once during application startup (e.g., in Program.cs).
    /// </summary>
    /// <param name="licenseKey">
    /// The license key in the format <c>ARC-XXXX-XXXX-XXXX</c>.
    /// Pass <c>null</c> or empty to revert to Community mode.
    /// </param>
    /// <example>
    /// <code>
    /// ArcadiaLicense.SetKey("ARC-P1A2-B3C4-K7M2");
    /// </code>
    /// </example>
    public static void SetKey(string? licenseKey)
    {
        lock (SyncLock)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                _currentKey = null;
                _isValid = false;
                _currentTier = LicenseTier.Community;
                return;
            }

            _currentKey = licenseKey.Trim();
            _isValid = ValidateKey(_currentKey);
            _currentTier = _isValid ? DetermineTier(_currentKey) : LicenseTier.Community;
        }
    }

    /// <summary>
    /// Returns the current license tier based on the configured key.
    /// </summary>
    /// <returns>
    /// The <see cref="LicenseTier"/> determined by the current license key.
    /// Returns <see cref="LicenseTier.Community"/> when no valid key is set.
    /// </returns>
    public static LicenseTier GetTier()
    {
        lock (SyncLock)
        {
            return _currentTier;
        }
    }

    /// <summary>
    /// Resets the license state to defaults. Intended for testing only.
    /// </summary>
    internal static void Reset()
    {
        lock (SyncLock)
        {
            _currentKey = null;
            _isValid = false;
            _currentTier = LicenseTier.Community;
        }
    }

    /// <summary>
    /// Validates the license key by checking its format and verifying the
    /// checksum group (4th segment) against the first three groups.
    /// </summary>
    private static bool ValidateKey(string key)
    {
        if (!KeyFormatRegex.IsMatch(key))
        {
            return false;
        }

        // Split into segments: ["ARC", group1, group2, group3]
        var segments = key.Split('-');
        var payload = segments[1] + segments[2]; // first two variable groups
        var expectedChecksum = ComputeChecksum(payload);

        return string.Equals(segments[3], expectedChecksum, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Computes a 4-character alphanumeric checksum from the payload string.
    /// Uses a simple hash to produce a deterministic verification group.
    /// </summary>
    internal static string ComputeChecksum(string payload)
    {
        // Simple hash: sum char values with position weighting
        long hash = 0;
        for (var i = 0; i < payload.Length; i++)
        {
            hash = (hash * 31) + payload[i];
        }

        // Ensure positive value
        hash = Math.Abs(hash);

        // Convert to 4-char alphanumeric
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = new char[4];
        for (var i = 0; i < 4; i++)
        {
            result[i] = chars[(int)(hash % chars.Length)];
            hash /= chars.Length;
        }

        return new string(result);
    }

    /// <summary>
    /// Determines the license tier from the key prefix.
    /// </summary>
    private static LicenseTier DetermineTier(string key)
    {
        if (key.StartsWith("ARC-E", StringComparison.OrdinalIgnoreCase))
        {
            return LicenseTier.Enterprise;
        }

        if (key.StartsWith("ARC-P", StringComparison.OrdinalIgnoreCase))
        {
            return LicenseTier.Pro;
        }

        return LicenseTier.Community;
    }
}
