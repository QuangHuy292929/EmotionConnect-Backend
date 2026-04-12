using Application.DTOs.Community;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CommunityController : ControllerBase
{
    private readonly ICommunityService _communityService;

    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CommunityDto>>> GetAll(CancellationToken cancellationToken)
    {
        var communities = await _communityService.GetAllAsync(cancellationToken);
        return Ok(communities);
    }

    [HttpGet("{communityId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CommunityDto>> GetById(Guid communityId, CancellationToken cancellationToken)
    {
        var community = await _communityService.GetByIdAsync(communityId, cancellationToken);
        if (community is null)
        {
            return NotFound(new { message = "Community not found." });
        }

        return Ok(community);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<CommunityDto>>> GetMyCommunities(CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        var communities = await _communityService.GetJoinedCommunitiesAsync(userId, cancellationToken);
        return Ok(communities);
    }

    [HttpPost("{communityId:guid}/join")]
    public async Task<IActionResult> Join(Guid communityId, CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        await _communityService.JoinAsync(communityId, userId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{communityId:guid}/leave")]
    public async Task<IActionResult> Leave(Guid communityId, CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        return NoContent();
    }
}
