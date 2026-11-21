// Computes the embeddings of every sample text and writes the data files the page loads.
//
// Usage: dotnet run --project tools/EmbeddingMap.Generator [model folder] [output folder]
// The model folder must hold model.onnx and sentencepiece.bpe.model from intfloat/multilingual-e5-small.

using System.Text.Json;
using EmbeddingMap.Core;
using EmbeddingMap.Generator;

const string ModelName = "multilingual-e5-small";
const string ModelUrl = "https://huggingface.co/intfloat/multilingual-e5-small";

var modelFolder = args.Length > 0
    ? args[0]
    : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "embedding-models", ModelName);
var outputFolder = args.Length > 1 ? args[1] : Path.Combine("src", "EmbeddingMap.Web", "wwwroot", "data");
Directory.CreateDirectory(outputFolder);

using var embedder = new Embedder(modelFolder);
var summaries = new List<SetSummary>();

foreach (var sample in Samples.All)
{
    var items = sample.Groups
        .SelectMany(group => group.Texts.Select(text => new EmbeddedText(text, group.Group, Round(embedder.Embed(text)))))
        .ToList();

    var set = new EmbeddingSet(sample.Id, sample.Name, sample.Description, items);
    var path = Path.Combine(outputFolder, sample.Id + ".json");
    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(set, EmbeddingJsonContext.Default.EmbeddingSet));

    summaries.Add(new SetSummary(sample.Id, sample.Name, sample.Description, items.Count));
    Console.WriteLine($"{sample.Id}: {items.Count} texts, {new FileInfo(path).Length / 1024} KB");
}

var index = new SetIndex(ModelName, ModelUrl, embedder.Dimensions, summaries);
await File.WriteAllTextAsync(Path.Combine(outputFolder, "index.json"), JsonSerializer.Serialize(index, EmbeddingJsonContext.Default.SetIndex));

// Four decimals are plenty to compare texts and keep the files small.
static float[] Round(float[] vector) => [.. vector.Select(x => MathF.Round(x, 4))];
