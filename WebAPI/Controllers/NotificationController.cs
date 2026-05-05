using Application.DTOs.Notification;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationRequestDto request, CancellationToken ct)
    {
        var createdNotification = await _notificationService.CreateAsync(request, ct);
        return Ok(createdNotification);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> CreateNotificationsBulk(IEnumerable<CreateNotificationRequestDto> requests, CancellationToken ct)
    {
        var createdNotifications = await _notificationService.CreateRangeAsync(requests, ct);
        return Ok(createdNotifications);
    }

    [HttpGet("{notificationId:guid}")]
    public async Task<ActionResult<NotificationDto>> GetNotificationById(Guid notificationId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var notification = await _notificationService.GetByIdAsync(notificationId, userId, ct);
        if (notification == null)
        {
            return NotFound();
        }
        return Ok(notification);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var unreadCount = await _notificationService.GetUnreadCountAsync(userId, ct);
        return Ok(unreadCount);
    }

    [HttpPost("{notificationId:guid}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var result = await _notificationService.MarkAsReadAsync(notificationId, userId, ct);
        return Ok(result);
    }

    [HttpPost("mark-all-as-read")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var updatedCount = await _notificationService.MarkAllAsReadAsync(userId, ct);
        return Ok(new { UpdatedCount = updatedCount });
    }
}