using Application.DTOs.Message;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task JoinRoom(Guid roomId)
    {
        var groupName = GetRoomGroupName(roomId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveRoom(Guid roomId)
    {
        var groupName = GetRoomGroupName(roomId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        var userId = Context.User?.GetCurrentUserId() ?? Guid.Empty;
        var message = await _messageService.SendAsync(request, userId, Context.ConnectionAborted);

        var groupName = GetRoomGroupName(request.RoomId);

        await Clients.Group(groupName).SendAsync("ReceiveMessage", message, Context.ConnectionAborted);
    }

    private static string GetRoomGroupName(Guid roomId) => $"room:{roomId}";

}
