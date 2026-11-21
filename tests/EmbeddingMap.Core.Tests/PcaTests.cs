using EmbeddingMap.Core;
using Xunit;

namespace EmbeddingMap.Core.Tests;

public class PcaTests
{
    [Fact]
    public void Points_on_a_line_keep_all_the_variation_on_the_first_axis()
    {
        // Five points on a line through 4D space, evenly spaced.
        float[] direction = [1, 2, -1, 0.5f];
        var vectors = Enumerable.Range(0, 5).Select(t => direction.Select(x => x * t).ToArray()).ToArray();

        var result = Pca.To2D(vectors);

        Assert.Equal(1, result.ExplainedVariance, 1e-6);
        Assert.All(result.Points, point => Assert.Equal(0, point.Y, 1e-6));

        // Consecutive points stay the same distance apart: the length of the direction vector.
        var step = Math.Sqrt(direction.Sum(x => x * (double)x));
        var xs = result.Points.Select(point => point.X).Order().ToArray();
        Assert.All(xs.Zip(xs.Skip(1)), pair => Assert.Equal(step, pair.Second - pair.First, 1e-5));
    }

    [Fact]
    public void A_flat_shape_in_more_dimensions_keeps_its_distances()
    {
        // A square lying in a plane inside 5D space. Two axes are enough to describe it exactly.
        float[] u = [1, 0, 0, 0, 0];
        float[] v = [0, 0.6f, 0.8f, 0, 0];
        (float A, float B)[] corners = [(0, 0), (3, 0), (3, 3), (0, 3)];
        var vectors = corners.Select(c => u.Zip(v, (x, y) => (c.A * x) + (c.B * y) + 7).ToArray()).ToArray();

        var result = Pca.To2D(vectors);

        Assert.Equal(1, result.ExplainedVariance, 1e-6);
        for (var i = 0; i < vectors.Length; i++)
        {
            for (var j = i + 1; j < vectors.Length; j++)
            {
                Assert.Equal(Distance(vectors[i], vectors[j]), Distance(result.Points[i], result.Points[j]), 1e-4);
            }
        }
    }

    [Fact]
    public void The_map_is_centered()
    {
        var result = Pca.To2D(Random(30, 8, seed: 1));

        Assert.Equal(0, result.Points.Average(point => point.X), 1e-6);
        Assert.Equal(0, result.Points.Average(point => point.Y), 1e-6);
    }

    [Fact]
    public void The_first_axis_spreads_more_than_the_second()
    {
        var result = Pca.To2D(Random(30, 8, seed: 2));

        Assert.True(Variance(result.Points.Select(p => p.X)) >= Variance(result.Points.Select(p => p.Y)));
        Assert.InRange(result.ExplainedVariance, 0, 1);
    }

    [Fact]
    public void Same_input_gives_the_same_map()
    {
        var vectors = Random(20, 6, seed: 3);

        Assert.Equal(Pca.To2D(vectors).Points, Pca.To2D(vectors).Points);
    }

    [Fact]
    public void Identical_vectors_all_land_on_the_origin()
    {
        float[][] vectors = [[1, 2, 3], [1, 2, 3], [1, 2, 3]];

        var result = Pca.To2D(vectors);

        Assert.All(result.Points, point => Assert.Equal(new Point2D(0, 0), point));
        Assert.Equal(0, result.ExplainedVariance);
    }

    [Fact]
    public void No_vectors_give_an_empty_map()
    {
        Assert.Empty(Pca.To2D([]).Points);
    }

    private static float[][] Random(int count, int dimensions, int seed)
    {
        var random = new Random(seed);
        return [.. Enumerable.Range(0, count)
            .Select(_ => Enumerable.Range(0, dimensions).Select(k => (float)(random.NextDouble() * (k + 1))).ToArray())];
    }

    private static double Distance(float[] a, float[] b) => Math.Sqrt(a.Zip(b, (x, y) => (x - y) * (double)(x - y)).Sum());

    private static double Distance(Point2D a, Point2D b) => Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));

    private static double Variance(IEnumerable<double> values)
    {
        var list = values.ToList();
        var mean = list.Average();
        return list.Average(x => (x - mean) * (x - mean));
    }
}
