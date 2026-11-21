using System.Text.Json.Serialization;

namespace EmbeddingMap.Core;

/// <summary>A text, the group it is colored by and its embedding.</summary>
public sealed record EmbeddedText(string Text, string Group, float[] Vector);

/// <summary>One sample set as stored in its data file.</summary>
public sealed record EmbeddingSet(string Id, string Name, string Description, IReadOnlyList<EmbeddedText> Items);

/// <summary>A sample set in the index, without its vectors.</summary>
public sealed record SetSummary(string Id, string Name, string Description, int Count);

/// <summary>What the index file holds: the model that computed the embeddings and the sets.</summary>
public sealed record SetIndex(string Model, string ModelUrl, int Dimensions, IReadOnlyList<SetSummary> Sets);

/// <summary>
/// JSON serialization generated at compile time, so it keeps working when the web build trims unused code.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(EmbeddingSet))]
[JsonSerializable(typeof(SetIndex))]
public sealed partial class EmbeddingJsonContext : JsonSerializerContext;
