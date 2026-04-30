using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FriendshipRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Friendship friendship, CancellationToken cancellationToken = default)
    {
        await _dbContext.Friendships.AddAsync(friendship, cancellationToken);
    }

    public Task RemoveAsync(Friendship friendship, CancellationToken cancellationToken = default)
    {
        _dbContext.Friendships.Remove(friendship);
        return Task.CompletedTask;
    }

    public async Task<Friendship?> GetByIdAsync(Guid friendshipId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Friendships
            .Include(x => x.Requester)
            .Include(x => x.Addressee)
            .FirstOrDefaultAsync(x => x.Id == friendshipId, cancellationToken);
    }

    public async Task<Friendship?> GetByUsersAsync(Guid userAId, Guid userBId, CancellationToken cancellationToken = default)
    {
        var lowId = userAId.CompareTo(userBId) <= 0 ? userAId : userBId;
        var highId = userAId.CompareTo(userBId) <= 0 ? userBId : userAId;

        return await _dbContext.Friendships
            .Include(x => x.Requester)
            .Include(x => x.Addressee)
            .FirstOrDefaultAsync(
                x => x.UserLowId == lowId && x.UserHighId == highId,
                cancellationToken);
    }

    public async Task<bool> ExistsBetweenUsersAsync(Guid userAId, Guid userBId, CancellationToken cancellationToken = default)
    {
        var lowId = userAId.CompareTo(userBId) <= 0 ? userAId : userBId;
        var highId = userAId.CompareTo(userBId) <= 0 ? userBId : userAId;

        return await _dbContext.Friendships
            .AnyAsync(x => x.UserLowId == lowId && x.UserHighId == highId, cancellationToken);
    }

    public async Task<List<Friendship>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Friendships
            .Include(x => x.Requester)
            .Include(x => x.Addressee)
            .Where(x => x.RequesterId == userId || x.AddresseeId == userId)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Friendship>> GetIncomingByStatusAsync(Guid addresseeId, FriendshipStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Friendships
            .Include(x => x.Requester)
            .Where(x => x.AddresseeId == addresseeId && x.Status == status)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Friendship>> GetOutgoingByStatusAsync(Guid requesterId, FriendshipStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Friendships
            .Include(x => x.Addressee)
            .Where(x => x.RequesterId == requesterId && x.Status == status)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }
}
