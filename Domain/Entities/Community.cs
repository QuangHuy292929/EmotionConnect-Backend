namespace Domain.Entities;

public class Community : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<CommunityMember> Members { get; set; } = new List<CommunityMember>();
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<EmotionEntry> EmotionEntries { get; set; } = new List<EmotionEntry>();
    public ICollection<MatchingRequest> MatchingRequests { get; set; } = new List<MatchingRequest>();
}
