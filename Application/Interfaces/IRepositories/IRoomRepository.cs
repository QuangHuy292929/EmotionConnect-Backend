using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid roomId, CancellationToken ct = default);
        Task<List<Room>> GetByUserIdAync(Guid userId, RoomType? roomType, CancellationToken ct = default);
        Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId, CancellationToken ct = default);
        Task<RoomMember?> GetRoomMemberAsync(Guid roomId, Guid userId, CancellationToken ct = default);
        Task<List<RoomMember>> GetRoomMembersAsync(Guid roomId, CancellationToken ct = default);
        Task AddAsync(Room room, CancellationToken ct = default);
        Task AddMemberAsync(RoomMember roomMember, CancellationToken ct = default);
        Task RemoveMemberAsync(RoomMember roomMember, CancellationToken ct = default);
        Task<Room?> GetAiRoomByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<int> GetMemberCountAsync(Guid roomId, CancellationToken ct = default);
        Task<Room?> GetDirectRoomBetweenUsersAsync(Guid userAId, Guid userBId, CancellationToken ct = default);
    }
}
