using Application.DTOs.Matching;
using Domain.Entities;

namespace Application.Interfaces.IRepositories;

public interface IMatchingRepository
{
    Task AddRequestAsync(MatchingRequest request, CancellationToken cancellationToken = default);
    Task AddCandidatesAsync(IEnumerable<MatchingCandidate> candidates, CancellationToken cancellationToken = default);
    Task<MatchingRequest?> GetRequestByIdAsync(Guid matchingRequestId, CancellationToken cancellationToken = default);
    Task<MatchingRequest?> GetLatestRequestByEmotionEntryIdAsync(Guid emotionEntryId, CancellationToken cancellationToken = default);
    Task<MatchingRequest?> GetRequestByIdForUserAsync(Guid matchingRequestId, Guid userId, CancellationToken cancellationToken = default);
    Task<List<MatchingCandidate>> GetCandidatesAsync(Guid matchingRequestId, CancellationToken cancellationToken = default);
    Task<List<MatchingCandidateSeed>> FindSimilarUsersAsync(Guid emotionEntryId, Guid userId, CancellationToken cancellationToken = default);
    Task<List<WaitingRoomMatchDto>> FindEligibleWaitingRoomsAsync(Guid matchingRequestId, Guid userId, CancellationToken cancellationToken = default);
    Task<Room?> LockWaitingRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<bool> IsUserInAnyOpenMatchingRoomAsync(Guid userId, CancellationToken cancellationToken = default);
}
