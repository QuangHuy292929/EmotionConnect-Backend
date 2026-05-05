using Application.DTOs.Friendship;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FriendshipController : ControllerBase
{
    private readonly IFriendshipService _friendshipService;
    public FriendshipController(IFriendshipService friendshipService)
    {
        _friendshipService = friendshipService;
    }

    [HttpPost("request")]
    public async Task<ActionResult<FriendshipDto>> AddFriend([FromBody]Guid addresseeId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.SendRequestAsync(userId, addresseeId, ct);
        return Ok(result);
    }

    [HttpPost("{friendshipId:guid}/accept")]
    public async Task<ActionResult<FriendshipDto>> AcceptRequest(Guid friendshipId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.AcceptRequestAsync(friendshipId, userId, ct);
        return Ok(result);
    }

    [HttpPost("{friendshipId:guid}/reject")]
    public async Task<ActionResult<FriendshipDto>> Reject(Guid friendshipId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.RejectRequestAsync(friendshipId, userId, ct);
        return Ok(result);
    }

    [HttpPost("{friendshipId:guid}/cancel")]
    public async Task<ActionResult<FriendshipDto>> CancelRequest(Guid friendshipId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.CancelRequestAsync(friendshipId, userId, ct);
        return Ok(result);
    }

    [HttpPost("{friendshipId:guid}/block")]
    public async Task<ActionResult<FriendshipDto>> BlockRequest(Guid friendshipId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.BlockAsync(friendshipId, userId, ct);
        return Ok(result);
    }

    [HttpDelete("{friendshipId:guid}")]
    public async Task<IActionResult> RemoveFriendship(Guid friendshipId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        await _friendshipService.RemoveFriendshipAsync(friendshipId, userId, ct);
        return NoContent();
    }

    [HttpGet("{friendshipId:guid}")]
    public async Task<ActionResult<FriendshipDto>> GetById(Guid friendshipId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.GetByIdAsync(friendshipId, userId, ct);
        return Ok(result);
    }

    [HttpGet("with/{otherUserId:guid}")]
    public async Task<ActionResult<FriendshipDto>> GetFriendshipWithUser(Guid otherUserId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.GetByUsersAsync(userId, otherUserId, ct);
        return Ok(result);
    }

    [HttpGet("incoming")]
    public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetIncomingRequests(CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.GetIncomingRequestsAsync(userId, ct);
        return Ok(result);
    }

    [HttpGet("outgoing")]
    public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetOutgoingRequests(CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.GetOutgoingRequestsAsync(userId, ct);
        return Ok(result);
    }

    [HttpGet("friends")]
    public async Task<ActionResult<IEnumerable<FriendshipDto>>> GetFriends(CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.GetFriendsAsync(userId, ct);
        return Ok(result);
    }

    [HttpGet("exists/{otherUserId:guid}")]
    public async Task<ActionResult<bool>> FriendshipExists(Guid otherUserId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _friendshipService.ExistsBetweenUsersAsync(userId, otherUserId, ct);
        return Ok(result);
    }
}