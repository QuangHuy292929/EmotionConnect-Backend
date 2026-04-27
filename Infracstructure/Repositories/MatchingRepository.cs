using Application.DTOs.Matching;
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

    public async Task<List<MatchingCandidateSeed>> FindSimilarUsersAsync(Guid emotionEntryId, Guid userId, CancellationToken cancellationToken = default)
    {
        FormattableString query = $@"
            WITH source_entry AS (
                SELECT ""Id"", ""CommunityId""
                FROM emotion_entries
                WHERE ""Id"" = {emotionEntryId}
            ),
            source_embedding AS (
                SELECT ""Embedding""
                FROM text_embeddings
                WHERE ""EmotionEntryId"" = {emotionEntryId}
            ),
            ranked_candidates AS (
                SELECT DISTINCT ON (candidate_entry.""UserId"")
                    candidate_entry.""UserId"" AS ""CandidateUserId"",
                    CAST(1 - (source_embedding.""Embedding"" <=> candidate_embedding.""Embedding"") AS numeric(6,5)) AS ""SimilarityScore"",
                    CASE
                        WHEN candidate_entry.""CommunityId"" IS NOT DISTINCT FROM source_entry.""CommunityId""
                            THEN 'Same community and similar emotional context.'
                        ELSE 'Similar emotional context.'
                    END AS ""MatchReason""
                FROM source_entry
                CROSS JOIN source_embedding
                JOIN text_embeddings candidate_embedding
                    ON candidate_embedding.""EmotionEntryId"" <> {emotionEntryId}
                JOIN emotion_entries candidate_entry
                    ON candidate_entry.""Id"" = candidate_embedding.""EmotionEntryId""
                WHERE candidate_entry.""UserId"" <> {userId}
                ORDER BY candidate_entry.""UserId"", source_embedding.""Embedding"" <=> candidate_embedding.""Embedding""
            )
            SELECT ""CandidateUserId"", ""SimilarityScore"", ""MatchReason""
            FROM ranked_candidates
            ORDER BY ""SimilarityScore"" DESC
            LIMIT 10";

        return await _dbContext.Set<MatchingCandidateSeed>()
            .FromSqlInterpolated(query)
            .ToListAsync(cancellationToken);

    }
}
