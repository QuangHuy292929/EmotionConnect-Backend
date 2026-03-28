using Application.DTOs.Room;
using Application.Interfaces;
using Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<RoomDto> CreateAsync(CreateRoomRequest request, Guid createByUserId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<RoomDto>> GetByCommunityAsync(Guid coomunityId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<RoomDto?> GetByIdAsync(Guid roomId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<RoomDto>> GetMyRoomsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task JoinRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task LeaveRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
