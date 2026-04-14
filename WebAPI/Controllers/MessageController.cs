using Application.DTOs.Message;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> Send(SendMessageRequest request, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _messageService.SendAsync(request, userId, ct);
        return Ok(result);
    }

    [HttpGet("room/{roomId:guid}")]
    public async Task<ActionResult<List<MessageDto>>> GetByRoomId(Guid roomId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _messageService.GetRoomMessagesAsync(roomId, userId, ct);
        return Ok(result);
    }

    [HttpDelete("{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        await _messageService.DeleteAsync(messageId, userId, ct);
        return NoContent();
    }
}
