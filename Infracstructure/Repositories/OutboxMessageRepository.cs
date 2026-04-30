using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OutboxMessageRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutBoxMessage message, CancellationToken cancellationToken = default)
    {
        await _dbContext.OutBoxMessages.AddAsync(message, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<OutBoxMessage> messages, CancellationToken cancellationToken = default)
    {
        await _dbContext.OutBoxMessages.AddRangeAsync(messages, cancellationToken);
    }

    public async Task<OutBoxMessage?> GetByIdAsync(Guid outboxMessageId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OutBoxMessages
            .FirstOrDefaultAsync(x => x.Id == outboxMessageId, cancellationToken);
    }

    public async Task<List<OutBoxMessage>> GetByStatusAsync(OutBoxStatus status, int take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OutBoxMessages
            .Where(x => x.Status == status)
            .OrderBy(x => x.OccurredAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
