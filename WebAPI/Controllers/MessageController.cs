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
    public async Task<ActionResult<PagedResult<MessageDto>>> GetByRoomId(Guid roomId, [FromQuery]int pageNumber, [FromQuery]int pageSize, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _messageService.GetRoomMessagesAsync(roomId, userId, pageNumber, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> GetById(Guid messageId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _messageService.GetByIdAsync(messageId, userId, ct);
        return Ok(result);
    }

    [HttpGet("room/{roomId:guid}/search")]
    public async Task<ActionResult<PagedResult<MessageDto>>> SearchByRoomId(Guid roomId, [FromQuery]string keyword, [FromQuery]int pageNumber, [FromQuery]int pageSize, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _messageService.SearchRoomMessagesAsync(roomId, userId, keyword, pageNumber, pageSize, ct);
        return Ok(result);
    }

    [HttpPut("{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> EditMessage(Guid messageId, EditMessageRequest request, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _messageService.EditAsync(messageId, request, userId, ct);
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
