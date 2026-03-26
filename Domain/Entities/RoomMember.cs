using Domain.Enums;

namespace Domain.Entities;

public class RoomMember : BaseEntity
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
    public RoomMemberState MemberState { get; set; } = RoomMemberState.Active;

    public Room Room { get; set; } = null!;
    public User User { get; set; } = null!;
}
