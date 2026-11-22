namespace EmbeddingMap.Core;

/// <summary>
/// How long to draw each similarity bar. With this model almost every pair of texts scores between 0.7 and 0.95, so
/// bars from 0 to 1 would all look the same. Instead the bars stretch between the weakest and the strongest match in
/// the ranking, which shows how far ahead the best matches are. The number next to each bar stays the real similarity.
/// </summary>
public static class BarScale
{
    /// <summary>The shortest bar, so that the weakest match still shows a sliver.</summary>
    public const double Minimum = 0.03;

    /// <summary>A length from <see cref="Minimum"/> to 1 for <paramref name="similarity"/>.</summary>
    public static double Length(double similarity, double weakest, double strongest)
    {
        if (strongest <= weakest)
        {
            return 1;
        }

        var position = Math.Clamp((similarity - weakest) / (strongest - weakest), 0, 1);
        return Minimum + ((1 - Minimum) * position);
    }

    /// <summary>The weakest and strongest similarity in a ranking, or (0, 1) if it is empty.</summary>
    public static (double Weakest, double Strongest) RangeOf(IReadOnlyList<Match> ranked) =>
        ranked.Count == 0
            ? (0, 1)
            : (ranked.Min(match => match.Similarity), ranked.Max(match => match.Similarity));
}
