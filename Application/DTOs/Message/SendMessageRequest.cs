namespace Application.DTOs.Message;

public class SendMessageRequest
{
    public Guid RoomId { get; set; }
    public string MessageType { get; set; } = "Text";
    public string Content { get; set; } = string.Empty;
}
