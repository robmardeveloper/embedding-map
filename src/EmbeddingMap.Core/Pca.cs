namespace EmbeddingMap.Core;

/// <summary>A point on the 2D map.</summary>
public readonly record struct Point2D(double X, double Y);

/// <summary>
/// The 2D map and how much of the original spread it keeps. <see cref="ExplainedVariance"/> goes from 0 to 1. A value
/// of 0.3 means the two map axes keep 30% of the variation among the vectors and the rest is lost.
/// </summary>
public sealed record PcaResult(IReadOnlyList<Point2D> Points, double ExplainedVariance);

/// <summary>
/// Principal component analysis down to two dimensions. It finds the two directions in which the vectors differ
/// the most and places each vector by how far it lies along them.
/// </summary>
public static class Pca
{
    private const int MaxIterations = 2000;
    private const double Converged = 1e-10;

    public static PcaResult To2D(IReadOnlyList<float[]> vectors)
    {
        var n = vectors.Count;
        if (n == 0)
        {
            return new PcaResult([], 0);
        }

        var gram = GramMatrix(Center(vectors));

        double total = 0;
        for (var i = 0; i < n; i++)
        {
            total += gram[i, i];
        }

        var (first, firstValue) = TopEigenvector(gram);
        Deflate(gram, first, firstValue);
        var (second, secondValue) = TopEigenvector(gram);

        // The coordinate of each vector on a principal axis is sqrt(eigenvalue) times the eigenvector.
        var scaleX = Math.Sqrt(firstValue);
        var scaleY = Math.Sqrt(secondValue);
        var points = new Point2D[n];
        for (var i = 0; i < n; i++)
        {
            points[i] = new Point2D(first[i] * scaleX, second[i] * scaleY);
        }

        var explained = total <= 0 ? 0 : Math.Min((firstValue + secondValue) / total, 1);
        return new PcaResult(points, explained);
    }

    private static double[][] Center(IReadOnlyList<float[]> vectors)
    {
        var dimensions = vectors[0].Length;
        if (vectors.Any(vector => vector.Length != dimensions))
        {
            throw new ArgumentException("All vectors must have the same length.", nameof(vectors));
        }

        var mean = new double[dimensions];
        foreach (var vector in vectors)
        {
            for (var k = 0; k < dimensions; k++)
            {
                mean[k] += vector[k];
            }
        }

        for (var k = 0; k < dimensions; k++)
        {
            mean[k] /= vectors.Count;
        }

        return [.. vectors.Select(vector => Enumerable.Range(0, dimensions).Select(k => vector[k] - mean[k]).ToArray())];
    }

    // With a few hundred vectors of 384 numbers it is cheaper to work on the n x n matrix of dot products (the Gram
    // matrix) than on the 384 x 384 covariance matrix. Both have the same nonzero eigenvalues.
    private static double[,] GramMatrix(double[][] rows)
    {
        var n = rows.Length;
        var gram = new double[n, n];
        for (var i = 0; i < n; i++)
        {
            for (var j = i; j < n; j++)
            {
                double dot = 0;
                for (var k = 0; k < rows[i].Length; k++)
                {
                    dot += rows[i][k] * rows[j][k];
                }

                gram[i, j] = dot;
                gram[j, i] = dot;
            }
        }

        return gram;
    }

    // Power iteration: multiplying a vector by the matrix again and again turns it towards the eigenvector with the
    // largest eigenvalue. The start vector is fixed so the map always comes out the same.
    private static (double[] Vector, double Value) TopEigenvector(double[,] matrix)
    {
        var n = matrix.GetLength(0);
        var vector = new double[n];
        for (var i = 0; i < n; i++)
        {
            vector[i] = 1 + Math.Sin(i + 1);
        }

        Scale(vector, 1 / Norm(vector));
        var value = 0.0;

        for (var iteration = 0; iteration < MaxIterations; iteration++)
        {
            var next = Multiply(matrix, vector);
            var norm = Norm(next);
            if (norm < 1e-12)
            {
                // Nothing left to explain, for example when every vector is the same.
                return (new double[n], 0);
            }

            Scale(next, 1 / norm);
            var change = 0.0;
            for (var i = 0; i < n; i++)
            {
                change = Math.Max(change, Math.Abs(next[i] - vector[i]));
            }

            vector = next;
            value = norm;
            if (change < Converged)
            {
                break;
            }
        }

        // An eigenvector can point either way. Fix the sign so the largest coordinate is positive.
        if (vector.MaxBy(Math.Abs) < 0)
        {
            Scale(vector, -1);
        }

        return (vector, value);
    }

    // Removes the direction already found so the next power iteration finds the second one.
    private static void Deflate(double[,] matrix, double[] vector, double value)
    {
        for (var i = 0; i < vector.Length; i++)
        {
            for (var j = 0; j < vector.Length; j++)
            {
                matrix[i, j] -= value * vector[i] * vector[j];
            }
        }
    }

    private static double[] Multiply(double[,] matrix, double[] vector)
    {
        var result = new double[vector.Length];
        for (var i = 0; i < vector.Length; i++)
        {
            double sum = 0;
            for (var j = 0; j < vector.Length; j++)
            {
                sum += matrix[i, j] * vector[j];
            }

            result[i] = sum;
        }

        return result;
    }

    private static double Norm(double[] vector) => Math.Sqrt(vector.Sum(x => x * x));

    private static void Scale(double[] vector, double factor)
    {
        for (var i = 0; i < vector.Length; i++)
        {
            vector[i] *= factor;
        }
    }
}
