using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;
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
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == roomId, ct);
    }

    public async Task<List<Room>> GetByUserIdAync(Guid userId, RoomType? roomType = null, CancellationToken ct = default)
    {
        var query = _dbContext.Rooms
            .Include(x => x.Members)
            .Where(x => x.Members.Any(m =>
                m.UserId == userId &&
                m.MemberState == RoomMemberState.Active));

        if (roomType.HasValue)
        {
            query = query.Where(x => x.RoomType == roomType.Value);
        }

        return await query
            .OrderByDescending(x => x.Messages
                .Where(m => m.DeletedAt == null)
                .Select(m => (DateTime?)m.CreatedAt)
                .Max() ?? x.CreatedAt)
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
    // Lấy tất cả thành viên của phòng

    public async Task<List<RoomMember>> GetRoomMembersAsync(Guid roomId, CancellationToken ct = default)
    {
        return await _dbContext.RoomMembers
            .Where(x => x.RoomId == roomId)
            .Include(x => x.User)
            .ToListAsync(ct);
    }

    public async Task<int> GetMemberCountAsync(Guid roomId, CancellationToken ct = default)
    {
        return await _dbContext.RoomMembers
            .CountAsync(
                x => x.RoomId == roomId && x.MemberState == RoomMemberState.Active,
                ct);
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

    public async Task<Room?> GetAiRoomByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _dbContext.Rooms
            .Include(x => x.Members)
            .FirstOrDefaultAsync(
                x => x.RoomType == RoomType.AiPrivate &&
                     x.Members.Any(m => m.UserId == userId && m.MemberState == RoomMemberState.Active),
                ct);
    }

    public async Task<Room?> GetDirectRoomBetweenUsersAsync(Guid userAId, Guid userBId, CancellationToken ct = default)
    {
        var lowId = userAId.CompareTo(userBId) <= 0 ? userAId : userBId;
        var highId = userAId.CompareTo(userBId) <= 0 ? userBId : userAId;

        return await _dbContext.Rooms
                        .Include(x => x.Members)
                        .FirstOrDefaultAsync(x => x.RoomType == RoomType.Direct && x.UserLowId == lowId && x.UserHighId == highId, ct);
    }

}

