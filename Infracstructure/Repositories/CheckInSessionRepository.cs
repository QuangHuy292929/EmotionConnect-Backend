using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Repositories
{
    public class CheckInSessionRepository : ICheckInSessionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CheckInSessionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(CheckInSession session, CancellationToken cancellationToken = default)
        {
            await _dbContext.CheckInSessions.AddAsync(session, cancellationToken);
        }

        public async Task<CheckInSession?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var activeStatuses = new[]
            {
                CheckInStatus.Started,
                CheckInStatus.InProgress,
                CheckInStatus.AwaitingConfirmation,
                CheckInStatus.Confirmed
            };

            return await _dbContext.CheckInSessions
                .Include(x => x.User)
                .Include(x => x.EmotionEntry)
                .Where(x => x.UserId == userId && activeStatuses.Contains(x.Status))
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<CheckInSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.CheckInSessions
            .Include(x => x.User)
            .Include(x => x.EmotionEntry)
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
        }
    }
}
