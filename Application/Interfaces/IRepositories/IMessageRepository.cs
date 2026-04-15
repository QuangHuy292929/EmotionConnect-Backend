using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message, CancellationToken cancellationToken = default);
        Task<Message?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default);
        Task<List<Message>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
        Task<List<Message>> GetPagedByRoomIdAsync(Guid roomId, int skip, int take, CancellationToken cancellationToken = default);
        Task<int> GetCountByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
        Task<List<Message>> SearchByRoomIdAsync(Guid roomId, string keyword, int skip, int take, CancellationToken cancellationToken = default);
        Task<int> CountSearchByRoomIdAsync(Guid roomId, string keyword, CancellationToken cancellationToken = default);
        Task<List<Message>> GetRecentByRoomIdAsync(Guid roomId, int take, CancellationToken cancellationToken = default);
    }
}
