using Application.DTOs.Matching;
using Application.DTOs.Room;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Common;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Humanizer;
using Infracstructure.Mappers;

namespace Infracstructure.Services;

public class MatchingService : IMatchingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserPresenceService _userPresenceService;

    public MatchingService(IUnitOfWork unitOfWork, IUserPresenceService userPresenceService)
    {
        _unitOfWork = unitOfWork;
        _userPresenceService = userPresenceService;
    }

    public async Task<MatchingResultDto> CreateMatchingAsync(Guid emotionEntryId, Guid userId, CancellationToken cancellationToken = default)
    {
        var emtionEntry = await _unitOfWork.EmotionRepository.GetByIdAsync(emotionEntryId, cancellationToken);
        if (emtionEntry == null) {
            throw new NotFoundException("Did not find emotion entry belong to this user");
        };

        var candidates = await _unitOfWork.MatchingRepository.FindSimilarUsersAsync(emotionEntryId, userId, cancellationToken);

        //Lấy thông tin online và lấy thời gian hoạt động gần nhất của các candidate để ưu tiên người đang online và có thời gian hoạt động gần nhất lên trên
        var enrichedCandidates = new List<(MatchingCandidateSeed Candidate, bool IsOnline, DateTime? LastActiveAt)>();
        foreach (var candidate in candidates)
        {
            var isOnline = await _userPresenceService.IsOnlineAsync(candidate.CandidateUserId, cancellationToken);
            var lastActiveAt = await _userPresenceService.GetLastActiveAsync(candidate.CandidateUserId, cancellationToken);

            enrichedCandidates.Add((candidate, isOnline, lastActiveAt));
        }

        var filteredCandidates = enrichedCandidates
            .OrderByDescending(c => c.IsOnline) 
            .ThenByDescending(c => c.LastActiveAt)
            .ThenByDescending(c => c.Candidate.SimilarityScore)
            .Take(5)
            .Select(c => c.Candidate)
            .ToList();

        var request = new MatchingRequest
        {
            UserId = userId,
            EmotionEntryId = emotionEntryId
        };
        await _unitOfWork.MatchingRepository.AddRequestAsync(request, cancellationToken);

        var candidateEntities = filteredCandidates.Select((x, index) => new MatchingCandidate
        {
            MatchingRequestId = request.Id,
            CandidateUserId = x.CandidateUserId,
            SimilarityScore = x.SimilarityScore,
            Rank = index + 1,
            MatchType = Domain.Enums.MatchType.UserToUser,
            MatchReason = x.MatchReason
        });

        await _unitOfWork.MatchingRepository.AddCandidatesAsync(candidateEntities, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        return request.ToResultDto();
    }
       
    public async Task<MatchQueueResultDto> JoinOrCreateQueueAsync(
        Guid matchingRequestId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var request = await _unitOfWork.MatchingRepository.GetRequestByIdForUserAsync(
                matchingRequestId,
                userId,
                cancellationToken);

            if (request is null)
            {
                throw new NotFoundException("Matching request not found.");
            }

            EnsureQueueableRequest(request);

            //Nếu request đã được gắn room rồi thì load room và đồng bộ lại tráng tháy request nếu room đã ready sau đó return thẳng
            if (request.AssignedRoomId.HasValue)
            {
                var assignedRoom = await _unitOfWork.RoomRepository.GetByIdAsync(
                    request.AssignedRoomId.Value,
                    cancellationToken);

                if (assignedRoom is null)
                {
                    throw new NotFoundException("Assigned room not found.");
                }

                await SynchronizeRequestWithRoomAsync(request, assignedRoom, cancellationToken);
                await _unitOfWork.CommitAsync();

                return await BuildQueueResultAsync(
                    request,
                    assignedRoom,
                    joinedExistingRoom: true,
                    cancellationToken);
            }

            var waitingRoomMatches = await _unitOfWork.MatchingRepository.FindEligibleWaitingRoomsAsync(
                matchingRequestId,
                userId,
                cancellationToken);

            foreach (var waitingRoomMatch in waitingRoomMatches)
            {
                var lockedRoom = await _unitOfWork.MatchingRepository.LockWaitingRoomAsync(
                    waitingRoomMatch.RoomId,
                    cancellationToken);

                if (lockedRoom is null)
                {
                    continue;
                }

                var currentMemberCount = await _unitOfWork.RoomRepository.GetMemberCountAsync(
                    lockedRoom.Id,
                    cancellationToken);

                if (currentMemberCount >= lockedRoom.MaxMembers)
                {
                    continue;
                }

                await _unitOfWork.RoomRepository.AddMemberAsync(new RoomMember
                {
                    RoomId = lockedRoom.Id,
                    UserId = userId
                }, cancellationToken);

                var finalMemberCount = currentMemberCount + 1;
                ApplyQueueAssignment(request, lockedRoom, finalMemberCount);

                await _unitOfWork.SaveChangeAsync(cancellationToken);
                await _unitOfWork.CommitAsync();

                return await BuildQueueResultAsync(
                    request,
                    lockedRoom,
                    joinedExistingRoom: true,
                    cancellationToken);
            }
            

            //Trường hợp không join được room nào thì tạo room mới với trạng thái waiting
            var waitingRoom = new Room
            {
                Name = null,
                RoomType = RoomType.Matching,
                Status = RoomStatus.Waiting,
                MinMembers = 2,
                MaxMembers = 5,
                CreatedById = userId
            };

            await _unitOfWork.RoomRepository.AddAsync(waitingRoom, cancellationToken);
            await _unitOfWork.RoomRepository.AddMemberAsync(new RoomMember
            {
                RoomId = waitingRoom.Id,
                UserId = userId
            }, cancellationToken);

            request.AssignedRoomId = waitingRoom.Id;
            request.RequestStatus = MatchingRequestStatus.Queued;
            request.QueuedAt ??= DateTime.UtcNow;
            request.ProcessedAt = null;
            request.AssignedAt = null;

            await _unitOfWork.SaveChangeAsync(cancellationToken);
            await _unitOfWork.CommitAsync();

            return await BuildQueueResultAsync(
                request,
                waitingRoom,
                joinedExistingRoom: false,
                cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<MatchQueueStatusDto> GetQueueStatusAsync(
        Guid matchingRequestId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.MatchingRepository.GetRequestByIdForUserAsync(
            matchingRequestId,
            userId,
            cancellationToken);

        if (request is null)
        {
            throw new NotFoundException("Matching request not found.");
        }

        if (!request.AssignedRoomId.HasValue)
        {
            return new MatchQueueStatusDto
            {
                MatchingRequestId = request.Id,
                RoomId = null,
                RequestStatus = request.RequestStatus.ToString(),
                RoomStatus = null,
                MemberCount = 0,
                MinMembers = 0,
                MaxMembers = 0,
                CanEnterRoom = false
            };
        }

        var room = await _unitOfWork.RoomRepository.GetByIdAsync(request.AssignedRoomId.Value, cancellationToken);
        if (room is null)
        {
            throw new NotFoundException("Assigned room not found.");
        }

        await SynchronizeRequestWithRoomAsync(request, room, cancellationToken);

        var memberCount = await _unitOfWork.RoomRepository.GetMemberCountAsync(room.Id, cancellationToken);

        return new MatchQueueStatusDto
        {
            MatchingRequestId = request.Id,
            RoomId = room.Id,
            RequestStatus = request.RequestStatus.ToString(),
            RoomStatus = room.Status.ToString(),
            MemberCount = memberCount,
            MinMembers = room.MinMembers,
            MaxMembers = room.MaxMembers,
            CanEnterRoom = room.Status is RoomStatus.Ready or RoomStatus.Active
        };
    }

    public async Task LeaveQueueAsync(
        Guid matchingRequestId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var request = await _unitOfWork.MatchingRepository.GetRequestByIdForUserAsync(
                matchingRequestId,
                userId,
                cancellationToken);

            if (request is null)
            {
                throw new NotFoundException("Matching request not found.");
            }

            if (request.RequestStatus is MatchingRequestStatus.Cancelled or MatchingRequestStatus.Completed or MatchingRequestStatus.Exprired)
            {
                throw new ConflictException("Matching request is no longer active.");
            }

            Room? room = null;
            var memberCountBeforeRemoval = 0;

            if (request.AssignedRoomId.HasValue)
            {
                room = await _unitOfWork.RoomRepository.GetByIdAsync(request.AssignedRoomId.Value, cancellationToken);

                if (room is not null)
                {
                    if (room.Status == RoomStatus.Active)
                    {
                        throw new ConflictException("Cannot leave the queue after the room is active.");
                    }

                    var roomMember = await _unitOfWork.RoomRepository.GetRoomMemberAsync(room.Id, userId, cancellationToken);
                    if (roomMember is not null)
                    {
                        memberCountBeforeRemoval = await _unitOfWork.RoomRepository.GetMemberCountAsync(room.Id, cancellationToken);
                        await _unitOfWork.RoomRepository.RemoveMemberAsync(roomMember, cancellationToken);
                    }
                }
            }

            request.RequestStatus = MatchingRequestStatus.Cancelled;
            request.AssignedRoomId = null;
            request.ProcessedAt = DateTime.UtcNow;

            if (room is not null)
            {
                var remainingMembers = Math.Max(memberCountBeforeRemoval - 1, 0);

                if (remainingMembers == 0)
                {
                    room.Status = RoomStatus.Closed;
                    room.ClosedAt = DateTime.UtcNow;
                }
                else if (remainingMembers < room.MinMembers)
                {
                    room.Status = RoomStatus.Waiting;
                    room.ReadyAt = default;
                }
            }

            await _unitOfWork.SaveChangeAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<List<MatchingCandidateDto>> GetCandidatesAsync(Guid userId, Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        await GetOwnedMatchingRequestAsync(matchingRequestId, userId, cancellationToken);

        var request = await _unitOfWork.MatchingRepository.GetRequestByIdAsync(matchingRequestId, cancellationToken);
        if (request == null)
        {
            throw new NotFoundException("Did not found candidate request by this id");
        }

        var candidates = await _unitOfWork.MatchingRepository.GetCandidatesAsync(matchingRequestId, cancellationToken);
        return candidates.ToDtoList();
    }

    private async Task<MatchingRequest> GetOwnedMatchingRequestAsync(Guid matchingRequestId, Guid userId, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.MatchingRepository.GetRequestByIdAsync(matchingRequestId, cancellationToken);

        if (request is null)
        {
            throw new NotFoundException("Matching request not found.");
        }

        if (request.UserId != userId)
        {
            throw new ForbiddenException("You do not have access to this matching request.");
        }

        return request;
    }

    private static void EnsureQueueableRequest(MatchingRequest request)
    {
        if (request.RequestStatus is MatchingRequestStatus.Cancelled or MatchingRequestStatus.Completed or MatchingRequestStatus.Exprired)
        {
            throw new ConflictException("Matching request is no longer active.");
        }
    }

    private void ApplyQueueAssignment(MatchingRequest request, Room room, int finalMemberCount)
    {
        request.AssignedRoomId = room.Id;

        if (finalMemberCount >= room.MinMembers)
        {
            room.Status = RoomStatus.Ready;
            room.ReadyAt = DateTime.UtcNow;
            request.RequestStatus = MatchingRequestStatus.Assigned;
            request.AssignedAt = DateTime.UtcNow;
            request.ProcessedAt = DateTime.UtcNow;
        }
        else
        {
            request.RequestStatus = MatchingRequestStatus.Queued;
            request.QueuedAt ??= DateTime.UtcNow;
        }
    }

    private async Task SynchronizeRequestWithRoomAsync(
        MatchingRequest request,
        Room room,
        CancellationToken cancellationToken)
    {
        if (request.RequestStatus == MatchingRequestStatus.Queued &&
            room.Status is RoomStatus.Ready or RoomStatus.Active)
        {
            request.RequestStatus = MatchingRequestStatus.Assigned;
            request.AssignedAt ??= room.ReadyAt == default ? DateTime.UtcNow : room.ReadyAt;
            request.ProcessedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangeAsync(cancellationToken);
        }
    }

    private async Task<MatchQueueResultDto> BuildQueueResultAsync(
        MatchingRequest request,
        Room room,
        bool joinedExistingRoom,
        CancellationToken cancellationToken)
    {
        var memberCount = await _unitOfWork.RoomRepository.GetMemberCountAsync(room.Id, cancellationToken);

        return new MatchQueueResultDto
        {
            MatchingRequestId = request.Id,
            RoomId = room.Id,
            RequestStatus = request.RequestStatus.ToString(),
            RoomStatus = room.Status.ToString(),
            JoinedExistingRoom = joinedExistingRoom,
            MemberCount = memberCount,
            MinMembers = room.MinMembers,
            MaxMembers = room.MaxMembers,
            CanEnterRoom = room.Status is RoomStatus.Ready or RoomStatus.Active
        };
    }

}
