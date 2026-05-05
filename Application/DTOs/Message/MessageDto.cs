namespace Application.DTOs.Message;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid SenderId { get; set; }
    public string? SenderUsername { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
