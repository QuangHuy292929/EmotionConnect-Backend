using Application.DTOs.Emotion;
using Domain.Entities;

namespace Infracstructure.Mappers;

public static class EmotionMapper
{
    public static EmotionEntryDto ToDto(this EmotionEntry emotionEntry)
    {
        return new EmotionEntryDto
        {
            Id = emotionEntry.Id,
            UserId = emotionEntry.UserId,
            RoomId = emotionEntry.RoomId,
            SourceType = emotionEntry.SourceType.ToString(),
            RawText = emotionEntry.RawText,
            NormalizedText = emotionEntry.NormalizedText,
            TopEmotion = emotionEntry.TopEmotion,
            TopEmotionScore = emotionEntry.TopEmotionScore,
            SentimentScore = emotionEntry.SentimentScore,
            LanguageCode = emotionEntry.LanguageCode,
            CreatedAt = emotionEntry.CreatedAt,
            Scores = emotionEntry.Scores?.Select(ToDto).ToList() ?? new List<EmotionScoreDto>()
        };
    }

    public static EmotionScoreDto ToDto(this EmotionScore emotionScore)
    {
        return new EmotionScoreDto
        {
            Label = emotionScore.EmotionLabel,
            Score = emotionScore.Score
        };
    }

    public static List<EmotionEntryDto> ToDtoList(this IEnumerable<EmotionEntry> emotionEntries)
    {
        return emotionEntries.Select(ToDto).ToList();
    }
}
