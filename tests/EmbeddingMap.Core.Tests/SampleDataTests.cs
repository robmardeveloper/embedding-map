using System.Text.Json;
using EmbeddingMap.Core;
using Xunit;

namespace EmbeddingMap.Core.Tests;

/// <summary>
/// Checks the data files the page loads, so what the page and the README say about them stays true.
/// </summary>
public class SampleDataTests
{
    private static readonly string DataFolder = Path.Combine(AppContext.BaseDirectory, "data");

    private static SetIndex Index() =>
        JsonSerializer.Deserialize(File.ReadAllText(Path.Combine(DataFolder, "index.json")), EmbeddingJsonContext.Default.SetIndex)!;

    private static EmbeddingSet Load(string id) =>
        JsonSerializer.Deserialize(File.ReadAllText(Path.Combine(DataFolder, id + ".json")), EmbeddingJsonContext.Default.EmbeddingSet)!;

    [Fact]
    public void Every_set_in_the_index_has_its_file_with_the_right_size()
    {
        var index = Index();

        Assert.Equal(384, index.Dimensions);
        Assert.All(index.Sets, summary =>
        {
            var set = Load(summary.Id);
            Assert.Equal(summary.Count, set.Items.Count);
            Assert.InRange(set.Items.Count, 50, 150);
            Assert.All(set.Items, item => Assert.Equal(index.Dimensions, item.Vector.Length));
            Assert.Equal(set.Items.Count, set.Items.Select(item => item.Text).Distinct().Count());
        });
    }

    [Fact]
    public void Vectors_have_length_one()
    {
        Assert.All(Index().Sets.SelectMany(summary => Load(summary.Id).Items), item =>
            Assert.Equal(1, Math.Sqrt(item.Vector.Sum(x => x * (double)x)), 1e-3));
    }

    [Fact]
    public void Most_words_have_their_translation_as_nearest_neighbor()
    {
        // The set lists the English words first and then the same words in Spanish, in the same order.
        var vectors = Load("languages").Items.Select(item => item.Vector).ToArray();
        var half = vectors.Length / 2;

        var hits = Enumerable.Range(0, vectors.Length)
            .Count(i => Neighbors.Nearest(vectors, i, 1)[0].Index == (i < half ? i + half : i - half));

        Assert.True(hits >= vectors.Length * 0.8, $"Only {hits} of {vectors.Length} words are next to their translation.");
    }

    [Fact]
    public void Search_by_meaning_finds_a_match_with_no_words_in_common()
    {
        var items = Load("bookings").Items;
        var vectors = items.Select(item => item.Vector).ToArray();
        var query = items.ToList().FindIndex(item => item.Text == "No me ha llegado el email de la reserva");

        var best = items[Neighbors.Nearest(vectors, query, 1)[0].Index];

        Assert.Equal("Falta la confirmación de mi reserva", best.Text);
    }

    [Fact]
    public void Dog_and_perro_are_very_similar()
    {
        var items = Load("languages").Items;

        Assert.True(Similarity.Cosine(
            items.Single(item => item.Text == "dog").Vector,
            items.Single(item => item.Text == "perro").Vector) > 0.9);
    }
}
