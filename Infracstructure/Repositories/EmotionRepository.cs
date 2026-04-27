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

    public async Task AddEmotionEntryAsync(EmotionEntry emotionEntry, CancellationToken cancellationToken = default)
    {
        await _dbContext.EmotionEntries.AddAsync(emotionEntry, cancellationToken);
    }

    public async Task AddEmotionScoresAsync(IEnumerable<EmotionScore> scores, CancellationToken cancellationToken = default)
    {
        await _dbContext.EmotionScores.AddRangeAsync(scores, cancellationToken);
    }

    public async Task AddEmbeddingAsync(TextEmbedding embedding, CancellationToken cancellationToken = default)
    {
        await _dbContext.TextEmbeddings.AddAsync(embedding, cancellationToken);
    }

    public async Task<EmotionEntry?> GetByIdAsync(Guid emotionEntryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmotionEntries
            .Include(x => x.Scores)
            .Include(x => x.Embedding)
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x => x.Id == emotionEntryId, cancellationToken);
    }

    public async Task<List<EmotionEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmotionEntries
            .Include(x => x.Scores)
            .Include(x => x.Embedding)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }


}
