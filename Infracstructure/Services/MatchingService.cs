using Application.DTOs.Matching;
using Application.DTOs.Room;
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
            throw new KeyNotFoundException("Did not find emotion entry belong to this user");
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
            EmotionEntryId = emotionEntryId,
            CommunityId = emtionEntry.CommunityId
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

    public async Task<RoomDto?> CreateRoomFromMatchingAsync(Guid userId, Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        await GetOwnedMatchingRequestAsync(matchingRequestId, userId, cancellationToken);

        var request = await _unitOfWork.MatchingRepository.GetRequestByIdAsync(matchingRequestId, cancellationToken);
        if (request == null)
        {
            throw new NoMatchFoundException("Did not found candidate request by this id");
        }

        var candidates = await _unitOfWork.MatchingRepository.GetCandidatesAsync(matchingRequestId, cancellationToken);
        var selectedCandidates = candidates
            .Where(x => x.CandidateUserId.HasValue)
            .Where(x => x.SimilarityScore >= 0.7m)
            .OrderBy(x => x.Rank)
            .Take(4)
            .ToList();

        if (!selectedCandidates.Any())
        {
            throw new NoMatchFoundException("Did not found any candidate with similarity score higher than threshold");
        }

        var room = new Room
        {
            CommunityId = request.CommunityId,
            Name = null,
            RoomType = RoomType.Matching,
            Status = RoomStatus.Open,
            MaxMembers = selectedCandidates.Count + 1,
            CreatedById = request.UserId
        };
        
        await _unitOfWork.RoomRepository.AddAsync(room, cancellationToken);

        //Create room and add room creator
        var members = new List<RoomMember>
        {
            new RoomMember
            {
                RoomId = room.Id,
                UserId = request.UserId
            }
        };

        //Add selected candidates to room members
        members.AddRange(selectedCandidates.Select(x => new RoomMember
        {
            RoomId = room.Id,
            UserId = x.CandidateUserId!.Value
        }));

        foreach (var member in members) 
        { 
            await _unitOfWork.RoomRepository.AddMemberAsync(member, cancellationToken);
        }
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        var createdRoom = await _unitOfWork.RoomRepository.GetByIdAsync(room.Id, cancellationToken);
        return createdRoom?.ToDto();
    }

    public async Task<List<MatchingCandidateDto>> GetCandidatesAsync(Guid userId, Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        await GetOwnedMatchingRequestAsync(matchingRequestId, userId, cancellationToken);

        var request = await _unitOfWork.MatchingRepository.GetRequestByIdAsync(matchingRequestId, cancellationToken);
        if (request == null)
        {
            throw new NoMatchFoundException("Did not found candidate request by this id");
        }

        var candidates = await _unitOfWork.MatchingRepository.GetCandidatesAsync(matchingRequestId, cancellationToken);
        return candidates.ToDtoList();
    }

    private async Task<MatchingRequest> GetOwnedMatchingRequestAsync(Guid matchingRequestId, Guid userId, CancellationToken cancellationToken = default)
    {
        var request = await _unitOfWork.MatchingRepository.GetRequestByIdAsync(matchingRequestId, cancellationToken);

        if (request is null)
        {
            throw new KeyNotFoundException("Matching request not found.");
        }

        if (request.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this matching request.");
        }

        return request;
    }

}
