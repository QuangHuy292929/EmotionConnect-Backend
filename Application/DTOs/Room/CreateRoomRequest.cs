namespace Application.DTOs.Room;

public class CreateRoomRequest
{
    public string? Name { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public int MaxMembers { get; set; } = 5;
}
