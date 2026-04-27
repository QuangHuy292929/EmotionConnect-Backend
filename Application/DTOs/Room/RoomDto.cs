namespace Application.DTOs.Room;

public class RoomDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int MaxMembers { get; set; }
    public int CurrentMemberCount { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}
