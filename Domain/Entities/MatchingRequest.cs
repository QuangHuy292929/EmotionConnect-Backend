using Domain.Enums;

namespace Domain.Entities;

public class MatchingRequest : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid EmotionEntryId { get; set; }
    public MatchingRequestStatus RequestStatus { get; set; } = MatchingRequestStatus.Created;
    public Guid? AssignedRoomId { get; set; }
    public DateTime? QueuedAt { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public User User { get; set; } = null!;
    public EmotionEntry EmotionEntry { get; set; } = null!;
    public Room? AssignedRoom { get; set; }
    public ICollection<MatchingCandidate> Candidates { get; set; } = new List<MatchingCandidate>();
}
