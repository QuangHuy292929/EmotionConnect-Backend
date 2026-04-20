using Application.DTOs.AI;
using Application.Exceptions;
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
        return result ?? throw new ExternalServiceException("AI service returned empty analyze response.");
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
        return result ?? throw new ExternalServiceException("AI service returned empty embedding response.");
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
        return result ?? throw new ExternalServiceException("AI service returned empty emotion response.");
    }

    public async Task<RewriteSummaryResponseDto> RewriteSummaryAsync(RewriteSummaryRequestDto request, CancellationToken ct = default)
    {
        if(request is null)
        {
            throw new BadRequestException("Request is required");
        }

        ValidateText(request.Text);

        var response = await _httpClient.PostAsJsonAsync("/api/rewrite-summary", new
        {
            text = request.Text.Trim()
        }, ct);

        await EnsureSuccessAsync(response, ct);

        var result = await response.Content.ReadFromJsonAsync<RewriteSummaryResponseDto>(ct);
        return result ?? throw new ExternalServiceException("AI service returned empty rewrite-summary response");
    }

    public async Task<ClarifySummaryResponseDto> ClarifySummaryAsync(ClarifySummaryRequestDto request, CancellationToken ct = default)
    {
        if(request is null)
        {
            throw new BadRequestException("request is required");
        }

        ValidateText(request.EmotionAnswer);
        ValidateText(request.IssueAnswer);
        ValidateText(request.DeepDiveAnswer);

        var response = await _httpClient.PostAsJsonAsync("/api/clarify-summary", new {
            emotionAnswer = request.EmotionAnswer.Trim(),
            issueAnswer = request.IssueAnswer.Trim(),
            deepDiveAnswer = request.DeepDiveAnswer.Trim()
        }, ct);

        await EnsureSuccessAsync(response, ct);

        var result = await response.Content.ReadFromJsonAsync<ClarifySummaryResponseDto>(ct);
        return result ?? throw new ExternalServiceException("AI service returned empty clarify-summary response");
    }

    private static void ValidateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new BadRequestException($"Text is required. {nameof(text)}");
        }
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new ExternalServiceException($"AI service request failed. Status: {(int)response.StatusCode}. Response: {errorBody}");
    }
}
