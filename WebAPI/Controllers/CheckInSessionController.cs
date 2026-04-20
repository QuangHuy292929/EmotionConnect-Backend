using Application.DTOs.CheckIn;
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
    public CheckInSessionController(ICheckInSessionService checkInSessionService)
    {
        _checkInSessionService = checkInSessionService;
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
        if (result == null)
        {
            return NotFound(new { message = "No active check in session found" });
        }
        return Ok(result);
    }

    [HttpPost("{sessionId:guid}/answer")]
    public async Task<ActionResult<CheckInStepResponseDto>> SubmitAnswer(Guid sessionId, SubmitCheckInAnswerRequest request, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _checkInSessionService.SubmitAnswerAsync(sessionId, userId, request, ct);
        return Ok(result);
    }

    [HttpPost("{sessionId:guid}/confirm")]
    public async Task<ActionResult<CheckInCompletedDto>> Confirm(Guid sessionId, ConfirmCheckInRequest request, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _checkInSessionService.ConfirmAsync(sessionId, userId, request, ct);
        return Ok(result);
    }

    [HttpPost("{sessionId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid sessionId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        await _checkInSessionService.CancelAsync(sessionId, userId, ct);
        return NoContent();
    }
}
