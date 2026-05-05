using Application.DTOs.Notification;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Common;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Mappers;

namespace Infracstructure.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRealtimePublisher _notificationRealtimePublisher;

    public NotificationService(
        IUnitOfWork unitOfWork,
        INotificationRealtimePublisher notificationRealtimePublisher)
    {
        _unitOfWork = unitOfWork;
        _notificationRealtimePublisher = notificationRealtimePublisher;
    }

    public async Task<NotificationDto> CreateAsync(
        CreateNotificationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureUserExistsAsync(request.UserId, cancellationToken);
        ValidateTitle(request.Title);

        var notification = new Notification
        {
            UserId = request.UserId,
            Type = request.Type,
            Title = request.Title.Trim(),
            Body = NormalizeOptional(request.Body),
            PayloadJson = NormalizeOptional(request.PayloadJson),
            IsRead = false
        };

        await _unitOfWork.NotificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        var notificationDto = notification.ToDto();
        await _notificationRealtimePublisher.PublishCreatedAsync(notificationDto, cancellationToken);

        return notificationDto;
    }

    public async Task<List<NotificationDto>> CreateRangeAsync(
        IEnumerable<CreateNotificationRequestDto> requests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        var requestList = requests.ToList();
        foreach (var request in requestList)
        {
            if (request is null)
            {
                throw new BadRequestException("Notification request cannot be null.");
            }

            await EnsureUserExistsAsync(request.UserId, cancellationToken);
            ValidateTitle(request.Title);
        }

        var notifications = requestList.Select(request => new Notification
        {
            UserId = request.UserId,
            Type = request.Type,
            Title = request.Title.Trim(),
            Body = NormalizeOptional(request.Body),
            PayloadJson = NormalizeOptional(request.PayloadJson),
            IsRead = false
        }).ToList();

        await _unitOfWork.NotificationRepository.AddRangeAsync(notifications, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        var notificationDtos = notifications.ToDtoList();
        await _notificationRealtimePublisher.PublishCreatedRangeAsync(notificationDtos, cancellationToken);

        return notificationDtos;
    }

    public async Task<NotificationDto> GetByIdAsync(
        Guid notificationId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateGuid(notificationId, "Notification ID cannot be empty.");
        await EnsureUserExistsAsync(currentUserId, cancellationToken);

        var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
        {
            throw new NotFoundException("Notification not found.");
        }

        EnsureOwnership(notification, currentUserId);
        return notification.ToDto();
    }

    public async Task<List<NotificationDto>> GetMyNotificationsAsync(
        Guid currentUserId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(currentUserId, cancellationToken);

        if (skip < 0)
        {
            throw new BadRequestException("Skip cannot be negative.");
        }

        if (take <= 0)
        {
            throw new BadRequestException("Take must be greater than zero.");
        }

        var notifications = await _unitOfWork.NotificationRepository.GetByUserIdAsync(
            currentUserId,
            skip,
            take,
            cancellationToken);

        return notifications.ToDtoList();
    }

    public async Task<int> GetUnreadCountAsync(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(currentUserId, cancellationToken);
        return await _unitOfWork.NotificationRepository.GetUnreadCountAsync(currentUserId, cancellationToken);
    }

    public async Task<NotificationDto> MarkAsReadAsync(
        Guid notificationId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateGuid(notificationId, "Notification ID cannot be empty.");
        await EnsureUserExistsAsync(currentUserId, cancellationToken);

        var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
        {
            throw new NotFoundException("Notification not found.");
        }

        EnsureOwnership(notification, currentUserId);

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangeAsync(cancellationToken);
        }

        var notificationDto = notification.ToDto();
        await _notificationRealtimePublisher.PublishMarkedAsReadAsync(notificationDto, cancellationToken);

        return notificationDto;
    }

    public async Task<int> MarkAllAsReadAsync(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(currentUserId, cancellationToken);

        var unreadNotifications = await _unitOfWork.NotificationRepository.GetUnreadByUserIdAsync(
            currentUserId,
            cancellationToken);

        if (unreadNotifications.Count == 0)
        {
            return 0;
        }

        var readAt = DateTime.UtcNow;
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = readAt;
            notification.UpdatedAt = readAt;
        }

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        await _notificationRealtimePublisher.PublishMarkedAllAsReadAsync(currentUserId, unreadNotifications.Count, cancellationToken);
        return unreadNotifications.Count;
    }

    public async Task<List<NotificationDto>> GetByTypeAsync(Guid currentUserId, string type, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(currentUserId, cancellationToken);
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new BadRequestException("Notification type cannot be null or empty.");
        }

        if(!Enum.TryParse<NotificationType>(type, true, out var notificationType))
        {
            throw new BadRequestException("Invalid notification type.");
        }

        var notifications = await _unitOfWork.NotificationRepository.GetByTypeAsync(
            currentUserId,
            notificationType,
            skip,
            take,
            cancellationToken);

        return notifications.ToDtoList();
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        ValidateGuid(userId, "User ID cannot be empty.");

        if (!await _unitOfWork.AuthRepository.ExistsByIdAsync(userId, cancellationToken))
        {
            throw new NotFoundException($"User with ID {userId} not found.");
        }
    }

    private static void EnsureOwnership(Notification notification, Guid currentUserId)
    {
        if (notification.UserId != currentUserId)
        {
            throw new ForbiddenException("You do not have access to this notification.");
        }
    }

    private static void ValidateGuid(Guid value, string message)
    {
        if (value == Guid.Empty)
        {
            throw new BadRequestException(message);
        }
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BadRequestException("Notification title cannot be null or empty.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
