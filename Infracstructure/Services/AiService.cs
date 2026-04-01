using Application.DTOs.AI;
using Application.Interfaces.Common;
using Infracstructure.AI;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Infracstructure.Services;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly AIServiceOptions _options;

    public AiService(HttpClient httpClient, IOptions<AIServiceOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<AnalyzeResponseDto> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        ValidateText(text);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/analyze",
            new { text = text.Trim() },
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<AnalyzeResponseDto>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("AI service returned empty analyze response.");
    }

    public async Task<EmbeddingResponseDto> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        ValidateText(text);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/embed",
            new { text = text.Trim() },
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<EmbeddingResponseDto>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("AI service returned empty embedding response.");
    }

    public async Task<EmotionDetectionResponseDto> DetectEmotionAsync(string text, CancellationToken cancellationToken = default)
    {
        ValidateText(text);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/emotion",
            new { text = text.Trim() },
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<EmotionDetectionResponseDto>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("AI service returned empty emotion response.");
    }

    private static void ValidateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text is required.", nameof(text));
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException(
            $"AI service request failed. Status: {(int)response.StatusCode}. Response: {errorBody}");
    }
}
