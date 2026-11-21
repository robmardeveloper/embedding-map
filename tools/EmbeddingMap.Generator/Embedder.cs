using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;

namespace EmbeddingMap.Generator;

/// <summary>
/// Turns a text into an embedding with multilingual-e5-small, run locally with ONNX Runtime.
/// </summary>
internal sealed class Embedder : IDisposable
{
    // The model uses the token IDs of XLM-RoBERTa. Its vocabulary is the SentencePiece one shifted by one, with four
    // special tokens at the start.
    private const long BeginOfText = 0;
    private const long EndOfText = 2;
    private const long Unknown = 3;
    private const int MaxTokens = 512;

    private readonly SentencePieceTokenizer tokenizer;
    private readonly InferenceSession session;

    public Embedder(string modelFolder)
    {
        using (var stream = File.OpenRead(Path.Combine(modelFolder, "sentencepiece.bpe.model")))
        {
            tokenizer = SentencePieceTokenizer.Create(stream, false, false);
        }

        session = new InferenceSession(Path.Combine(modelFolder, "model.onnx"));
    }

    public int Dimensions => session.OutputMetadata["last_hidden_state"].Dimensions[^1];

    public float[] Embed(string text)
    {
        // E5 models expect a prefix. "query: " is the one to use when comparing texts with each other.
        var pieces = tokenizer.EncodeToIds("query: " + text);
        var ids = new List<long>(pieces.Count + 2) { BeginOfText };
        ids.AddRange(pieces.Take(MaxTokens - 2).Select(id => id == 0 ? Unknown : id + 1L));
        ids.Add(EndOfText);

        var length = ids.Count;
        var shape = new[] { 1, length };
        var inputs = new[]
        {
            NamedOnnxValue.CreateFromTensor("input_ids", new DenseTensor<long>(ids.ToArray(), shape)),
            NamedOnnxValue.CreateFromTensor("attention_mask", new DenseTensor<long>(Enumerable.Repeat(1L, length).ToArray(), shape)),
            NamedOnnxValue.CreateFromTensor("token_type_ids", new DenseTensor<long>(new long[length], shape)),
        };

        using var results = session.Run(inputs);
        var hidden = results.First(result => result.Name == "last_hidden_state").AsTensor<float>();

        // The model gives one vector per token. The embedding of the whole text is their average, scaled to length 1.
        var dimensions = hidden.Dimensions[2];
        var embedding = new float[dimensions];
        for (var token = 0; token < length; token++)
        {
            for (var k = 0; k < dimensions; k++)
            {
                embedding[k] += hidden[0, token, k] / length;
            }
        }

        var norm = MathF.Sqrt(embedding.Sum(x => x * x));
        for (var k = 0; k < dimensions; k++)
        {
            embedding[k] /= norm;
        }

        return embedding;
    }

    public void Dispose() => session.Dispose();
}
