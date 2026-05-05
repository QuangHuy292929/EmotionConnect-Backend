using Domain.Enums;

namespace Application.DTOs.Notification;

public class CreateNotificationRequestDto
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string? PayloadJson { get; set; }
}
