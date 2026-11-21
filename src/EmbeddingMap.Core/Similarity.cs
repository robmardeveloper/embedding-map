namespace EmbeddingMap.Core;

/// <summary>How alike two embeddings are.</summary>
public static class Similarity
{
    /// <summary>
    /// Cosine similarity: 1 when two vectors point the same way, 0 when they are unrelated (at right angles) and -1
    /// when they point in opposite directions. Only the direction counts, not the length.
    /// </summary>
    public static double Cosine(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
    {
        if (a.Length != b.Length)
        {
            throw new ArgumentException($"Vectors must have the same length ({a.Length} and {b.Length}).", nameof(b));
        }

        double dot = 0, normA = 0, normB = 0;
        for (var i = 0; i < a.Length; i++)
        {
            dot += a[i] * (double)b[i];
            normA += a[i] * (double)a[i];
            normB += b[i] * (double)b[i];
        }

        // A zero vector has no direction, so it is not similar to anything.
        return normA == 0 || normB == 0 ? 0 : dot / Math.Sqrt(normA * normB);
    }
}
