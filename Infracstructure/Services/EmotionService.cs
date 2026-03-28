using Application.DTOs.Emotion;
using Application.Interfaces;
using Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Services
{
    public class EmotionService : IEmotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmotionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<EmotionAnalysisResultDto> CreateAndAnalyzeAsync(CreateEmotionEntryRequest request, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<EmotionEntryDto?> GetByIdAsync(Guid emotionEntryId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<EmotionEntryDto>> GetMyEntriesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
