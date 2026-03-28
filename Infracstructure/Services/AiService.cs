using Application.DTOs.AI;
using Application.Interfaces.Common;

namespace Infracstructure.Services;

public class AiService : IAiService
{
    public Task<AnalyzeResponseDto> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        var emotions = BuildPredictions(text);

        return Task.FromResult(new AnalyzeResponseDto
        {
            Text = text,
            TopEmotion = emotions.FirstOrDefault(),
            AllEmotions = emotions,
            SimilarSentences = new List<SemanticSearchResultDto>(),
            Vector = BuildVector(text)
        });
    }

    public Task<EmotionDetectionResponseDto> DetectEmotionAsync(string text, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new EmotionDetectionResponseDto
        {
            Text = text,
            Emotions = BuildPredictions(text)
        });
    }

    public Task<EmbeddingResponseDto> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new EmbeddingResponseDto
        {
            Text = text,
            Vector = BuildVector(text)
        });
    }

    private static List<EmotionPredictionDto> BuildPredictions(string text)
    {
        var normalized = text.Trim().ToLowerInvariant();

        var top = normalized switch
        {
            var t when t.Contains("buồn") || t.Contains("cô đơn") || t.Contains("tủi") => "sadness",
            var t when t.Contains("vui") || t.Contains("hạnh phúc") || t.Contains("biết ơn") => "joy",
            var t when t.Contains("giận") || t.Contains("tức") || t.Contains("khó chịu") => "anger",
            var t when t.Contains("lo") || t.Contains("sợ") || t.Contains("áp lực") || t.Contains("căng thẳng") => "fear",
            _ => "neutral"
        };

        return top switch
        {
            "sadness" => new List<EmotionPredictionDto>
            {
                new() { Label = "sadness", Score = 0.78 },
                new() { Label = "fear", Score = 0.14 },
                new() { Label = "neutral", Score = 0.08 }
            },
            "joy" => new List<EmotionPredictionDto>
            {
                new() { Label = "joy", Score = 0.81 },
                new() { Label = "neutral", Score = 0.11 },
                new() { Label = "sadness", Score = 0.08 }
            },
            "anger" => new List<EmotionPredictionDto>
            {
                new() { Label = "anger", Score = 0.76 },
                new() { Label = "fear", Score = 0.15 },
                new() { Label = "neutral", Score = 0.09 }
            },
            "fear" => new List<EmotionPredictionDto>
            {
                new() { Label = "fear", Score = 0.74 },
                new() { Label = "sadness", Score = 0.17 },
                new() { Label = "neutral", Score = 0.09 }
            },
            _ => new List<EmotionPredictionDto>
            {
                new() { Label = "neutral", Score = 0.72 },
                new() { Label = "sadness", Score = 0.15 },
                new() { Label = "joy", Score = 0.13 }
            }
        };
    }

    private static IReadOnlyList<float> BuildVector(string text)
    {
        const int dimension = 384;
        var vector = new float[dimension];
        var seed = text.Aggregate(17, (current, c) => current * 31 + c);
        var random = new Random(seed);

        for (var i = 0; i < dimension; i++)
        {
            vector[i] = (float)(random.NextDouble() * 2 - 1);
        }

        return vector;
    }
}
