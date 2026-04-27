using Application.DTOs.AI;
using Application.DTOs.CheckIn;
using Application.Interfaces.Common;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CheckInSessionController : ControllerBase
{
    private readonly ICheckInSessionService _checkInSessionService;
    private readonly IAiService _aiService;

    public CheckInSessionController(
        ICheckInSessionService checkInSessionService,
        IAiService aiService)
    {
        _checkInSessionService = checkInSessionService;
        _aiService = aiService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<CheckInStartResponseDto>> Start(StartCheckInRequest request, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _checkInSessionService.StartAsync(userId, request, ct);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<CheckInSessionDto>> GetActive(CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _checkInSessionService.GetActiveSessionAsync(userId, ct);

        if (result is null)
        {
            return NotFound(new { message = "No active check in session found" });
        }

        return Ok(result);
    }

    [HttpPost("{sessionId:guid}/answer")]
    public async Task<ActionResult<CheckInStepResponseDto>> SubmitAnswer(
        Guid sessionId,
        SubmitCheckInAnswerRequest request,
        CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _checkInSessionService.SubmitAnswerAsync(sessionId, userId, request, ct);
        return Ok(result);
    }

    [HttpPost("{sessionId:guid}/confirm")]
    public async Task<ActionResult<CheckInCompletedDto>> Confirm(
        Guid sessionId,
        ConfirmCheckInRequest request,
        CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _checkInSessionService.ConfirmAsync(sessionId, userId, request, ct);
        return Ok(result);
    }

    [HttpPost("rewrite-summary")]
    public async Task<ActionResult<RewriteSummaryResponseDto>> RewriteSummary(
        RewriteSummaryRequestDto request,
        CancellationToken ct)
    {
        var result = await _aiService.RewriteSummaryAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<CheckInSessionDto>> GetById(Guid sessionId, CancellationToken ct)
    {
        var result = await _checkInSessionService.GetBySessionId(sessionId, ct);
        return Ok(result);
    }

    [HttpPost("{sessionId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid sessionId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        await _checkInSessionService.CancelAsync(sessionId, userId, ct);
        return NoContent();
    }

    [HttpGet("my-sessions")]
    public async Task<ActionResult<List<CheckInSessionDto>>> MySessions(CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _checkInSessionService.GetMySessions(userId, ct);
        return Ok(result);
    }
}
