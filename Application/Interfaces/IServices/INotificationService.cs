using Application.DTOs.Notification;
using Domain.Enums;

namespace Application.Interfaces.IServices;

public interface INotificationService
{
    Task<NotificationDto> CreateAsync(CreateNotificationRequestDto request, CancellationToken cancellationToken = default);
    Task<List<NotificationDto>> CreateRangeAsync(IEnumerable<CreateNotificationRequestDto> requests, CancellationToken cancellationToken = default);
    Task<NotificationDto> GetByIdAsync(Guid notificationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<List<NotificationDto>> GetMyNotificationsAsync(Guid currentUserId, int skip = 0, int take = 20, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid currentUserId, CancellationToken cancellationToken = default);
    Task<List<NotificationDto>> GetByTypeAsync(Guid currentUserId, string type, int skip = 0, int take = 20, CancellationToken cancellationToken = default);
    Task<NotificationDto> MarkAsReadAsync(Guid notificationId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<int> MarkAllAsReadAsync(Guid currentUserId, CancellationToken cancellationToken = default);
}
