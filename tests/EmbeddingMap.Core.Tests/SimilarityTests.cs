using EmbeddingMap.Core;
using Xunit;

namespace EmbeddingMap.Core.Tests;

public class SimilarityTests
{
    [Fact]
    public void Same_direction_is_one_whatever_the_length()
    {
        Assert.Equal(1, Similarity.Cosine([1, 2, 3], [2, 4, 6]), 1e-9);
    }

    [Fact]
    public void Right_angle_is_zero()
    {
        Assert.Equal(0, Similarity.Cosine([1, 0], [0, 5]), 1e-9);
    }

    [Fact]
    public void Opposite_direction_is_minus_one()
    {
        Assert.Equal(-1, Similarity.Cosine([1, -2], [-1, 2]), 1e-9);
    }

    [Fact]
    public void Known_angle_gives_its_cosine()
    {
        // 45 degrees apart.
        Assert.Equal(Math.Sqrt(0.5), Similarity.Cosine([1, 0], [1, 1]), 1e-6);
    }

    [Fact]
    public void Zero_vector_is_not_similar_to_anything()
    {
        Assert.Equal(0, Similarity.Cosine([0, 0], [1, 1]));
    }

    [Fact]
    public void Different_lengths_are_rejected()
    {
        Assert.Throws<ArgumentException>(() => Similarity.Cosine([1, 2], [1, 2, 3]));
    }
}
