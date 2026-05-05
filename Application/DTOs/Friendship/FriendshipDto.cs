using Application.DTOs.Auth;

namespace Application.DTOs.Friendship;

public class FriendshipDto
{
    public Guid Id { get; set; }
    public Guid RequesterId { get; set; }
    public Guid AddresseeId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? BlockedAt { get; set; }
    public UserSummaryDto Requester { get; set; } = new();
    public UserSummaryDto Addressee { get; set; } = new();
}
