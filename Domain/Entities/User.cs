using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity
{

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string? GoogleId { get; set; } = string.Empty;
    public bool IsGoogleAccount { get; set; } = false;

    public ICollection<Room> CreatedRooms { get; set; } = new List<Room>();
    public ICollection<RoomMember> RoomMemberships { get; set; } = new List<RoomMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<EmotionEntry> EmotionEntries { get; set; } = new List<EmotionEntry>();
    public ICollection<MatchingRequest> MatchingRequests { get; set; } = new List<MatchingRequest>();
    public ICollection<MatchingCandidate> MatchingCandidates { get; set; } = new List<MatchingCandidate>();
    public ICollection<Reflection> Reflections { get; set; } = new List<Reflection>();
    public ICollection<Friendship> FriendshipsInitiated { get; set; } = new List<Friendship>();
    public ICollection<Friendship> FriendshipsReceived { get; set; } = new List<Friendship>();
    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
