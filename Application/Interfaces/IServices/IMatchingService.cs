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
        Task<List<MatchingCandidateDto>> GetCandidatesAsync(Guid matchingRequestId, CancellationToken cancellationToken = default);
        Task<RoomDto?> CreateRoomFromMatchingAsync(Guid matchingRequestId, CancellationToken cancellationToken = default);
    }
}
