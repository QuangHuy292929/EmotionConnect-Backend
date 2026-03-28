using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IMatchingRepository
    {
        Task AddRequestAsync(MatchingRequest request, CancellationToken cancellationToken = default);
        Task AddCandidatesAsync(IEnumerable<MatchingCandidate> candidates, CancellationToken cancellationToken = default);
        Task<MatchingRequest?> GetRequestByIdAsync(Guid matchingRequestId, CancellationToken cancellationToken = default);
        Task<List<MatchingCandidate>> GetCandidatesAsync(Guid matchingRequestId, CancellationToken cancellationToken = default);
    }
}
