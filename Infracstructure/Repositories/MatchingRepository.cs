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

    public async Task AddRequestAsync(MatchingRequest request, CancellationToken cancellationToken = default)
    {
        await _dbContext.MatchingRequests.AddAsync(request, cancellationToken);
    }

    public async Task AddCandidatesAsync(IEnumerable<MatchingCandidate> candidates, CancellationToken cancellationToken = default)
    {
        await _dbContext.MatchingCandidates.AddRangeAsync(candidates, cancellationToken);
    }

    public async Task<MatchingRequest?> GetRequestByIdAsync(Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MatchingRequests
            .Include(x => x.Candidates)
            .FirstOrDefaultAsync(x => x.Id == matchingRequestId, cancellationToken);
    }

    public async Task<List<MatchingCandidate>> GetCandidatesAsync(Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MatchingCandidates
            .Include(x => x.CandidateUser)
            .Include(x => x.CandidateRoom)
            .Where(x => x.MatchingRequestId == matchingRequestId)
            .OrderBy(x => x.Rank)
            .ToListAsync(cancellationToken);
    }
}
