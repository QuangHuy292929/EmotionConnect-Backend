using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class CommunityRepository : ICommunityRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CommunityRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Community>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Communities
            .Include(x => x.Members)
            .Include(x => x.Rooms)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Community?> GetByIdAsync(Guid communityId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Communities
            .Include(x => x.Members)
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == communityId, cancellationToken);
    }

    public Task<Community?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return _dbContext.Communities
            .Include(x => x.Members)
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
    }

    public Task<bool> IsUserJoinedAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.CommunityMembers
            .AnyAsync(x => x.CommunityId == communityId && x.UserId == userId, cancellationToken);
    }

    public Task AddMemberAsync(CommunityMember member, CancellationToken cancellationToken = default)
    {
        return _dbContext.CommunityMembers.AddAsync(member, cancellationToken).AsTask();
    }

    public Task RemoveMemberAsync(CommunityMember member)
    {
        _dbContext.CommunityMembers.Remove(member);
        return Task.CompletedTask;
    }

    public Task<List<Community>> GetJoinedCommunitiesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Communities
            .Include(x => x.Members)
            .Include(x => x.Rooms)
            .Where(x => x.Members.Any(m => m.UserId == userId))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<CommunityMember?> GetMemberAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.CommunityMembers
            .FirstOrDefaultAsync(x => x.CommunityId == communityId && x.UserId == userId, cancellationToken);
    }
}
