using Application.DTOs.Room;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("{roomId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<RoomDto>> GetById(Guid roomId, CancellationToken cancellationToken)
    {
        var room = await _roomService.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            return NotFound(new { message = "Room not found." });
        }

        return Ok(room);
    }

    [HttpGet("community/{communityId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<RoomDto>>> GetByCommunity(Guid communityId, CancellationToken cancellationToken)
    {
        var rooms = await _roomService.GetByCommunityAsync(communityId, cancellationToken);
        return Ok(rooms);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<RoomDto>>> GetMyRooms(CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetCurrentUserId();
            var rooms = await _roomService.GetMyRoomsAsync(userId, cancellationToken);
            return Ok(rooms);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create(CreateRoomRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetCurrentUserId();
            var room = await _roomService.CreateAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { roomId = room.Id }, room);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{roomId:guid}/join")]
    public async Task<IActionResult> Join(Guid roomId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetCurrentUserId();
            await _roomService.JoinRoomAsync(roomId, userId, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{roomId:guid}/leave")]
    public async Task<IActionResult> Leave(Guid roomId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetCurrentUserId();
            await _roomService.LeaveRoomAsync(roomId, userId, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
