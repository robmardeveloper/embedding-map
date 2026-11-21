using EmbeddingMap.Core;
using Xunit;

namespace EmbeddingMap.Core.Tests;

public class NeighborsTests
{
    private static readonly float[][] Vectors =
    [
        [1, 0],     // 0
        [0.9f, 0.1f], // 1: close to 0
        [0, 1],     // 2: far from 0
        [-1, 0],    // 3: opposite to 0
        [0.7f, 0.7f], // 4: in between
    ];

    [Fact]
    public void Rank_orders_by_similarity_and_leaves_out_the_query()
    {
        var ranked = Neighbors.Rank(Vectors, 0);

        Assert.Equal([1, 4, 2, 3], ranked.Select(match => match.Index));
        Assert.DoesNotContain(ranked, match => match.Index == 0);
        Assert.True(ranked.Zip(ranked.Skip(1)).All(pair => pair.First.Similarity >= pair.Second.Similarity));
    }

    [Fact]
    public void Rank_reports_the_cosine_similarity()
    {
        var ranked = Neighbors.Rank(Vectors, 0);

        Assert.Equal(-1, ranked.Single(match => match.Index == 3).Similarity, 1e-9);
        Assert.Equal(0, ranked.Single(match => match.Index == 2).Similarity, 1e-9);
    }

    [Fact]
    public void Nearest_keeps_only_the_closest()
    {
        Assert.Equal([1, 4], Neighbors.Nearest(Vectors, 0, 2).Select(match => match.Index));
    }

    [Fact]
    public void Nearest_with_more_than_available_returns_all_others()
    {
        Assert.Equal(4, Neighbors.Nearest(Vectors, 2, 10).Count);
    }

    [Fact]
    public void Ties_keep_the_original_order()
    {
        float[][] vectors = [[1, 0], [0, 1], [0, -1]];

        Assert.Equal([1, 2], Neighbors.Rank(vectors, 0).Select(match => match.Index));
    }

    [Fact]
    public void Index_out_of_range_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Neighbors.Rank(Vectors, 5));
    }
}
