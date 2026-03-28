namespace Application.DTOs.Matching;

public class MatchingCandidateDto
{
    public Guid Id { get; set; }
    public Guid? CandidateUserId { get; set; }
    public Guid? CandidateRoomId { get; set; }
    public string MatchType { get; set; } = string.Empty;
    public decimal SimilarityScore { get; set; }
    public string? MatchReason { get; set; }
    public int Rank { get; set; }
}
