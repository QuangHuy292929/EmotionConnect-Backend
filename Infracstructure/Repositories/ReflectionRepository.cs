using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class ReflectionRepository : IReflectionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReflectionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Reflection reflection, CancellationToken cancellationToken = default)
    {
        return _dbContext.Reflections.AddAsync(reflection, cancellationToken).AsTask();
    }

    public Task<List<Reflection>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Reflections
            .Include(x => x.User)
            .Include(x => x.EmotionEntry)
            .Where(x => x.RoomId == roomId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Reflection>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Reflections
            .Include(x => x.Room)
            .Include(x => x.EmotionEntry)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
