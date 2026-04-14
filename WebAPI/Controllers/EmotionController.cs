using Application.DTOs.Emotion;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmotionController : ControllerBase
{
    private readonly IEmotionService _emotionService;
    public EmotionController(IEmotionService emotionService)
    {
        _emotionService = emotionService;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<EmotionAnalysisResultDto>> Analyze(CreateEmotionEntryRequest request, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _emotionService.CreateAndAnalyzeAsync(request, userId, ct);
        return Ok(result);
    }

    [HttpGet("{emotionEntryId:guid}")]
    public async Task<ActionResult<EmotionEntryDto>> GetById(Guid emotionEntryId, CancellationToken ct)
    {
        var result = await _emotionService.GetByIdAsync(emotionEntryId, ct);
        if (result == null)
        {
            return NotFound(new { message = "Emotion Entry Not Found" });
        }
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<EmotionEntryDto>>> GetMyEntries(
       CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        var result = await _emotionService.GetMyEntriesAsync(userId, cancellationToken);
        return Ok(result);
    }
}
