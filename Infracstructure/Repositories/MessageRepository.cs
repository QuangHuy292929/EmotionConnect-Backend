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

    public Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        return _dbContext.Messages.AddAsync(message, cancellationToken).AsTask();
    }

    public Task<Message?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Messages
            .Include(x => x.Sender)
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
    }

    public Task<List<Message>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Messages
            .Include(x => x.Sender)
            .Where(x => x.RoomId == roomId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
