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
        ICommunityRepository CommunityRepository { get; }
        IMatchingRepository MatchingRepository { get; }
        IMessageRepository MessageRepository { get; }
        IReflectionRepository ReflectionRepository { get; }
        IEmotionRepository EmotionRepository { get; }
        ICheckInSessionRepository CheckInSessionRepository { get; }

        Task<int> SaveChangeAsync(CancellationToken ct = default);
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
