namespace Application.DTOs.Matching;

public class WaitingRoomMatchDto
{
    public Guid RoomId { get; set; }
    public decimal BestSimilarityScore { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
