namespace Application.DTOs.Matching;

public class MatchQueueResultDto
{
    public Guid MatchingRequestId { get; set; }
    public Guid RoomId { get; set; }
    public string RequestStatus { get; set; } = string.Empty;
    public string RoomStatus { get; set; } = string.Empty;
    public bool JoinedExistingRoom { get; set; }
    public int MemberCount { get; set; }
    public int MinMembers { get; set; }
    public int MaxMembers { get; set; }
    public bool CanEnterRoom { get; set; }
}
