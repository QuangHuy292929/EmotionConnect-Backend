using Application.DTOs.OutboxMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Common;

public interface IOutBoxProcessor
{
    Task ProcessPendingAsync(CancellationToken ct);
    Task ProcessMessageAsync(OutboxMessageDto message, CancellationToken ct);
    Task HandleFriendRequestSentAsync(OutboxMessageDto message, CancellationToken ct);
    Task HandleFriendRequestAcceptedAsync(OutboxMessageDto message, CancellationToken ct);
    Task HandleAchievementUnlockedAsync(OutboxMessageDto message, CancellationToken ct);
}
