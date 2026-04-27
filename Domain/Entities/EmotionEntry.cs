using Domain.Enums;

namespace Domain.Entities;

public class EmotionEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? RoomId { get; set; }
    public EmotionSourceType SourceType { get; set; }
    public string RawText { get; set; } = string.Empty;
    public string? NormalizedText { get; set; }
    public string? TopEmotion { get; set; }
    public decimal? TopEmotionScore { get; set; }
    public decimal? SentimentScore { get; set; }
    public string LanguageCode { get; set; } = "vi";

    public User User { get; set; } = null!;
    public Room? Room { get; set; }
    public ICollection<EmotionScore> Scores { get; set; } = new List<EmotionScore>();
    public TextEmbedding? Embedding { get; set; }
    public ICollection<MatchingRequest> MatchingRequests { get; set; } = new List<MatchingRequest>();
    public ICollection<Reflection> Reflections { get; set; } = new List<Reflection>();
}
