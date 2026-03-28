using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IEmotionRepository
    {
        Task AddEmotionEntryAsync(EmotionEntry emotionEntry, CancellationToken cancellationToken = default);
        Task AddEmotionScoresAsync(IEnumerable<EmotionScore> scores, CancellationToken cancellationToken = default);
        Task AddEmbeddingAsync(TextEmbedding embedding, CancellationToken cancellationToken = default);
        Task<EmotionEntry?> GetByIdAsync(Guid emotionEntryId, CancellationToken cancellationToken = default);
        Task<List<EmotionEntry>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
