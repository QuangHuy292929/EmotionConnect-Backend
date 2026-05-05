using Application.DTOs.Notification;

namespace Application.Interfaces.Common;

public interface INotificationRealtimePublisher
{
    Task PublishCreatedAsync(NotificationDto notification, CancellationToken cancellationToken = default);
    Task PublishCreatedRangeAsync(IEnumerable<NotificationDto> notifications, CancellationToken cancellationToken = default);
    Task PublishMarkedAsReadAsync(NotificationDto notification, CancellationToken cancellationToken = default);
    Task PublishMarkedAllAsReadAsync(Guid userId, int updatedCount, CancellationToken cancellationToken = default);
}
