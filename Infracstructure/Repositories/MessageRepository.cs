using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MessageRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _dbContext.Messages.AddAsync(message, cancellationToken);
    }

    public async Task<Message?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Messages
            .Include(x => x.Sender)
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
    }

    public async Task<List<Message>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Messages
            .Include(x => x.Sender)
            .Where(x => x.RoomId == roomId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
