namespace Application.DTOs.Message;

public class SendMessageRequest
{
    public Guid RoomId { get; set; }
    public string MessageType { get; set; } = "Text";
    public string Content { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }    // bytes

}
