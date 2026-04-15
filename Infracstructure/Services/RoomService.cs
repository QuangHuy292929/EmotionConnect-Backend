using Application.DTOs.Room;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Extensions;
using Infracstructure.Mappers;

namespace Infracstructure.Services;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomDto> CreateAsync(CreateRoomRequest request, Guid createByUserId, CancellationToken ct = default)
    {
        if (request is null)
        {
            throw new BadRequestException($"Request is not allow null. {nameof(request)}");
        }

        if (createByUserId == Guid.Empty)
        {
            throw new BadRequestException($"CreateByUserId is required {nameof(createByUserId)}");
        }

        if (request.MaxMembers <= 0)
        {
            throw new BadRequestException($"MaxMembers must be greater than 0. {nameof(request.MaxMembers)}");
        }

        if (!request.RoomType.TryToEnum(out RoomType roomType))
        {
            throw new BadRequestException($"RoomType '{request.RoomType}' is invalid.");
        }

        var room = new Room
        {
            Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
            CommunityId = request.CommunityId,
            CreatedById = createByUserId,
            MaxMembers = request.MaxMembers,
            RoomType = roomType,
            Status = RoomStatus.Open
        };

        await _unitOfWork.RoomRepository.AddAsync(room, ct);

        var creatorMember = new RoomMember
        {
            RoomId = room.Id,
            UserId = createByUserId
        };

        await _unitOfWork.RoomRepository.AddMemberAsync(creatorMember, ct);
        await _unitOfWork.SaveChangeAsync(ct);

        var createdRoom = await _unitOfWork.RoomRepository.GetByIdAsync(room.Id, ct);
        return (createdRoom ?? room).ToDto();
    }

    public async Task<List<RoomDto>> GetByCommunityAsync(Guid coomunityId, CancellationToken ct = default)
    {
        if (coomunityId == Guid.Empty)
        {
            throw new BadRequestException($"CommunityId is required. {nameof(coomunityId)}");
        }

        var rooms = await _unitOfWork.RoomRepository.GetByCommunityIdAsync(coomunityId, ct);
        return rooms.ToDtoList();
    }

    public async Task<RoomDto?> GetByIdAsync(Guid roomId, CancellationToken ct = default)
    {
        if (roomId == Guid.Empty)
        {
            throw new BadRequestException($"RoomId is required. {nameof(roomId)}" );
        }

        var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId, ct);
        return room?.ToDto();
    }

    public async Task<List<RoomDto>> GetMyRoomsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException($"UserId is required. {nameof(userId)}");
        }

        var rooms = await _unitOfWork.RoomRepository.GetByUserIdAync(userId, cancellationToken);
        return rooms.ToDtoList();
    }

    public async Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.RoomRepository.IsUserInRoomAsync(roomId, userId, cancellationToken);
    }

    public async Task JoinRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            throw new NotFoundException("Room not found.");
        }

        var alreadyInRoom = await _unitOfWork.RoomRepository.IsUserInRoomAsync(roomId, userId, cancellationToken);
        if (alreadyInRoom)
        {
            return;
        }

        if (room.Members.Count >= room.MaxMembers)
        {
            throw new ConflictException("Room is full.");
        }

        var member = new RoomMember
        {
            RoomId = roomId,
            UserId = userId
        };

        await _unitOfWork.RoomRepository.AddMemberAsync(member, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
    }

    public async Task LeaveRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (roomId == Guid.Empty)
        {
            throw new BadRequestException($"RoomId is required. {nameof(roomId)}" );
        }

        if (userId == Guid.Empty)
        {
            throw new BadRequestException($"UserId is required. {nameof(userId)}");
        }

        var member = await _unitOfWork.RoomRepository.GetRoomMemberAsync(roomId, userId, cancellationToken);
        if (member is null)
        {
            return;
        }

        await _unitOfWork.RoomRepository.RemoveMemberAsync(member, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
    }


}
