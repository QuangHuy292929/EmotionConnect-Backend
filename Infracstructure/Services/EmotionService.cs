using Application.DTOs.Emotion;
using Application.Interfaces;
using Application.Interfaces.Common;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Extensions;
using Infracstructure.Mappers;
using Pgvector;

namespace Infracstructure.Services;

public class EmotionService : IEmotionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;

    public EmotionService(IUnitOfWork unitOfWork, IAiService aiService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
    }

    public async Task<EmotionAnalysisResultDto> CreateAndAnalyzeAsync(CreateEmotionEntryRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(request.RawText))
        {
            throw new ArgumentException("RawText is required.", nameof(request.RawText));
        }

        if (!request.SourceType.TryToEnum(out EmotionSourceType sourceType))
        {
            throw new ArgumentException($"SourceType '{request.SourceType}' is invalid.", nameof(request.SourceType));
        }

        var analysis = await _aiService.AnalyzeAsync(request.RawText.Trim(), cancellationToken);

        var emotionEntry = new EmotionEntry
        {
            UserId = userId,
            CommunityId = request.CommunityId,
            RoomId = request.RoomId,
            SourceType = sourceType,
            RawText = request.RawText.Trim(),
            TopEmotion = analysis.TopEmotion?.Label,
            TopEmotionScore = analysis.TopEmotion is null ? null : Convert.ToDecimal(analysis.TopEmotion.Score),
            LanguageCode = string.IsNullOrWhiteSpace(request.LanguageCode) ? "vi" : request.LanguageCode.Trim().ToLowerInvariant()
        };

        var emotionScores = analysis.AllEmotions.Select(x => new EmotionScore
        {
            EmotionEntryId = emotionEntry.Id,
            EmotionLabel = x.Label,
            Score = Convert.ToDecimal(x.Score)
        }).ToList();

        await _unitOfWork.EmotionRepository.AddEmotionEntryAsync(emotionEntry, cancellationToken);

        if (emotionScores.Count > 0)
        {
            await _unitOfWork.EmotionRepository.AddEmotionScoresAsync(emotionScores, cancellationToken);
        }

        if (analysis.Vector.Count > 0)
        {
            var embedding = new TextEmbedding
            {
                EmotionEntryId = emotionEntry.Id,
                ModelName = "local-fallback",
                ModelVersion = "1.0",
                Embedding = new Vector(analysis.Vector.ToArray())
            };

            await _unitOfWork.EmotionRepository.AddEmbeddingAsync(embedding, cancellationToken);
        }

        await _unitOfWork.SaveChangeAsync(cancellationToken);

        return new EmotionAnalysisResultDto
        {
            EmotionEntryId = emotionEntry.Id,
            TopEmotion = emotionEntry.TopEmotion,
            TopEmotionScore = emotionEntry.TopEmotionScore,
            SentimentScore = emotionEntry.SentimentScore,
            AllEmotions = emotionScores.Select(x => x.ToDto()).ToList(),
            Vector = analysis.Vector
        };
    }

    public async Task<EmotionEntryDto?> GetByIdAsync(Guid emotionEntryId, CancellationToken cancellationToken = default)
    {
        if (emotionEntryId == Guid.Empty)
        {
            throw new ArgumentException("EmotionEntryId is required.", nameof(emotionEntryId));
        }

        var emotionEntry = await _unitOfWork.EmotionRepository.GetByIdAsync(emotionEntryId, cancellationToken);
        return emotionEntry?.ToDto();
    }

    public async Task<List<EmotionEntryDto>> GetMyEntriesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.", nameof(userId));
        }

        var emotionEntries = await _unitOfWork.EmotionRepository.GetByUserIdAsync(userId, cancellationToken);
        return emotionEntries.ToDtoList();
    }
}
