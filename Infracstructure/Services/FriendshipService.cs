using Application.DTOs.Friendship;
using Application.DTOs.OutboxMessage;
using Application.DTOs.OutboxMessage.Payloads;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Mappers;
using System.Text.Json;

namespace Infracstructure.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxMessageService _outboxMessageService;

    public FriendshipService(IUnitOfWork unitOfWork, IOutboxMessageService outboxMessageService)
    {
        _unitOfWork = unitOfWork;
        _outboxMessageService = outboxMessageService;
    }

    public async Task<FriendshipDto> SendRequestAsync(
        Guid requesterId,
        Guid addresseeId,
        CancellationToken cancellationToken = default)
    {
        ValidateUserIds(requesterId, addresseeId);

        if (requesterId == addresseeId)
        {
            throw new BadRequestException("You cannot send a friendship request to yourself.");
        }

        await EnsureUserExistsAsync(requesterId, cancellationToken);
        await EnsureUserExistsAsync(addresseeId, cancellationToken);

        var existingFriendship = await _unitOfWork.FriendshipRepository.GetByUsersAsync(
            requesterId,
            addresseeId,
            cancellationToken);

        if (existingFriendship is null)
        {
            var newFriendship = CreatePendingFriendship(requesterId, addresseeId);
            await _unitOfWork.FriendshipRepository.AddAsync(newFriendship, cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);
            await _outboxMessageService.EnqueueAsync(new CreateOutboxMessageRequestDto
            {
                EventType = EventType.FriendRequestSent,
                AggregateType = AggregateType.Friendship,
                AggregateId = newFriendship.Id,
                PayloadJson = SerializePayload(new FriendRequestPayload
                {
                    FriendshipId = newFriendship.Id,
                    RequesterId = requesterId,
                    AddresseeId = addresseeId
                })
            }, cancellationToken);

            return (await GetOwnedFriendshipAsync(newFriendship.Id, requesterId, cancellationToken)).ToDto();
        }

        if (existingFriendship.Status == FriendshipStatus.Blocked)
        {
            throw new ConflictException("This friendship is blocked.");
        }

        if (existingFriendship.Status == FriendshipStatus.Accepted)
        {
            throw new ConflictException("Users are already friends.");
        }

        if (existingFriendship.Status == FriendshipStatus.Pending)
        {
            throw new ConflictException("A pending friendship request already exists.");
        }

        existingFriendship.RequesterId = requesterId;
        existingFriendship.AddresseeId = addresseeId;
        existingFriendship.Status = FriendshipStatus.Pending;
        existingFriendship.RequestedAt = DateTime.UtcNow;
        existingFriendship.RespondedAt = null;
        existingFriendship.CancelledAt = null;
        existingFriendship.BlockedAt = null;
        Touch(existingFriendship);

        await _unitOfWork.SaveChangeAsync(cancellationToken);

        await _outboxMessageService.EnqueueAsync(new CreateOutboxMessageRequestDto
        {
            EventType = EventType.FriendRequestSent,
            AggregateType = AggregateType.Friendship,
            AggregateId = existingFriendship.Id,
            PayloadJson = SerializePayload(new FriendRequestPayload
            {
                FriendshipId = existingFriendship.Id,
                RequesterId = requesterId,
                AddresseeId = addresseeId
            })
        }, cancellationToken);

        return (await GetOwnedFriendshipAsync(existingFriendship.Id, requesterId, cancellationToken)).ToDto();
    }

    public async Task<FriendshipDto> AcceptRequestAsync(
        Guid friendshipId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var friendship = await GetOwnedFriendshipAsync(friendshipId, currentUserId, cancellationToken);

        if (friendship.AddresseeId != currentUserId)
        {
            throw new ForbiddenException("Only the addressee can accept this friendship request.");
        }

        if (friendship.Status != FriendshipStatus.Pending)
        {
            throw new ConflictException("Only pending friendship requests can be accepted.");
        }

        friendship.Status = FriendshipStatus.Accepted;
        friendship.RespondedAt = DateTime.UtcNow;
        Touch(friendship);

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        await _outboxMessageService.EnqueueAsync(new CreateOutboxMessageRequestDto
        {
            EventType = EventType.FriendRequestAccepted,
            AggregateType = AggregateType.Friendship,
            AggregateId = friendship.Id,
            PayloadJson = SerializePayload(new FriendRequestPayload
            {
                FriendshipId = friendship.Id,
                RequesterId = friendship.RequesterId,
                AddresseeId = friendship.AddresseeId
            })
        }, cancellationToken);

        return friendship.ToDto();
    }

    public async Task<FriendshipDto> RejectRequestAsync(
        Guid friendshipId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var friendship = await GetOwnedFriendshipAsync(friendshipId, currentUserId, cancellationToken);

        if (friendship.AddresseeId != currentUserId)
        {
            throw new ForbiddenException("Only the addressee can reject this friendship request.");
        }

        if (friendship.Status != FriendshipStatus.Pending)
        {
            throw new ConflictException("Only pending friendship requests can be rejected.");
        }

        friendship.Status = FriendshipStatus.Rejected;
        friendship.RespondedAt = DateTime.UtcNow;
        Touch(friendship);

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return friendship.ToDto();
    }

    public async Task<FriendshipDto> CancelRequestAsync(
        Guid friendshipId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var friendship = await GetOwnedFriendshipAsync(friendshipId, currentUserId, cancellationToken);

        if (friendship.RequesterId != currentUserId)
        {
            throw new ForbiddenException("Only the requester can cancel this friendship request.");
        }

        if (friendship.Status != FriendshipStatus.Pending)
        {
            throw new ConflictException("Only pending friendship requests can be cancelled.");
        }

        friendship.Status = FriendshipStatus.Cancelled;
        friendship.CancelledAt = DateTime.UtcNow;
        Touch(friendship);

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return friendship.ToDto();
    }

    public async Task<FriendshipDto> BlockAsync(
        Guid friendshipId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var friendship = await GetOwnedFriendshipAsync(friendshipId, currentUserId, cancellationToken);

        friendship.Status = FriendshipStatus.Blocked;
        friendship.BlockedAt = DateTime.UtcNow;
        Touch(friendship);

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return friendship.ToDto();
    }

    public async Task RemoveFriendshipAsync(
        Guid friendshipId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var friendship = await GetOwnedFriendshipAsync(friendshipId, currentUserId, cancellationToken);

        if (friendship.Status != FriendshipStatus.Accepted)
        {
            throw new ConflictException("Only accepted friendships can be removed.");
        }

        await _unitOfWork.FriendshipRepository.RemoveAsync(friendship, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
    }

    public async Task<FriendshipDto> GetByIdAsync(
        Guid friendshipId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var friendship = await GetOwnedFriendshipAsync(friendshipId, currentUserId, cancellationToken);
        return friendship.ToDto();
    }

    public async Task<FriendshipDto?> GetByUsersAsync(
        Guid currentUserId,
        Guid otherUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateUserIds(currentUserId, otherUserId);

        var friendship = await _unitOfWork.FriendshipRepository.GetByUsersAsync(
            currentUserId,
            otherUserId,
            cancellationToken);

        if (friendship is null)
        {
            return null;
        }

        EnsureParticipant(friendship, currentUserId);
        return friendship.ToDto();
    }

    public async Task<List<FriendshipDto>> GetIncomingRequestsAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateSingleUserId(currentUserId);

        var friendships = await _unitOfWork.FriendshipRepository.GetIncomingByStatusAsync(
            currentUserId,
            FriendshipStatus.Pending,
            cancellationToken);

        return friendships.ToDtoList();
    }

    public async Task<List<FriendshipDto>> GetOutgoingRequestsAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateSingleUserId(currentUserId);

        var friendships = await _unitOfWork.FriendshipRepository.GetOutgoingByStatusAsync(
            currentUserId,
            FriendshipStatus.Pending,
            cancellationToken);

        return friendships.ToDtoList();
    }

    public async Task<List<FriendshipDto>> GetFriendsAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateSingleUserId(currentUserId);

        var friendships = await _unitOfWork.FriendshipRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        return friendships
            .Where(x => x.Status == FriendshipStatus.Accepted)
            .ToList()
            .ToDtoList();
    }

    public async Task<bool> ExistsBetweenUsersAsync(
        Guid currentUserId,
        Guid otherUserId,
        CancellationToken cancellationToken = default)
    {
        ValidateUserIds(currentUserId, otherUserId);

        return await _unitOfWork.FriendshipRepository.ExistsBetweenUsersAsync(
            currentUserId,
            otherUserId,
            cancellationToken);
    }

    private async Task<Friendship> GetOwnedFriendshipAsync(
        Guid friendshipId,
        Guid currentUserId,
        CancellationToken cancellationToken)
    {
        ValidateSingleUserId(currentUserId);

        var friendship = await _unitOfWork.FriendshipRepository.GetByIdAsync(friendshipId, cancellationToken);
        if (friendship is null)
        {
            throw new NotFoundException("Friendship not found.");
        }

        EnsureParticipant(friendship, currentUserId);
        return friendship;
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.AuthRepository.ExistsByIdAsync(userId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }
    }

    private static Friendship CreatePendingFriendship(Guid requesterId, Guid addresseeId)
    {
            var lowId = requesterId.CompareTo(addresseeId) <= 0 ? requesterId : addresseeId;
            var highId = requesterId.CompareTo(addresseeId) <= 0 ? addresseeId : requesterId;

        return new Friendship
        {
            RequesterId = requesterId,
            AddresseeId = addresseeId,
            UserLowId = lowId,
            UserHighId = highId,
            Status = FriendshipStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };
    }

    private static void EnsureParticipant(Friendship friendship, Guid currentUserId)
    {
        if (friendship.RequesterId != currentUserId && friendship.AddresseeId != currentUserId)
        {
            throw new ForbiddenException("You do not have access to this friendship.");
        }
    }

    private static void Touch(Friendship friendship)
    {
        friendship.UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateUserIds(Guid userId1, Guid userId2)
    {
        ValidateSingleUserId(userId1);
        ValidateSingleUserId(userId2);
    }

    private static void ValidateSingleUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BadRequestException("User id cannot be empty.");
        }
    }

    private static string SerializePayload<T>(T payload)
    {
        return JsonSerializer.Serialize(payload);
    }

}
