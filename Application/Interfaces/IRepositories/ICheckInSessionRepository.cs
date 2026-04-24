using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface ICheckInSessionRepository
    {
        Task AddAsync(CheckInSession session, CancellationToken cancellationToken = default);
        Task<CheckInSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task<CheckInSession?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<CheckInSession>> GetMySessions(Guid userId, CancellationToken ct = default);
    }
}
