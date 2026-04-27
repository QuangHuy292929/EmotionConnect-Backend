using Application.DTOs.Matching;
using Application.DTOs.Room;
using Application.Interfaces.IServices;
using Humanizer;
using Infracstructure.Extensions;
using Infracstructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MatchingController : ControllerBase
{
    private readonly IMatchingService _matchingService;
    public MatchingController(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    [HttpPost]
    public async Task<ActionResult<MatchingResultDto>> CreateMatching(CreateMatchingRequest request, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _matchingService.CreateMatchingAsync(request.emtionEntryId, userId, ct);
        return Ok(result);
}

    [HttpGet("{matchingRequestId:guid}/candidates")]
    public async Task<ActionResult<List<MatchingCandidateDto>>> GetCandidates(Guid matchingRequestId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _matchingService.GetCandidatesAsync(userId, matchingRequestId, ct);
        return Ok(result);
    }

    [HttpPost("{matchingRequestId:guid}/join-or-create")]
    public async Task<ActionResult<MatchQueueResultDto>> JoinOrCreate(
        Guid matchingRequestId,
        CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _matchingService.JoinOrCreateQueueAsync(matchingRequestId, userId, ct);
        return Ok(result);
    }

    [HttpGet("{matchingRequestId:guid}/status")]
    public async Task<ActionResult<MatchQueueStatusDto>> GetQueueStatus(
        Guid matchingRequestId,
        CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _matchingService.GetQueueStatusAsync(matchingRequestId, userId, ct);
        return Ok(result);
    }

    [HttpPost("{matchingRequestId:guid}/leave-queue")]
    public async Task<IActionResult> LeaveQueue(
        Guid matchingRequestId,
        CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        await _matchingService.LeaveQueueAsync(matchingRequestId, userId, ct);
        return NoContent();
    }
}
