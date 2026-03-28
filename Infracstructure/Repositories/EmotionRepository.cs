using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class EmotionRepository : IEmotionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmotionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddEmotionEntryAsync(EmotionEntry emotionEntry, CancellationToken cancellationToken = default)
    {
        return _dbContext.EmotionEntries.AddAsync(emotionEntry, cancellationToken).AsTask();
    }

    public Task AddEmotionScoresAsync(IEnumerable<EmotionScore> scores, CancellationToken cancellationToken = default)
    {
        return _dbContext.EmotionScores.AddRangeAsync(scores, cancellationToken);
    }

    public Task AddEmbeddingAsync(TextEmbedding embedding, CancellationToken cancellationToken = default)
    {
        return _dbContext.TextEmbeddings.AddAsync(embedding, cancellationToken).AsTask();
    }

    public Task<EmotionEntry?> GetByIdAsync(Guid emotionEntryId, CancellationToken cancellationToken = default)
    {
        return _dbContext.EmotionEntries
            .Include(x => x.Scores)
            .Include(x => x.Embedding)
            .Include(x => x.Community)
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x => x.Id == emotionEntryId, cancellationToken);
    }

    public Task<List<EmotionEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.EmotionEntries
            .Include(x => x.Scores)
            .Include(x => x.Embedding)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
