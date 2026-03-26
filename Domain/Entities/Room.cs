using Domain.Enums;

namespace Domain.Entities;

public class Room : BaseEntity
{
    public Guid? CommunityId { get; set; }
    public string? Name { get; set; }
    public RoomType RoomType { get; set; } = RoomType.Matching;
    public RoomStatus Status { get; set; } = RoomStatus.Open;
    public int MaxMembers { get; set; } = 5;
    public Guid CreatedById { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Community? Community { get; set; }
    public User CreatedBy { get; set; } = null!;
    public ICollection<RoomMember> Members { get; set; } = new List<RoomMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<EmotionEntry> EmotionEntries { get; set; } = new List<EmotionEntry>();
    public ICollection<MatchingCandidate> CandidateMatches { get; set; } = new List<MatchingCandidate>();
    public ICollection<Reflection> Reflections { get; set; } = new List<Reflection>();
}
