using Application.Interfaces.Common;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

[Authorize]
public class PresenceHub : Hub
{
    private readonly IUserPresenceService _userPresenceService;

    public PresenceHub(IUserPresenceService userPresenceService)
    {
        _userPresenceService = userPresenceService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.GetCurrentUserId()
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        await _userPresenceService.UserConnectedAsync(userId, Context.ConnectionId);

        // Chỉ broadcast UserOnline nếu đây là connection ĐẦU TIÊN
        var connectionCount = await _userPresenceService.GetActiveConnectionCountAsync(userId);
        if (connectionCount == 1)
        {
            await Clients.All.SendAsync("UserOnline", new { userId, isOnline = true });
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.User;
        if (user is not null)
        {
            try
            {
                var userId = user.GetCurrentUserId();
                await _userPresenceService.UserDisconnectedAsync(userId, Context.ConnectionId);

                // Chỉ broadcast UserOffline nếu KHÔNG còn connection nào
                var connectionCount = await _userPresenceService.GetActiveConnectionCountAsync(userId);
                if (connectionCount == 0)
                {
                    await Clients.All.SendAsync("UserOffline", new { userId, isOnline = false });
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Client gọi ngay sau khi connect để lấy danh sách userId đang online.
    /// Cần thiết vì các user đã online trước đó sẽ không emit UserOnline lại.
    /// </summary>
    public async Task<IEnumerable<string>> GetOnlineUsers()
    {
        return await _userPresenceService.GetOnlineUserIdsAsync();
    }
}