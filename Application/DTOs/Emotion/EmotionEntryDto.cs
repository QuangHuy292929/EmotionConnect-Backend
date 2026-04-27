namespace Application.DTOs.Emotion;

public class EmotionEntryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoomId { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string RawText { get; set; } = string.Empty;
    public string? NormalizedText { get; set; }
    public string? TopEmotion { get; set; }
    public decimal? TopEmotionScore { get; set; }
    public decimal? SentimentScore { get; set; }
    public string LanguageCode { get; set; } = "vi";
    public DateTime CreatedAt { get; set; }
    public List<EmotionScoreDto> Scores { get; set; } = new();
}
