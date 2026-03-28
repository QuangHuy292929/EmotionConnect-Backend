namespace Application.DTOs.Matching;

public class MatchingResultDto
{
    public Guid MatchingRequestId { get; set; }
    public Guid EmotionEntryId { get; set; }
    public List<MatchingCandidateDto> Candidates { get; set; } = new();
}
