using Application.DTOs.Emotion;
using Application.DTOs.Matching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.CheckIn;

public class CheckInCompletedDto
{
    public Guid SessionId { get; set; }
    public Guid EmotionEntryId { get; set; }
    public string ConfirmedSummary { get; set; } = string.Empty;

    public string? TopEmotion { get; set; }
    public decimal? TopEmotionScore { get; set; }

    public List<EmotionScoreDto> AllEmotions { get; set; } = new();
    public List<float> Vector { get; set; } = new();

    public Guid? MatchingRequestId { get; set; }
    public List<MatchingCandidateDto> Candidates { get; set; } = new();
}
