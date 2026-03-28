using Application.DTOs.AI;
using Application.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Services
{
    public class AiService : IAiService
    {
        public Task<AnalyzeResponseDto> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<EmotionDetectionResponseDto> DetectEmotionAsync(string text, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<EmbeddingResponseDto> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
