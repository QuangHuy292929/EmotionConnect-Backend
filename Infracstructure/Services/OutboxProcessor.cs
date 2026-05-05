using System.Text.Json;
using Application.DTOs.Notification;
using Application.DTOs.OutboxMessage;
using Application.DTOs.OutboxMessage.Payloads;
using Application.Interfaces.Common;
using Application.Interfaces.IServices;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infracstructure.Services;

public class OutboxProcessor : IOutBoxProcessor
{
    private readonly IOutboxMessageService _outboxMessageService;
    private readonly INotificationService _notificationService;
    private readonly IAchievementService _achievementService;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IOutboxMessageService outboxMessageService,
        INotificationService notificationService,
        IAchievementService achievementService,
        ILogger<OutboxProcessor> logger)
    {
        _outboxMessageService = outboxMessageService;
        _notificationService = notificationService;
        _achievementService = achievementService;
        _logger = logger;
    }

    public async Task ProcessPendingAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _outboxMessageService.GetPendingAsync(20, cancellationToken);

        foreach (var message in messages)
        {
            await ProcessMessageAsync(message, cancellationToken);
        }
    }

    public async Task ProcessMessageAsync(
        OutboxMessageDto message,
        CancellationToken cancellationToken)
    {
        try
        {
            await _outboxMessageService.MarkProcessingAsync(message.Id, cancellationToken);

            switch (message.EventType)
            {
                case nameof(EventType.FriendRequestSent):
                    await HandleFriendRequestSentAsync(message, cancellationToken);
                    break;

                case nameof(EventType.FriendRequestAccepted):
                    await HandleFriendRequestAcceptedAsync(message, cancellationToken);
                    break;

                case nameof(EventType.AchievementUnlocked):
                    await HandleAchievementUnlockedAsync(message, cancellationToken);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported outbox event type: {message.EventType}");
            }

            await _outboxMessageService.MarkProcessedAsync(message.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process outbox message {OutboxMessageId}", message.Id);
            await _outboxMessageService.MarkFailedAsync(message.Id, ex.Message, cancellationToken);
        }
    }

    public async Task HandleFriendRequestSentAsync(
        OutboxMessageDto message,
        CancellationToken cancellationToken)
    {
        var payload = DeserializePayload<FriendRequestPayload>(message.PayloadJson);

        await _notificationService.CreateAsync(new CreateNotificationRequestDto
        {
            UserId = payload.AddresseeId,
            Type = NotificationType.FriendRequestReceived,
            Title = "New friend request",
            Body = "You received a new friend request.",
            PayloadJson = message.PayloadJson
        }, cancellationToken);
    }

    public async Task HandleFriendRequestAcceptedAsync(
        OutboxMessageDto message,
        CancellationToken cancellationToken)
    {
        var payload = DeserializePayload<FriendRequestPayload>(message.PayloadJson);

        await _notificationService.CreateAsync(new CreateNotificationRequestDto
        {
            UserId = payload.RequesterId,
            Type = NotificationType.FriendRequestAccepted,
            Title = "Friend request accepted",
            Body = "Your friend request was accepted.",
            PayloadJson = message.PayloadJson
        }, cancellationToken);

        await _achievementService.IncrementProgressAsync(payload.RequesterId, "FIRST_FRIEND", 1, cancellationToken);
        await _achievementService.IncrementProgressAsync(payload.AddresseeId, "FIRST_FRIEND", 1, cancellationToken);
    }

    public async Task HandleAchievementUnlockedAsync(
        OutboxMessageDto message,
        CancellationToken cancellationToken)
    {
        var payload = DeserializePayload<AchievementUnlockedPayload>(message.PayloadJson);

        await _notificationService.CreateAsync(new CreateNotificationRequestDto
        {
            UserId = payload.UserId,
            Type = NotificationType.AchievementUnlocked,
            Title = "Achievement unlocked",
            Body = $"You unlocked achievement: {payload.AchievementCode}.",
            PayloadJson = message.PayloadJson
        }, cancellationToken);
    }

    private static T DeserializePayload<T>(string payloadJson)
    {
        var payload = JsonSerializer.Deserialize<T>(payloadJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (payload is null)
        {
            throw new InvalidOperationException($"Failed to deserialize payload to {typeof(T).Name}.");
        }

        return payload;
    }
}
