namespace EmbeddingMap.Core;

/// <summary>One text and how similar it is to the one being compared.</summary>
public readonly record struct Match(int Index, double Similarity);

/// <summary>Finds the texts whose embeddings are closest to a given one.</summary>
public static class Neighbors
{
    /// <summary>
    /// Every other vector, most similar first. This is search by meaning: it compares the full vectors, not the
    /// points on the 2D map.
    /// </summary>
    public static IReadOnlyList<Match> Rank(IReadOnlyList<float[]> vectors, int queryIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(queryIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(queryIndex, vectors.Count);

        var query = vectors[queryIndex];
        return [.. Enumerable.Range(0, vectors.Count)
            .Where(i => i != queryIndex)
            .Select(i => new Match(i, Similarity.Cosine(query, vectors[i])))
            .OrderByDescending(match => match.Similarity)
            .ThenBy(match => match.Index)];
    }

    /// <summary>The <paramref name="count"/> vectors most similar to the one at <paramref name="queryIndex"/>.</summary>
    public static IReadOnlyList<Match> Nearest(IReadOnlyList<float[]> vectors, int queryIndex, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        return [.. Rank(vectors, queryIndex).Take(count)];
    }
}
