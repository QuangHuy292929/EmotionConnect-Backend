using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Emotion;

namespace Application.Interfaces.IServices
{
    public interface IEmotionService
    {
        Task<EmotionAnalysisResultDto> CreateAndAnalyzeAsync(CreateEmotionEntryRequest request, Guid userId, CancellationToken cancellationToken = default);
        Task<EmotionEntryDto?> GetByIdAsync(Guid emotionEntryId, CancellationToken cancellationToken = default);
        Task<List<EmotionEntryDto>> GetMyEntriesAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
