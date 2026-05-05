using Application.DTOs.Notification;
using Application.Interfaces.Common;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

public class NotificationRealtimePublisher : INotificationRealtimePublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationRealtimePublisher(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublishCreatedAsync(NotificationDto notification, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group(NotificationHub.GetUserGroupName(notification.UserId))
            .SendAsync("NotificationCreated", notification, cancellationToken);
    }

    public async Task PublishCreatedRangeAsync(IEnumerable<NotificationDto> notifications, CancellationToken cancellationToken = default)
    {
        foreach (var notification in notifications)
        {
            await PublishCreatedAsync(notification, cancellationToken);
        }
    }

    public async Task PublishMarkedAsReadAsync(NotificationDto notification, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group(NotificationHub.GetUserGroupName(notification.UserId))
            .SendAsync("NotificationMarkedAsRead", notification, cancellationToken);
    }

    public async Task PublishMarkedAllAsReadAsync(Guid userId, int updatedCount, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group(NotificationHub.GetUserGroupName(userId))
            .SendAsync("NotificationsMarkedAllAsRead", new
            {
                UserId = userId,
                UpdatedCount = updatedCount
            }, cancellationToken);
    }
}
