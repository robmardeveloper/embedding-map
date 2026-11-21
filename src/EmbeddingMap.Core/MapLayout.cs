namespace EmbeddingMap.Core;

/// <summary>Places the PCA points inside the drawing area of the map.</summary>
public static class MapLayout
{
    /// <summary>
    /// Scales and moves the points so they fill a <paramref name="width"/> by <paramref name="height"/> area, leaving
    /// <paramref name="margin"/> on every side. Both axes get the same scale, so distances on the map are not
    /// stretched in one direction.
    /// </summary>
    public static IReadOnlyList<Point2D> Fit(IReadOnlyList<Point2D> points, double width, double height, double margin)
    {
        if (points.Count == 0)
        {
            return [];
        }

        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxY = points.Max(p => p.Y);

        var spanX = maxX - minX;
        var spanY = maxY - minY;
        var usableWidth = width - (2 * margin);
        var usableHeight = height - (2 * margin);

        var scale = Math.Min(
            spanX > 0 ? usableWidth / spanX : double.PositiveInfinity,
            spanY > 0 ? usableHeight / spanY : double.PositiveInfinity);
        if (double.IsInfinity(scale))
        {
            scale = 0; // Every point is in the same place: put them all in the middle.
        }

        // Center the cloud of points in the area.
        var centerX = (minX + maxX) / 2;
        var centerY = (minY + maxY) / 2;
        return [.. points.Select(p => new Point2D(
            (width / 2) + ((p.X - centerX) * scale),
            (height / 2) + ((p.Y - centerY) * scale)))];
    }
}
