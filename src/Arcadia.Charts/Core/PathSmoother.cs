namespace Arcadia.Charts.Core;

/// <summary>
/// Generates smooth SVG paths using Catmull-Rom to cubic Bezier conversion.
/// </summary>
public static class PathSmoother
{
    /// <summary>
    /// Converts a series of points into a smooth SVG path using cubic Bezier curves.
    /// </summary>
    public static string SmoothPath(List<(double X, double Y)> points, double tension = 0.3)
    {
        if (points.Count < 2) return "";
        if (points.Count == 2)
            return $"M{F(points[0].X)},{F(points[0].Y)} L{F(points[1].X)},{F(points[1].Y)}";

        var path = $"M{F(points[0].X)},{F(points[0].Y)}";

        for (var i = 0; i < points.Count - 1; i++)
        {
            var p0 = i > 0 ? points[i - 1] : points[i];
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = i + 2 < points.Count ? points[i + 2] : points[i + 1];

            var cp1x = p1.X + (p2.X - p0.X) * tension;
            var cp1y = p1.Y + (p2.Y - p0.Y) * tension;
            var cp2x = p2.X - (p3.X - p1.X) * tension;
            var cp2y = p2.Y - (p3.Y - p1.Y) * tension;

            path += $" C{F(cp1x)},{F(cp1y)} {F(cp2x)},{F(cp2y)} {F(p2.X)},{F(p2.Y)}";
        }

        return path;
    }

    private static string F(double v) => v.ToString("F1");
}
