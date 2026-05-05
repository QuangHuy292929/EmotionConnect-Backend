using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Room;

namespace Application.Interfaces.IServices
{
    public interface IRoomService
    {
        Task<RoomDto> CreateAsync(CreateRoomRequest request, Guid createByUserId, CancellationToken ct = default);
        Task<RoomDto?> GetByIdAsync(Guid roomId, CancellationToken ct = default);
        Task<List<RoomDto>> GetMyRoomsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task JoinRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);
        Task LeaveRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);
        Task<List<RoomMemberDto>> GetRoomMembersAsync(Guid roomId, CancellationToken cancellationToken = default);

        

    }
}
