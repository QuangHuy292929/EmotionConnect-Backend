using Application.DTOs.Matching;
using Application.DTOs.Room;
using Application.Interfaces;
using Application.Interfaces.IServices;

namespace Infracstructure.Services;

public class MatchingService : IMatchingService
{
    private readonly IUnitOfWork _unitOfWork;

    public MatchingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<MatchingResultDto> CreateMatchingAsync(Guid emotionEntryId, Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RoomDto?> CreateRoomFromMatchingAsync(Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<MatchingCandidateDto>> GetCandidatesAsync(Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
