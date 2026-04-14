namespace Application.DTOs.Matching;

public class MatchingCandidateSeed
{
    public Guid CandidateUserId { get; set; }
    public decimal SimilarityScore { get; set; }
    public string MatchReason { get; set; } = string.Empty;
}
