using EmbeddingMap.Core;
using Xunit;

namespace EmbeddingMap.Core.Tests;

public class BarScaleTests
{
    [Fact]
    public void The_strongest_match_fills_the_bar_and_the_weakest_shows_a_sliver()
    {
        Assert.Equal(1, BarScale.Length(0.94, 0.80, 0.94), 6);
        Assert.Equal(BarScale.Minimum, BarScale.Length(0.80, 0.80, 0.94), 6);
    }

    [Fact]
    public void Lengths_in_between_are_proportional()
    {
        var middle = BarScale.Length(0.87, 0.80, 0.94);

        Assert.Equal(BarScale.Minimum + ((1 - BarScale.Minimum) * 0.5), middle, 6);
    }

    [Fact]
    public void Close_scores_get_clearly_different_bars()
    {
        // A translation at 0.94 against unrelated words at 0.89, in a ranking that goes down to 0.80.
        var translation = BarScale.Length(0.94, 0.80, 0.94);
        var unrelated = BarScale.Length(0.89, 0.80, 0.94);

        Assert.True(translation - unrelated > 0.3);
    }

    [Fact]
    public void Values_outside_the_range_are_clamped()
    {
        Assert.Equal(1, BarScale.Length(0.99, 0.80, 0.94), 6);
        Assert.Equal(BarScale.Minimum, BarScale.Length(0.50, 0.80, 0.94), 6);
    }

    [Fact]
    public void A_ranking_where_every_score_is_equal_gets_full_bars()
    {
        Assert.Equal(1, BarScale.Length(0.9, 0.9, 0.9), 6);
    }

    [Fact]
    public void RangeOf_finds_the_weakest_and_strongest()
    {
        Assert.Equal((0.7, 0.95), BarScale.RangeOf([new Match(1, 0.95), new Match(2, 0.8), new Match(3, 0.7)]));
        Assert.Equal((0.0, 1.0), BarScale.RangeOf([]));
    }
}
