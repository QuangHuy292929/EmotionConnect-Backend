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

    [HttpPost("{matchingRequestId:guid}/room")]
    public async Task<ActionResult<RoomDto>> CreateRoom(Guid matchingRequestId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _matchingService.CreateRoomFromMatchingAsync(userId, matchingRequestId, ct);
        if (result == null)
        {
            return NotFound(new { message = "Unable to create room from the matching request." });
        }
        return Ok(result);
    }
}
