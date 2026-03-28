using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class MatchingRepository : IMatchingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MatchingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddRequestAsync(MatchingRequest request, CancellationToken cancellationToken = default)
    {
        return _dbContext.MatchingRequests.AddAsync(request, cancellationToken).AsTask();
    }

    public Task AddCandidatesAsync(IEnumerable<MatchingCandidate> candidates, CancellationToken cancellationToken = default)
    {
        return _dbContext.MatchingCandidates.AddRangeAsync(candidates, cancellationToken);
    }

    public Task<MatchingRequest?> GetRequestByIdAsync(Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        return _dbContext.MatchingRequests
            .Include(x => x.Candidates)
            .FirstOrDefaultAsync(x => x.Id == matchingRequestId, cancellationToken);
    }

    public Task<List<MatchingCandidate>> GetCandidatesAsync(Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        return _dbContext.MatchingCandidates
            .Include(x => x.CandidateUser)
            .Include(x => x.CandidateRoom)
            .Where(x => x.MatchingRequestId == matchingRequestId)
            .OrderBy(x => x.Rank)
            .ToListAsync(cancellationToken);
    }
}
