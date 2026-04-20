using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoomRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Room?> GetByIdAsync(Guid roomId, CancellationToken ct = default)
    {
         return await _dbContext.Rooms
            .Include(x => x.Community)
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == roomId, ct);
    }

    public async Task<List<Room>> GetByCommunityIdAsync(Guid communityId, CancellationToken ct = default)
    {
        return await _dbContext.Rooms
            .Include(x => x.Community)
            .Include(x => x.Members)
            .Where(x => x.CommunityId == communityId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Room>> GetByUserIdAync(Guid userId, CancellationToken ct = default)
    {
        return await _dbContext.RoomMembers
            .Include(x => x.Room)
                .ThenInclude(x => x!.Community)
            .Include(x => x.Room)
                .ThenInclude(x => x!.Members)
            .Where(x => x.UserId == userId)
            .Select(x => x.Room)
            .Distinct()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId, CancellationToken ct = default)
    {
        return await _dbContext.RoomMembers
            .AnyAsync(x => x.RoomId == roomId && x.UserId == userId, ct);
    }

    public async Task<RoomMember?> GetRoomMemberAsync(Guid roomId, Guid userId, CancellationToken ct = default)
    {
        return await _dbContext.RoomMembers
            .Where(x => x.RoomId == roomId && x.UserId == userId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddAsync(Room room, CancellationToken ct = default)
    {
        await _dbContext.Rooms.AddAsync(room, ct);
    }

    public async Task AddMemberAsync(RoomMember roomMember, CancellationToken ct = default)
    {
        await _dbContext.RoomMembers.AddAsync(roomMember, ct);
    }

    public Task RemoveMemberAsync(RoomMember roomMember, CancellationToken ct = default)
    {
        _dbContext.RoomMembers.Remove(roomMember);
        return Task.CompletedTask;
    }

    public Task<Room?> GetAiRoomByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
