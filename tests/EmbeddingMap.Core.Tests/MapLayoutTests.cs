using EmbeddingMap.Core;
using Xunit;

namespace EmbeddingMap.Core.Tests;

public class MapLayoutTests
{
    [Fact]
    public void Points_fill_the_area_inside_the_margin()
    {
        Point2D[] points = [new(-1, -1), new(1, 1), new(0, 0.5)];

        var fitted = MapLayout.Fit(points, 100, 100, 10);

        Assert.Equal(new Point2D(10, 10), fitted[0]);
        Assert.Equal(new Point2D(90, 90), fitted[1]);
        Assert.Equal(new Point2D(50, 70), fitted[2]);
    }

    [Fact]
    public void Both_axes_get_the_same_scale()
    {
        // Wide cloud in a square area: it fills the width and is centered vertically.
        Point2D[] points = [new(0, 0), new(4, 1)];

        var fitted = MapLayout.Fit(points, 100, 100, 0);

        Assert.Equal(new Point2D(0, 37.5), fitted[0]);
        Assert.Equal(new Point2D(100, 62.5), fitted[1]);
    }

    [Fact]
    public void Points_in_one_place_go_to_the_middle()
    {
        var fitted = MapLayout.Fit([new(3, 3), new(3, 3)], 200, 100, 10);

        Assert.All(fitted, point => Assert.Equal(new Point2D(100, 50), point));
    }

    [Fact]
    public void No_points_give_no_points()
    {
        Assert.Empty(MapLayout.Fit([], 100, 100, 10));
    }
}
