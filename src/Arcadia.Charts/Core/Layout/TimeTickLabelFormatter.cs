namespace Arcadia.Charts.Core.Layout;

/// <summary>
/// Selects and applies an appropriate date/time format string based on the data range.
/// </summary>
internal static class TimeTickLabelFormatter
{
    /// <summary>Gets the best format string for the given time span.</summary>
    public static string GetFormat(TimeSpan range)
    {
        if (range.TotalHours < 2) return "HH:mm:ss";
        if (range.TotalDays < 2) return "HH:mm";
        if (range.TotalDays < 60) return "MMM d";
        if (range.TotalDays < 730) return "MMM yyyy";
        return "yyyy";
    }

    /// <summary>Formats a DateTime using the auto-detected format for the given range.</summary>
    public static string Format(DateTime value, TimeSpan range, IFormatProvider? provider = null)
    {
        var fmt = GetFormat(range);
        return value.ToString(fmt, provider);
    }
}
