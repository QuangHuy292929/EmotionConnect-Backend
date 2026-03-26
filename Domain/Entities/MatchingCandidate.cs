using Domain.Enums;

namespace Domain.Entities;

public class MatchingCandidate : BaseEntity
{
    public Guid MatchingRequestId { get; set; }
    public Guid? CandidateUserId { get; set; }
    public Guid? CandidateRoomId { get; set; }
    public Domain.Enums.MatchType MatchType { get; set; }
    public decimal SimilarityScore { get; set; }
    public string? MatchReason { get; set; }
    public int Rank { get; set; }

    public MatchingRequest MatchingRequest { get; set; } = null!;
    public User? CandidateUser { get; set; }
    public Room? CandidateRoom { get; set; }
}
