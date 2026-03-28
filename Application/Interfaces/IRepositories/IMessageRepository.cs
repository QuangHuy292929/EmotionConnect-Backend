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
    }
}
