using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.AI;

namespace Application.Interfaces.Common
{
    public interface IAiService
    {
        Task<AnalyzeResponseDto> AnalyzeAsync(string text, CancellationToken cancellationToken = default);
        Task<EmbeddingResponseDto> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
        Task<EmotionDetectionResponseDto> DetectEmotionAsync(string text, CancellationToken cancellationToken = default);
    }
}
