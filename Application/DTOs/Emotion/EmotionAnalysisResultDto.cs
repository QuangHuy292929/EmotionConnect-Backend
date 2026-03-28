namespace Application.DTOs.Emotion;

public class EmotionAnalysisResultDto
{
    public Guid EmotionEntryId { get; set; }
    public string? TopEmotion { get; set; }
    public decimal? TopEmotionScore { get; set; }
    public decimal? SentimentScore { get; set; }
    public List<EmotionScoreDto> AllEmotions { get; set; } = new();
    public IReadOnlyList<float> Vector { get; set; } = Array.Empty<float>();
}
