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
            CreatedById = createByUserId,
            MaxMembers = request.MaxMembers,
            RoomType = roomType,
            Status = RoomStatus.Waiting
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

    public async Task<RoomDto?> GetByIdAsync(Guid roomId, CancellationToken ct = default)
    {
        if (roomId == Guid.Empty)
        {
            throw new BadRequestException($"RoomId is required. {nameof(roomId)}");
        }

        var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId, ct);
        return room?.ToDto();
    }

    public async Task<List<RoomDto>> GetMyRoomsAsync(Guid userId, string roomType, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException($"UserId is required. {nameof(userId)}");
        }

        if (!Enum.TryParse<RoomType>(roomType, true, out var roomTypeParsed))
        {
            throw new ConflictException("Check in input mode is invalid");
        }

        var rooms = await _unitOfWork.RoomRepository.GetByUserIdAync(userId, roomTypeParsed, cancellationToken);
        return rooms.ToDtoList();
    }

    public async Task<bool> IsUserInRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.RoomRepository.IsUserInRoomAsync(roomId, userId, cancellationToken);
    }

    public async Task<List<RoomMemberDto>> GetRoomMembersAsync(Guid roomId, CancellationToken cancellationToken = default)

    {
        if (roomId == Guid.Empty)
        {
            throw new BadRequestException($"RoomId is required. {nameof(roomId)}");
        }

        var members = await _unitOfWork.RoomRepository.GetRoomMembersAsync(roomId, cancellationToken);
        return members.ToDtoList();
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

    public async Task<RoomDto> GetOrCreateDirectRoomAsync(Guid currentUserId, Guid otherUserId, CancellationToken cancellationToken = default)
    {
        if (otherUserId == Guid.Empty)
        {
            throw new BadRequestException($"OtherUserId is required. {nameof(otherUserId)}");
        }
        if (currentUserId == otherUserId)
        {
            throw new BadRequestException("CurrentUserId and OtherUserId cannot be the same.");
        }

        var friendship = await _unitOfWork.FriendshipRepository.GetByUsersAsync(currentUserId, otherUserId, cancellationToken);
        if(friendship is null || friendship.Status != FriendshipStatus.Accepted)
        {
            throw new ForbiddenException("Direct chat is only available between accepted friend");
        }


        var existingRoom = await _unitOfWork.RoomRepository.GetDirectRoomBetweenUsersAsync(currentUserId, otherUserId, cancellationToken);
        if (existingRoom != null)
        {
            return existingRoom.ToDto();
        }

        var lowId = currentUserId.CompareTo(otherUserId) <= 0 ? currentUserId : otherUserId;
        var highId = currentUserId.CompareTo(otherUserId) <= 0 ? otherUserId : currentUserId;

        var room = new Room
        {
            Name = null,
            RoomType = RoomType.Direct,
            Status = RoomStatus.Active,
            CreatedById = currentUserId,
            MaxMembers = 2,
            UserLowId = lowId,
            UserHighId = highId
        };
        await _unitOfWork.RoomRepository.AddAsync(room, cancellationToken);

        var member1 = new RoomMember
        {
            RoomId = room.Id,
            UserId = currentUserId,
            MemberState = RoomMemberState.Active
        };
        var member2 = new RoomMember
        {
            RoomId = room.Id,
            UserId = otherUserId,
            MemberState = RoomMemberState.Active
        };
        await _unitOfWork.RoomRepository.AddMemberAsync(member1, cancellationToken);
        await _unitOfWork.RoomRepository.AddMemberAsync(member2, cancellationToken);

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        var createdRoom = await _unitOfWork.RoomRepository.GetByIdAsync(room.Id, cancellationToken);
        return (createdRoom ?? room).ToDto();
    }
}

