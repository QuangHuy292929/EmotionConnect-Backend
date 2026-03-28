using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IReflectionRepository
    {
        Task AddAsync(Reflection reflection, CancellationToken cancellationToken = default);
        Task<List<Reflection>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
        Task<List<Reflection>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
