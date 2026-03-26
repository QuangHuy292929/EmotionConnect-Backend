using Domain.Enums;

namespace Domain.Entities;

public class MatchingRequest : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid EmotionEntryId { get; set; }
    public Guid? CommunityId { get; set; }
    public MatchingRequestStatus RequestStatus { get; set; } = MatchingRequestStatus.Pending;
    public DateTime? ProcessedAt { get; set; }

    public User User { get; set; } = null!;
    public EmotionEntry EmotionEntry { get; set; } = null!;
    public Community? Community { get; set; }
    public ICollection<MatchingCandidate> Candidates { get; set; } = new List<MatchingCandidate>();
}
