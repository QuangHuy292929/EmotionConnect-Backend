using Application.DTOs.Notification;
using Domain.Entities;

namespace Infracstructure.Mappers;

public static class NotificationMapper
{
    public static NotificationDto ToDto(this Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type.ToString(),
            Title = notification.Title,
            Body = notification.Body,
            PayloadJson = notification.PayloadJson,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }

    public static List<NotificationDto> ToDtoList(this IEnumerable<Notification> notifications)
    {
        return notifications.Select(ToDto).ToList();
    }
}
