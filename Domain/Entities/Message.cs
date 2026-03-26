using Domain.Enums;

namespace Domain.Entities;

public class Message : BaseEntity
{
    public Guid RoomId { get; set; }
    public Guid SenderId { get; set; }
    public MessageType MessageType { get; set; } = MessageType.Text;
    public string Content { get; set; } = string.Empty;
    public DateTime? EditedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Room Room { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
