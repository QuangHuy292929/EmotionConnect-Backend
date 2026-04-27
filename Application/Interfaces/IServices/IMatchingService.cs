using Application.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Matching;

namespace Application.Interfaces.IServices
{
    public interface IMatchingService
    {
        Task<MatchingResultDto> CreateMatchingAsync(Guid emotionEntryId, Guid userId, CancellationToken cancellationToken = default);
        Task<List<MatchingCandidateDto>> GetCandidatesAsync(Guid userId, Guid matchingRequestId, CancellationToken cancellationToken = default);
        Task<MatchQueueResultDto> JoinOrCreateQueueAsync(Guid matchingRequestId, Guid userId, CancellationToken cancellationToken = default);
        Task<MatchQueueStatusDto> GetQueueStatusAsync(Guid matchingRequestId, Guid userId, CancellationToken cancellationToken = default);
        Task LeaveQueueAsync(Guid matchingRequestId, Guid userId, CancellationToken cancellationToken = default);
    }
}
