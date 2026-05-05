using Application.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository AuthRepository { get; }
        IRoomRepository RoomRepository { get; }
        IMatchingRepository MatchingRepository { get; }
        IMessageRepository MessageRepository { get; }
        IReflectionRepository ReflectionRepository { get; }
        IEmotionRepository EmotionRepository { get; }
        ICheckInSessionRepository CheckInSessionRepository { get; }
        IFriendshipRepository FriendshipRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IAchievementRepository AchievementRepository { get; }
        IUserAchievementRepository UserAchievementRepository { get; }
        IOutboxMessageRepository OutboxMessageRepository { get; }

        Task<int> SaveChangeAsync(CancellationToken ct = default);
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
